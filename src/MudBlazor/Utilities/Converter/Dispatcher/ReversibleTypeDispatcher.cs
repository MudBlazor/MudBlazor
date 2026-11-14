using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor.Utilities.Converter.Dispatcher;

/// <summary>
/// Helper that creates a type-based reversible dispatcher for converting values using per-type reversible converters.
/// </summary>
/// <remarks>
/// The returned dispatcher (built by the nested builder) allows registering reversible converters for specific concrete
/// input types and produces an <see cref="IReversibleConverter{TIn,TOut}"/> that will route forward and backward conversions
/// to the registered converters for the matching type.
/// </remarks>
public static class ReversibleTypeDispatcher
{
    /// <summary>
    /// Creates a new reversible dispatcher builder for dispatching conversions from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TIn">The general input type accepted by the resulting dispatcher.</typeparam>
    /// <typeparam name="TOut">The output type produced by registered reversible converters.</typeparam>
    /// <returns>
    /// A builder implementing <see cref="IReversibleDispatcherBuilder{TIn,TOut}"/>
    /// to register per-type reversible converters and produce a concrete dispatcher via <see cref="IReversibleDispatcherBuilder{TIn,TOut}.Build"/>.
    /// </returns>
    public static IReversibleDispatcherBuilder<TIn, TOut> Create<TIn, TOut>()
        => new ReversibleTypeDispatcher<TIn, TOut>.ReversibleBuilder(DispatcherRegistrationPolicy.LastWins);

    /// <summary>
    /// Creates a new reversible dispatcher builder for dispatching conversions from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TIn">The general input type accepted by the resulting dispatcher.</typeparam>
    /// <typeparam name="TOut">The output type produced by registered reversible converters.</typeparam>
    /// <param name="duplicateRegistrationPolicy">How registrations for the same concrete type are handled.</param>
    /// <returns>
    /// A builder implementing <see cref="IReversibleDispatcherBuilder{TIn,TOut}"/>
    /// to register per-type reversible converters and produce a concrete dispatcher via <see cref="IReversibleDispatcherBuilder{TIn,TOut}.Build"/>.
    /// </returns>
    public static IReversibleDispatcherBuilder<TIn, TOut> Create<TIn, TOut>(DispatcherRegistrationPolicy duplicateRegistrationPolicy)
        => new ReversibleTypeDispatcher<TIn, TOut>.ReversibleBuilder(duplicateRegistrationPolicy);
}

internal class ReversibleTypeDispatcher<TIn, TOut> :
    TypeDispatcher<TIn, TOut>, IReversibleConverter<TIn, TOut>
{
    private readonly Func<TOut, TIn>? _backward;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReversibleTypeDispatcher{TIn,TOut}"/> class.
    /// </summary>
    /// <param name="forward">The resolved forward delegate for <typeparamref name="TIn"/>, or <c>null</c> when no converter is registered.</param>
    /// <param name="backward">The resolved backward delegate for <typeparamref name="TIn"/>, or <c>null</c> when no converter is registered.</param>
    protected ReversibleTypeDispatcher(Func<TIn, TOut>? forward, Func<TOut, TIn>? backward)
        : base(forward) => _backward = backward;

    internal static Func<TOut, TIn>? ResolveBackwardHandler(Dictionary<Type, Delegate> reverseHandlers)
    {
        var runtimeType = typeof(TIn);
        if (reverseHandlers.TryGetValue(runtimeType, out var del))
        {
            return del as Func<TOut, TIn> ?? (input => (TIn)del.DynamicInvoke(input)!);
        }

        return null;
    }

    /// <inheritdoc />
    /// <exception cref="ConversionException">
    /// Thrown when no backward converter has been registered for the resolved type. The exception contains a localization key
    /// and an inner exception describing the missing registration.
    /// </exception>
    public TIn ConvertBack(TOut input)
    {
        if (_backward is not null)
        {
            return _backward(input);
        }

        var runtimeType = typeof(TIn);

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [runtimeType], new InvalidOperationException($"No converter registered for {runtimeType}"));
    }

    internal class ReversibleBuilder(DispatcherRegistrationPolicy registrationPolicy)
        : IReversibleDispatcherBuilder<TIn, TOut>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();
        private readonly Dictionary<Type, Delegate> _reverseHandlers = new();

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> Add<TSpecific>(IReversibleConverter<TSpecific, TOut> converter)
        {
            AddHandlers(
                typeof(TSpecific),
                new Func<TSpecific, TOut>(converter.Convert),
                new Func<TOut, TSpecific>(converter.ConvertBack));

            return this;
        }

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> AddForward<TSpecific>(IConverter<TSpecific, TOut> converter)
        {
            AddForwardHandler(
                typeof(TSpecific),
                new Func<TSpecific, TOut>(converter.Convert));

            return this;
        }

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object? converter)
        {
            ArgumentNullException.ThrowIfNull(specificType);
            ArgumentNullException.ThrowIfNull(converter);

            var forwardDelegate = DelegateHelper.CreateForwardDelegate<TIn, TOut>(specificType, converter);
            var backwardDelegate = DelegateHelper.CreateBackwardDelegate<TIn, TOut>(specificType, converter);

            AddHandlers(specificType, forwardDelegate, backwardDelegate);

            return this;
        }

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> AddDynamicForward(Type specificType, object? converter)
        {
            ArgumentNullException.ThrowIfNull(specificType);
            ArgumentNullException.ThrowIfNull(converter);

            var forwardDelegate = DelegateHelper.CreateForwardDelegate<TIn, TOut>(specificType, converter);

            AddForwardHandler(specificType, forwardDelegate);

            return this;
        }

        private void AddForwardHandler(Type specificType, Delegate forwardHandler)
        {
            switch (registrationPolicy)
            {
                case DispatcherRegistrationPolicy.LastWins:
                    _handlers[specificType] = forwardHandler;
                    return;
                case DispatcherRegistrationPolicy.FirstWins:
                    _handlers.TryAdd(specificType, forwardHandler);
                    return;
                case DispatcherRegistrationPolicy.Throw:
                    if (!_handlers.TryAdd(specificType, forwardHandler))
                    {
                        throw new InvalidOperationException($"Converter already registered for {specificType}.");
                    }

                    return;
                default:
                    throw new InvalidOperationException($"Unsupported registration policy: {registrationPolicy}.");
            }
        }

        private void AddHandlers(Type specificType, Delegate forwardHandler, Delegate backwardHandler)
        {
            switch (registrationPolicy)
            {
                case DispatcherRegistrationPolicy.LastWins:
                    _handlers[specificType] = forwardHandler;
                    _reverseHandlers[specificType] = backwardHandler;
                    return;
                case DispatcherRegistrationPolicy.FirstWins:
                    _handlers.TryAdd(specificType, forwardHandler);
                    _reverseHandlers.TryAdd(specificType, backwardHandler);
                    return;
                case DispatcherRegistrationPolicy.Throw:
                    if (_handlers.ContainsKey(specificType) || _reverseHandlers.ContainsKey(specificType))
                    {
                        throw new InvalidOperationException($"Converter already registered for {specificType}.");
                    }

                    _handlers.Add(specificType, forwardHandler);
                    _reverseHandlers.Add(specificType, backwardHandler);
                    return;
                default:
                    throw new InvalidOperationException($"Unsupported registration policy: {registrationPolicy}.");
            }
        }

        /// <inheritdoc />
        public IReversibleConverter<TIn, TOut> Build()
        {
            var forward = ResolveForwardHandler(_handlers);
            var backward = ResolveBackwardHandler(_reverseHandlers);
            return new ReversibleTypeDispatcher<TIn, TOut>(forward, backward);
        }
    }
}

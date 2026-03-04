using System.Reflection;
using System.Runtime.ExceptionServices;
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
    private readonly Dictionary<Type, Delegate> _backwards;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReversibleTypeDispatcher{TIn,TOut}"/> class.
    /// </summary>
    /// <param name="forwards">A pre-populated map of concrete input <see cref="Type"/> to forward conversion delegates.</param>
    /// <param name="backwards">A pre-populated map of concrete input <see cref="Type"/> to backward conversion delegates.</param>
    protected ReversibleTypeDispatcher(
        Dictionary<Type, Delegate> forwards,
        Dictionary<Type, Delegate> backwards)
        : base(forwards)
    {
        _backwards = backwards;
    }

    /// <inheritdoc />
    /// <exception cref="ConversionException">
    /// Thrown when no backward converter has been registered for the resolved type. The exception contains a localization key
    /// and an inner exception describing the missing registration.
    /// </exception>
    public TIn ConvertBack(TOut input)
    {
        var runtimeType = typeof(TIn);

        if (_backwards.TryGetValue(runtimeType, out var del))
        {
            try
            {
                return (TIn)del.DynamicInvoke(input)!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

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
        public IReversibleDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object? converter)
        {
            ArgumentNullException.ThrowIfNull(specificType);
            ArgumentNullException.ThrowIfNull(converter);

            var convType = converter.GetType();

            var convertMethodInterface = typeof(IConverter<,>).MakeGenericType(specificType, typeof(TOut));
            if (!convertMethodInterface.IsAssignableFrom(convType))
            {
                throw new InvalidOperationException($"Converter type {convType.FullName} does not implement Convert({specificType})");
            }

            var convertMethod = convertMethodInterface.GetMethod(nameof(IConverter<TIn, TOut>.Convert), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var convertBackMethodInterface = typeof(IReversibleConverter<,>).MakeGenericType(specificType, typeof(TOut));
            if (!convertBackMethodInterface.IsAssignableFrom(convType))
            {
                throw new InvalidOperationException($"Converter type {convType.FullName} does not implement ConvertBack({typeof(TOut)})");
            }

            var convertBackMethod = convertBackMethodInterface.GetMethod(nameof(IReversibleConverter<TIn, TOut>.ConvertBack), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // Cannot be null since we already verified the interface is implemented
            var forwardDelegate = convertMethod!.CreateDelegate(typeof(Func<,>).MakeGenericType(specificType, typeof(TOut)), converter);
            var backwardDelegate = convertBackMethod!.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(TOut), specificType), converter);

            AddHandlers(specificType, forwardDelegate, backwardDelegate);

            return this;
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
        public IReversibleConverter<TIn, TOut> Build() => new ReversibleTypeDispatcher<TIn, TOut>(_handlers, _reverseHandlers);
    }
}

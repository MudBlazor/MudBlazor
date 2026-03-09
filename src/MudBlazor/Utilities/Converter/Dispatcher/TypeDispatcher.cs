using System.Reflection;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor.Utilities.Converter.Dispatcher;

/// <summary>
/// Helper that creates a type-based dispatcher for converting values using per-type converters.
/// </summary>
/// <remarks>
/// The returned dispatcher (built by the nested builder) allows registering converters for specific concrete
/// input types and produces an <see cref="IConverter{TIn,TOut}"/> that will route conversions to the
/// registered converter for the matching type.
/// </remarks>
public static class TypeDispatcher
{
    /// <summary>
    /// Creates a new dispatcher builder for dispatching conversions from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TIn">The general input type accepted by the resulting dispatcher.</typeparam>
    /// <typeparam name="TOut">The output type produced by registered converters.</typeparam>
    /// <returns>
    /// A builder implementing <see cref="IDispatcherBuilder{TIn,TOut}"/> to register per-type converters
    /// and produce a concrete dispatcher via <see cref="IDispatcherBuilder{TIn,TOut}.Build"/>.
    /// </returns>
    public static IDispatcherBuilder<TIn, TOut> Create<TIn, TOut>()
        => new TypeDispatcher<TIn, TOut>.Builder(DispatcherRegistrationPolicy.LastWins);

    /// <summary>
    /// Creates a new dispatcher builder for dispatching conversions from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TIn">The general input type accepted by the resulting dispatcher.</typeparam>
    /// <typeparam name="TOut">The output type produced by registered converters.</typeparam>
    /// <param name="duplicateRegistrationPolicy">How registrations for the same concrete type are handled.</param>
    /// <returns>
    /// A builder implementing <see cref="IDispatcherBuilder{TIn,TOut}"/> to register per-type converters
    /// and produce a concrete dispatcher via <see cref="IDispatcherBuilder{TIn,TOut}.Build"/>.
    /// </returns>
    public static IDispatcherBuilder<TIn, TOut> Create<TIn, TOut>(DispatcherRegistrationPolicy duplicateRegistrationPolicy)
        => new TypeDispatcher<TIn, TOut>.Builder(duplicateRegistrationPolicy);
}

internal class TypeDispatcher<TIn, TOut> : IConverter<TIn, TOut>
{
    private readonly Func<TIn, TOut>? _forward;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDispatcher{TIn,TOut}"/> class.
    /// </summary>
    /// <param name="forward">The resolved forward delegate for <typeparamref name="TIn"/>, or <c>null</c> when no converter is registered.</param>
    protected TypeDispatcher(Func<TIn, TOut>? forward) => _forward = forward;

    internal static Func<TIn, TOut>? ResolveForwardHandler(Dictionary<Type, Delegate> handlers)
    {
        var runtimeType = typeof(TIn);
        if (handlers.TryGetValue(runtimeType, out var del))
        {
            return del as Func<TIn, TOut> ?? (input => (TOut)del.DynamicInvoke(input)!);
        }

        return null;
    }

    /// <inheritdoc />
    /// <exception cref="ConversionException">
    /// Thrown when no converter has been registered for the resolved type. The exception contains a localization key and an inner exception
    /// describing the missing registration.
    /// </exception>
    public TOut Convert(TIn input)
    {
        if (_forward is not null)
        {
            return _forward(input);
        }

        var runtimeType = typeof(TIn);

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [runtimeType], new InvalidOperationException($"No converter registered for {runtimeType}"));
    }

    internal class Builder(DispatcherRegistrationPolicy registrationPolicy) : IDispatcherBuilder<TIn, TOut>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        /// <inheritdoc />
        public IDispatcherBuilder<TIn, TOut> Add<TSpecific>(IConverter<TSpecific, TOut> converter)
        {
            AddHandler(typeof(TSpecific), new Func<TSpecific, TOut>(converter.Convert));

            return this;
        }

        /// <inheritdoc />
        public IDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object converter)
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

            // Cannot be null since we already verified the interface is implemented
            var forwardDelegate = convertMethod!.CreateDelegate(typeof(Func<,>).MakeGenericType(specificType, typeof(TOut)), converter);

            AddHandler(specificType, forwardDelegate);

            return this;
        }

        private void AddHandler(Type specificType, Delegate handler)
        {
            switch (registrationPolicy)
            {
                case DispatcherRegistrationPolicy.LastWins:
                    _handlers[specificType] = handler;
                    return;
                case DispatcherRegistrationPolicy.FirstWins:
                    _handlers.TryAdd(specificType, handler);
                    return;
                case DispatcherRegistrationPolicy.Throw:
                    if (!_handlers.TryAdd(specificType, handler))
                    {
                        throw new InvalidOperationException($"Converter already registered for {specificType}.");
                    }

                    return;
                default:
                    throw new InvalidOperationException($"Unsupported registration policy: {registrationPolicy}.");
            }
        }

        /// <inheritdoc />
        public IConverter<TIn, TOut> Build()
        {
            var forward = ResolveForwardHandler(_handlers);
            return new TypeDispatcher<TIn, TOut>(forward);
        }
    }
}

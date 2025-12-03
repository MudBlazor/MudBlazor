using System.Reflection;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor.Utilities.Converter.Dispatcher;

#nullable enable
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
    public static IDispatcherBuilder<TIn, TOut> Create<TIn, TOut>() => new TypeDispatcher<TIn, TOut>.Builder();
}

internal class TypeDispatcher<TIn, TOut> : IConverter<TIn, TOut>
{
    private readonly Dictionary<Type, Delegate> _handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDispatcher{TIn,TOut}"/> class.
    /// </summary>
    /// <param name="handlers">A pre-populated map of concrete input <see cref="Type"/> to conversion delegates.</param>
    protected TypeDispatcher(Dictionary<Type, Delegate> handlers)
    {
        _handlers = handlers;
    }

    /// <inheritdoc />
    /// <exception cref="ConversionException">
    /// Thrown when no converter has been registered for the resolved type. The exception contains a localization key and an inner exception
    /// describing the missing registration.
    /// </exception>
    public TOut Convert(TIn input)
    {
        //var runtimeType = input is null ? typeof(TIn) : input.GetType();
        var runtimeType = typeof(TIn);

        if (_handlers.TryGetValue(runtimeType, out var del))
        {
            return (TOut)del.DynamicInvoke(input)!;
        }

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [runtimeType], new InvalidOperationException($"No converter registered for {runtimeType}"));
    }

    internal class Builder : IDispatcherBuilder<TIn, TOut>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        /// <inheritdoc />
        public IDispatcherBuilder<TIn, TOut> Add<TSpecific>(IConverter<TSpecific, TOut> converter)
        {
            _handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);

            return this;
        }

        /// <inheritdoc />
        public IDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object converter)
        {
            ArgumentNullException.ThrowIfNull(specificType);
            ArgumentNullException.ThrowIfNull(converter);

            var convType = converter.GetType();

            var convertMethodInterface = typeof(IConverter<,>).MakeGenericType(specificType, typeof(TOut));
            var convertMethod = convertMethodInterface.GetMethod(nameof(IConverter<,>.Convert), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (convertMethod is null)
            {
                throw new InvalidOperationException($"Converter type {convType.FullName} does not implement Convert({specificType})");
            }

            var forwardDelegate = convertMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(specificType, typeof(TOut)), converter);

            _handlers[specificType] = forwardDelegate;

            return this;
        }

        /// <inheritdoc />
        public IConverter<TIn, TOut> Build() => new TypeDispatcher<TIn, TOut>(_handlers);
    }
}

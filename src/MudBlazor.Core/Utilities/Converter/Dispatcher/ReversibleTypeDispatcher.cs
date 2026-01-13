using System.Reflection;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor.Utilities.Converter.Dispatcher;

#nullable enable
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
    public static IReversibleDispatcherBuilder<TIn, TOut> Create<TIn, TOut>() => new ReversibleTypeDispatcher<TIn, TOut>.ReversibleBuilder();
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
        //var runtimeType = input is null ? typeof(TIn) : input.GetType();
        var runtimeType = typeof(TIn);

        if (_backwards.TryGetValue(runtimeType, out var del))
        {
            return (TIn)del.DynamicInvoke(input)!;
        }

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [runtimeType], new InvalidOperationException($"No converter registered for {runtimeType}"));
    }

    internal class ReversibleBuilder : IReversibleDispatcherBuilder<TIn, TOut>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();
        private readonly Dictionary<Type, Delegate> _reverseHandlers = new();

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> Add<TSpecific>(IReversibleConverter<TSpecific, TOut> converter)
        {
            _handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);

            // backward
            _reverseHandlers[typeof(TSpecific)] = new Func<TOut, TSpecific>(converter.ConvertBack);

            return this;
        }

        /// <inheritdoc />
        public IReversibleDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object? converter)
        {
            ArgumentNullException.ThrowIfNull(specificType);
            ArgumentNullException.ThrowIfNull(converter);

            var convType = converter.GetType();

            var convertMethodInterface = typeof(IConverter<,>).MakeGenericType(specificType, typeof(TOut));
            var convertMethod = convertMethodInterface.GetMethod(nameof(IConverter<TIn, TOut>.Convert), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (convertMethod is null)
            {
                throw new InvalidOperationException($"Converter type {convType.FullName} does not implement Convert({specificType})");
            }

            var convertBackMethodInterface = typeof(IReversibleConverter<,>).MakeGenericType(specificType, typeof(TOut));
            var convertBackMethod = convertBackMethodInterface.GetMethod(nameof(IReversibleConverter<TIn, TOut>.ConvertBack), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (convertBackMethod is null)
            {
                throw new InvalidOperationException($"Converter type {convType.FullName} does not implement ConvertBack({typeof(TOut)})");
            }

            var forwardDelegate = convertMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(specificType, typeof(TOut)), converter);
            var backwardDelegate = convertBackMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(TOut), specificType), converter);

            _handlers[specificType] = forwardDelegate;
            _reverseHandlers[specificType] = backwardDelegate;

            return this;
        }

        /// <inheritdoc />
        public IReversibleConverter<TIn, TOut> Build() => new ReversibleTypeDispatcher<TIn, TOut>(_handlers, _reverseHandlers);
    }
}

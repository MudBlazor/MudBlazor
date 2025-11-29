using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Dispatcher;

#nullable enable
public static class ReversibleTypeDispatcher
{
    public static IReversibleDispatcherBuilder<TIn, TOut, IReversibleConverter<TIn, TOut>> Create<TIn, TOut>() => new ReversibleTypeDispatcher<TIn, TOut>.ReversibleBuilder();
}

internal class ReversibleTypeDispatcher<TIn, TOut> :
    TypeDispatcher<TIn, TOut>, IReversibleConverter<TIn, TOut>
{
    private readonly Dictionary<Type, Delegate> _backwards;

    public ReversibleTypeDispatcher(
        Dictionary<Type, Delegate> forwards,
        Dictionary<Type, Delegate> backwards)
        : base(forwards)
    {
        _backwards = backwards;
    }

    public TIn ConvertBack(TOut output)
    {
        var type = typeof(TIn);

        if (_backwards.TryGetValue(type, out var del))
        {
            return (TIn)del.DynamicInvoke(output)!;
        }

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [type], new InvalidOperationException($"No converter registered for {type}"));
    }

    internal class ReversibleBuilder : IReversibleDispatcherBuilder<TIn, TOut, IReversibleConverter<TIn, TOut>>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();
        private readonly Dictionary<Type, Delegate> _reverseHandlers = new();

        public IReversibleDispatcherBuilder<TIn, TOut, IReversibleConverter<TIn, TOut>> Add<TSpecific>(IReversibleConverter<TSpecific, TOut> converter)
        {
            _handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);

            // backward
            _reverseHandlers[typeof(TSpecific)] = new Func<TOut, TSpecific>(converter.ConvertBack);

            return this;
        }

        public IReversibleConverter<TIn, TOut> Build()
        {
            return new ReversibleTypeDispatcher<TIn, TOut>(_handlers, _reverseHandlers);
        }
    }
}

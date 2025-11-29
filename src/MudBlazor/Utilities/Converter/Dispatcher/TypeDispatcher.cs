using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Dispatcher;

#nullable enable
public static class TypeDispatcher
{
    public static IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>> Create<TIn, TOut>() => new TypeDispatcher<TIn, TOut>.Builder();
}

internal class TypeDispatcher<TIn, TOut> : IConverter<TIn, TOut>
{
    private readonly Dictionary<Type, Delegate> _handlers;

    protected TypeDispatcher(Dictionary<Type, Delegate> handlers)
    {
        _handlers = handlers;
    }

    public TOut Convert(TIn input)
    {
        //var runtimeType = input is null ? typeof(TIn) : input.GetType();
        var runtimeType = typeof(TIn);

        // 1) Static lookup
        if (_handlers.TryGetValue(runtimeType, out var del))
        {
            return (TOut)del.DynamicInvoke(input)!;
        }

        // 2) Dynamic factories (future)

        throw new ConversionException(LanguageResource.Converter_ConversionNotImplemented, [runtimeType], new InvalidOperationException($"No converter registered for {runtimeType}"));
    }

    internal class Builder : IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>> Add<TSpecific>(IConverter<TSpecific, TOut> converter)
        {
            _handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);

            return this;
        }

        public IConverter<TIn, TOut> Build() => new TypeDispatcher<TIn, TOut>(_handlers);
    }
}

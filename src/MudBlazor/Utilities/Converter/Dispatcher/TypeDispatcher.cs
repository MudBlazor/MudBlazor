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
        var type = typeof(TIn);

        if (!_handlers.TryGetValue(type, out var del))
        {
            throw new InvalidOperationException($"No converter registered for {type}");
        }

        return (TOut)del.DynamicInvoke(input)!;
    }

    // public static Builder Create() => new();

    //public static IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>, TypeDispatcher<TIn, TOut>> Create() => new Builder();

    internal class Builder : IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public IDispatcherBuilder<TIn, TOut, IConverter<TIn, TOut>> Add<TSpecific>(IConverter<TSpecific, TOut> converter)
        {
            _handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);

            return this;
        }

        public IConverter<TIn, TOut> Build()
        {
            return new TypeDispatcher<TIn, TOut>(_handlers);
        }
    }

    //internal class Builder
    //{
    //    protected readonly Dictionary<Type, Delegate> Handlers = new();

    //    public Builder Add<TSpecific>(IConverter<TSpecific, TOut> converter)
    //    {
    //        Handlers[typeof(TSpecific)] = new Func<TSpecific, TOut>(converter.Convert);
    //        return this;
    //    }

    //    public TypeDispatcher<TIn, TOut> Build() => new TypeDispatcher<TIn, TOut>(Handlers);
    //}
}

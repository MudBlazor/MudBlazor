using MudBlazor.Utilities.Converter.Chain;

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public static class Convert
{
    public static ConverterChain<TIn, TOut> From<TIn, TOut>(IConverter<TIn, TOut> converter) => new(converter.Convert);

    public static ReversibleChain<TIn, TOut> From<TIn, TOut>(IReversibleConverter<TIn, TOut> converter) =>
        new(converter.Convert, converter.ConvertBack);

    public static ConverterChain<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> forward) => new(forward);

    public static ReversibleChain<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> forward, Func<TOut, TIn> backward) =>
        new(forward, backward);
}

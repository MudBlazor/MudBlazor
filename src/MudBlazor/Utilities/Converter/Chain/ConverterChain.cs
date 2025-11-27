using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Chain;

#nullable enable
public class ConverterChain<TIn, TOut> : IConverter<TIn, TOut>
{
    protected readonly Func<TIn, TOut> Forward;

    public ConverterChain(Func<TIn, TOut> forward)
    {
        Forward = forward;
    }

    public TOut Convert(TIn input) => Forward(input);

    public ConverterChain<TIn, TNext> Then<TNext>(IConverter<TOut, TNext> next) => new(x => next.Convert(Forward(x)));

    public ConverterChain<TIn, TNext> Then<TNext>(Func<TOut, TNext> func) => new(x => func(Forward(x)));


    public ConversionResult<TOut> TryConvert(TIn input)
    {
        try
        {
            return new ConversionResult<TOut>(Forward(input));
        }
        catch (Exception ex)
        {
            return new ConversionResult<TOut>(ex);
        }
    }
}

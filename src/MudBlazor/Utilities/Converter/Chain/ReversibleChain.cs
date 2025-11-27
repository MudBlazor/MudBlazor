using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Chain;

#nullable enable
public sealed class ReversibleChain<TIn, TOut> : ConverterChain<TIn, TOut>, IReversibleConverter<TIn, TOut>
{
    private readonly Func<TOut, TIn> _backward;

    public ReversibleChain(Func<TIn, TOut> forward, Func<TOut, TIn> backward)
        : base(forward)
    {
        _backward = backward;
    }

    public TIn ConvertBack(TOut output) => _backward(output);

    public ReversibleChain<TOut, TIn> Reverse() => new(_backward, Forward);

    public ReversibleChain<TIn, TNext> Then<TNext>(IReversibleConverter<TOut, TNext> next)
        => new(
            x => next.Convert(Forward(x)),
            o => _backward(next.ConvertBack(o))
        );

    public ConversionResult<TIn> TryConvertBack(TOut input)
    {
        try
        {
            return new ConversionResult<TIn>(_backward(input));
        }
        catch (Exception ex)
        {
            return new ConversionResult<TIn>(ex);
        }
    }
}

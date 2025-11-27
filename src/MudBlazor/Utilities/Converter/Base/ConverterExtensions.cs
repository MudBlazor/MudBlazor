using MudBlazor.Utilities.Converter.Chain;

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public static class ConverterExtensions
{
    public static ReversibleChain<TOut, TIn> Reverse<TIn, TOut>(this IReversibleConverter<TIn, TOut> converter)
    {
        return new ReversibleChain<TOut, TIn>(converter.ConvertBack, converter.Convert);
    }

    public static ConversionResult<TOut> TryConvert<TIn, TOut>(this IConverter<TIn, TOut> converter, TIn input)
    {
        try
        {
            return new ConversionResult<TOut>(converter.Convert(input));
        }
        catch (Exception ex)
        {
            return new ConversionResult<TOut>(ex);
        }
    }

    public static ConversionResult<TIn> TryConvertBack<TIn, TOut>(this IReversibleConverter<TIn, TOut> converter, TOut input)
    {
        try
        {
            return new ConversionResult<TIn>(converter.ConvertBack(input));
        }
        catch (Exception ex)
        {
            return new ConversionResult<TIn>(ex);
        }
    }

    /// <summary>
    /// Convert back using a reversible converter if supported.
    /// Throws an exception if the converter does not support ConvertBack().
    /// </summary>
    public static TOut ConvertBack<TOut, TIn>(this IConverter<TOut, TIn> converter, TIn value)
    {
        if (converter is IReversibleConverter<TOut, TIn> reversible)
        {
            return reversible.ConvertBack(value);
        }

        throw new InvalidOperationException($"Converter {converter.GetType().Name} does not support ConvertBack. Implement an IReversibleConverter for the converter instead.");
    }
}

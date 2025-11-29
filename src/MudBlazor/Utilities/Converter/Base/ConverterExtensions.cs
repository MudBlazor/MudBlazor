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
        return Wrap(() => converter.Convert(input));
    }

    public static ConversionResult<TIn> TryConvertBack<TIn, TOut>(this IConverter<TIn, TOut> converter, TOut input)
    {
        return Wrap(() => converter.ConvertBack(input));
    }

    public static ConversionResult<TIn> TryConvertBack<TIn, TOut>(this IReversibleConverter<TIn, TOut> converter, TOut input)
    {
        return Wrap(() => converter.ConvertBack(input));
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

        // TODO: throw ConversionException
        throw new InvalidOperationException($"Converter {converter.GetType().Name} does not support ConvertBack. Implement an IReversibleConverter for the converter instead.");
    }

    private static ConversionResult<T> Wrap<T>(Func<T> func)
    {
        try
        {
            return new ConversionResult<T>(func());
        }
        catch (Exception ex)
        {
            // Direct ConversionException
            if (ex is ConversionException conversionException)
            {
                return new ConversionResult<T>(conversionException, conversionException.ErrorMessageKey, conversionException.ErrorMessageArgs);
            }

            // Wrapped
            if (ex.InnerException is ConversionException innerExceptionConversionException)
            {
                return new ConversionResult<T>(innerExceptionConversionException, innerExceptionConversionException.ErrorMessageKey, innerExceptionConversionException.ErrorMessageArgs);
            }

            // AggregateException containing ConversionException
            if (ex is AggregateException aggregateException)
            {
                var aggregateConversionException = aggregateException.InnerExceptions.OfType<ConversionException>().FirstOrDefault();
                if (aggregateConversionException is not null)
                {
                    return new ConversionResult<T>(aggregateConversionException, aggregateConversionException.ErrorMessageKey, aggregateConversionException.ErrorMessageArgs);
                }
            }

            // Unknown exception
            return new ConversionResult<T>(ex);
        }
    }
}

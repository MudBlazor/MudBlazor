using MudBlazor.Utilities.Converter.Base;
using MudBlazor.Utilities.Converter.Chain;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor.Extensions;

#nullable enable
/// <summary>
/// Extension helpers for working with <see cref="IConverter{TIn,TOut}"/> and <see cref="IReversibleConverter{TIn,TOut}"/>.
/// </summary>
/// <remarks>
/// Provides utilities for reversing reversible converters, attempting conversions with captured errors (<see cref="ConversionResult{T}"/>),
/// and invoking <c>ConvertBack</c> on a converter when supported.
/// </remarks>
public static class ConverterExtensions
{
    /// <summary>
    /// Creates a <see cref="ReversibleChain{TOut,TIn}"/> that reverses the forward/backward delegates of the supplied reversible converter.
    /// </summary>
    /// <typeparam name="TIn">Input type of the original converter.</typeparam>
    /// <typeparam name="TOut">Output type of the original converter.</typeparam>
    /// <param name="converter">The reversible converter to reverse.</param>
    /// <returns>
    /// A <see cref="ReversibleChain{TOut,TIn}"/> whose forward conversion delegates to <see cref="IReversibleConverter{TIn,TOut}.ConvertBack"/>
    /// and whose backward conversion delegates to <see cref="IConverter{TIn,TOut}.Convert"/>.
    /// </returns>
    public static ReversibleChain<TOut, TIn> Reverse<TIn, TOut>(this IReversibleConverter<TIn, TOut> converter)
    {
        return new ReversibleChain<TOut, TIn>(converter.ConvertBack, converter.Convert);
    }

    /// <summary>
    /// Attempts to convert <paramref name="input"/> using <paramref name="converter"/> and returns a <see cref="ConversionResult{TOut}"/>
    /// that captures either the converted value or details about the failure.
    /// </summary>
    /// <typeparam name="TIn">Input type accepted by the converter.</typeparam>
    /// <typeparam name="TOut">Output type produced by the converter.</typeparam>
    /// <param name="converter">The converter to invoke.</param>
    /// <param name="input">The input value to convert.</param>
    /// <returns>
    /// A <see cref="ConversionResult{TOut}"/> where <see cref="ConversionResult{TOut}.Success"/> is <c>true</c> on success,
    /// otherwise it contains the exception and optional error metadata.
    /// </returns>
    public static ConversionResult<TOut> TryConvert<TIn, TOut>(this IConverter<TIn, TOut> converter, TIn input)
    {
        return Wrap(() => converter.Convert(input));
    }

    /// <summary>
    /// Attempts to perform a backward conversion using a converter and returns a <see cref="ConversionResult{TIn}"/>.
    /// </summary>
    /// <typeparam name="TIn">The expected result type of the backward conversion.</typeparam>
    /// <typeparam name="TOut">The input type to the backward conversion.</typeparam>
    /// <param name="converter">The converter (may be reversible) to use for converting back.</param>
    /// <param name="input">The value to convert back.</param>
    /// <returns>
    /// A <see cref="ConversionResult{TIn}"/> containing the converted value on success or error information on failure.
    /// </returns>
    public static ConversionResult<TIn> TryConvertBack<TIn, TOut>(this IConverter<TIn, TOut> converter, TOut input)
    {
        return Wrap(() => converter.ConvertBack(input));
    }

    /// <summary>
    /// Attempts to perform a backward conversion using a reversible converter and returns a <see cref="ConversionResult{TIn}"/>.
    /// </summary>
    /// <typeparam name="TIn">The expected result type of the backward conversion.</typeparam>
    /// <typeparam name="TOut">The input type to the backward conversion.</typeparam>
    /// <param name="converter">The reversible converter to use for converting back.</param>
    /// <param name="input">The value to convert back.</param>
    /// <returns>
    /// A <see cref="ConversionResult{TIn}"/> containing the converted value on success or error information on failure.
    /// </returns>
    public static ConversionResult<TIn> TryConvertBack<TIn, TOut>(this IReversibleConverter<TIn, TOut> converter, TOut input)
    {
        return Wrap(() => converter.ConvertBack(input));
    }

    /// <summary>
    /// Convert back using a reversible converter if supported.
    /// </summary>
    /// <typeparam name="TOut">The converter's forward input type (and backward result type).</typeparam>
    /// <typeparam name="TIn">The converter's forward output type (and backward input type).</typeparam>
    /// <param name="converter">The converter instance.</param>
    /// <param name="value">The value to convert back.</param>
    /// <returns>The result of <c>ConvertBack</c> when supported.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="converter"/> does not implement <see cref="IReversibleConverter{TOut,TIn}"/>.
    /// </exception>
    /// <remarks>
    /// This extension provides a convenient way to call <c>ConvertBack</c> on a converter reference typed as <see cref="IConverter{TOut,TIn}"/>.
    /// If the underlying converter implements <see cref="IReversibleConverter{TOut,TIn}"/>, its <c>ConvertBack</c> will be invoked.
    /// Otherwise, an <see cref="InvalidOperationException"/> is thrown; callers can test for reversibility via <c>is IReversibleConverter&lt;TOut,TIn&gt;</c>.
    /// </remarks>
    public static TOut ConvertBack<TOut, TIn>(this IConverter<TOut, TIn> converter, TIn value)
    {
        if (converter is IReversibleConverter<TOut, TIn> reversible)
        {
            return reversible.ConvertBack(value);
        }

        // TODO: throw ConversionException
        throw new InvalidOperationException($"Converter {converter.GetType().Name} does not support ConvertBack. Implement an IReversibleConverter for the converter instead.");
    }

    /// <summary>
    /// Wraps invocation of a conversion delegate and converts any thrown exceptions into a <see cref="ConversionResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The result type produced by the delegate.</typeparam>
    /// <param name="func">The delegate to invoke.</param>
    /// <returns>
    /// A successful <see cref="ConversionResult{T}"/> containing the delegate result, or a failure result containing the thrown exception
    /// and, when applicable, extracted localization metadata from <see cref="ConversionException"/>.
    /// </returns>
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

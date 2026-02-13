using MudBlazor.Extensions;
using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Chain;

/// <summary>
/// Wraps a forward conversion delegate and provides a fluent API to compose converter pipelines.
/// </summary>
/// <typeparam name="TIn">The input type accepted by the chain.</typeparam>
/// <typeparam name="TOut">The output type produced by the chain.</typeparam>
/// <remarks>
/// Use <see cref="ConverterChain{TIn,TOut}"/> to adapt simple <see cref="IConverter{TIn,TOut}"/> implementations or
/// delegate-based converters into chainable conversion pipelines. Call one of the overloads to append further conversion steps:
/// <list type="bullet">
/// <item>
///   <description>
///     <see cref="ConverterChain{TIn,TOut}.Then{TNext}(IConverter{TOut,TNext})"/>
///   </description>
/// </item>
/// <item>
///   <description>
///     <see cref="ConverterChain{TIn,TOut}.Then{TNext}(System.Func{TOut,TNext})"/>
///   </description>
/// </item>
/// </list>
/// </remarks>
public class ConverterChain<TIn, TOut> : IConverter<TIn, TOut>
{
    /// <summary>
    /// The underlying forward conversion delegate.
    /// </summary>
    protected readonly Func<TIn, TOut> Forward;

    /// <summary>
    /// Initializes a new instance of <see cref="ConverterChain{TIn,TOut}"/> that invokes the provided forward delegate.
    /// </summary>
    /// <param name="forward">The delegate that performs the forward conversion from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.</param>
    public ConverterChain(Func<TIn, TOut> forward)
    {
        Forward = forward;
    }

    /// <summary>
    /// Converts the specified <paramref name="input"/> using the chain's forward delegate.
    /// </summary>
    /// <param name="input">The input value to convert.</param>
    /// <returns>The converted value.</returns>
    /// <remarks>
    /// This method executes the wrapped forward delegate. Exceptions thrown by the delegate will propagate to the caller.
    /// For a safe conversion that captures exceptions as a <see cref="ConversionResult{T}"/>, use <see cref="TryConvert"/>.
    /// </remarks>
    public TOut Convert(TIn input) => Forward(input);

    /// <summary>
    /// Appends another converter to this chain, producing a new chain whose result is the output of <paramref name="next"/>.
    /// </summary>
    /// <typeparam name="TNext">The output type of the appended converter (and the new chain's output type).</typeparam>
    /// <param name="next">The converter to execute after this chain.</param>
    /// <returns>A new <see cref="ConverterChain{TIn, TNext}"/> representing the composed conversion pipeline.</returns>
    public ConverterChain<TIn, TNext> Then<TNext>(IConverter<TOut, TNext> next) => new(x => next.Convert(Forward(x)));

    /// <summary>
    /// Appends a conversion delegate to this chain, producing a new chain that applies <paramref name="next"/> to this chain's output.
    /// </summary>
    /// <typeparam name="TNext">The output type of the appended delegate (and the new chain's output type).</typeparam>
    /// <param name="next">The delegate to execute after this chain.</param>
    /// <returns>A new <see cref="ConverterChain{TIn, TNext}"/> representing the composed conversion pipeline.</returns>
    public ConverterChain<TIn, TNext> Then<TNext>(Func<TOut, TNext> next) => new(x => next(Forward(x)));

    /// <summary>
    /// Attempts to convert the specified <paramref name="input"/> and returns a <see cref="ConversionResult{TOut}"/>
    /// that contains either the converted value or error information if conversion failed.
    /// </summary>
    /// <param name="input">The input value to convert.</param>
    /// <returns>
    /// A <see cref="ConversionResult{TOut}"/> describing the outcome. Use <see cref="ConversionResult{TOut}.Success"/>
    /// to determine whether the conversion succeeded.
    /// </returns>
    public ConversionResult<TOut> TryConvert(TIn input) => ConverterExtensions.TryConvert(this, input);
}

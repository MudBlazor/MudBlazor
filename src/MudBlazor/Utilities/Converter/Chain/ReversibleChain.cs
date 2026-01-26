using MudBlazor.Extensions;
using MudBlazor.Utilities.Converter.Base;

namespace MudBlazor.Utilities.Converter.Chain;

#nullable enable
/// <summary>
/// A chainable, reversible converter that supports both forward and backward conversions.
/// </summary>
/// <typeparam name="TIn">The input type for the forward conversion.</typeparam>
/// <typeparam name="TOut">The output type produced by the forward conversion (and input type for the backward conversion).</typeparam>
/// <remarks>
/// <see cref="ReversibleChain{TIn,TOut}"/> extends <see cref="ConverterChain{TIn,TOut}"/> and implements
/// <see cref="IReversibleConverter{TIn,TOut}"/> so it can be used where a
/// reversible converter is required. Use the reversible composition helpers to append further reversible steps:
/// <list type="bullet">
/// <item>
///   <description>
///     <see cref="ReversibleChain{TIn,TOut}.Then{TNext}(IReversibleConverter{TOut,TNext})"/>
///     — compose another reversible converter while preserving the ability to convert back.
///   </description>
/// </item>
/// <item>
///   <description>
///     <see cref="ReversibleChain{TIn,TOut}.Then{TNext}(System.Func{TOut,TNext},System.Func{TNext,TOut})"/>
///     — append a delegate-based reversible step by providing both forward and backward delegates; the composed result remains reversible.
///   </description>
/// </item>
/// </list>
/// Note: the base <see cref="ConverterChain{TIn,TOut}.Then{TNext}(System.Func{TOut,TNext})"/> overload is still available (inherited from <see cref="ConverterChain{TIn,TOut}"/>)
/// but appending a single delegate-based step (forward only) returns a non-reversible <see cref="ConverterChain{TIn,TNext}"/> and therefore loses backward-conversion capability.
/// </remarks>
public sealed class ReversibleChain<TIn, TOut> : ConverterChain<TIn, TOut>, IReversibleConverter<TIn, TOut>
{
    private readonly Func<TOut, TIn> _backward;

    /// <summary>
    /// Initializes a new instance of <see cref="ReversibleChain{TIn,TOut}"/>.
    /// </summary>
    /// <param name="forward">Delegate that performs the forward conversion from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.</param>
    /// <param name="backward">Delegate that performs the backward conversion from <typeparamref name="TOut"/> to <typeparamref name="TIn"/>.</param>
    public ReversibleChain(Func<TIn, TOut> forward, Func<TOut, TIn> backward)
        : base(forward)
    {
        _backward = backward;
    }

    /// <summary>
    /// Performs the backward conversion (from <typeparamref name="TOut"/> to <typeparamref name="TIn"/>).
    /// </summary>
    /// <param name="input">The value to convert back.</param>
    /// <returns>The result of the backward conversion.</returns>
    /// <remarks>
    /// Exceptions thrown by the provided backward delegate propagate to the caller. For a non-throwing alternative
    /// that captures errors in a <see cref="ConversionResult{T}"/>, use <see cref="TryConvertBack"/>.
    /// </remarks>
    public TIn ConvertBack(TOut input) => _backward(input);

    /// <summary>
    /// Creates a new <see cref="ReversibleChain{TOut,TIn}"/> representing this chain reversed
    /// (forward and backward delegates swapped).
    /// </summary>
    /// <returns>A new reversible chain that converts in the opposite direction.</returns>
    public ReversibleChain<TOut, TIn> Reverse() => new(_backward, Forward);

    /// <summary>
    /// Appends another reversible converter to this chain and returns a new reversible chain that represents
    /// the composed forward and backward conversions.
    /// </summary>
    /// <typeparam name="TNext">The output type of the appended reversible converter.</typeparam>
    /// <param name="next">The reversible converter to execute after this chain.</param>
    /// <returns>
    /// A new <see cref="ReversibleChain{TIn, TNext}"/> whose forward conversion applies this chain's forward delegate
    /// followed by <paramref name="next"/>. Its backward conversion applies <paramref name="next"/>'s backward conversion
    /// followed by this chain's backward conversion.
    /// </returns>
    public ReversibleChain<TIn, TNext> Then<TNext>(IReversibleConverter<TOut, TNext> next)
        => new(value => next.Convert(Forward(value)), value => _backward(next.ConvertBack(value)));

    /// <summary>
    /// Appends a delegate-based conversion step that includes both forward and backward delegates and returns a new reversible chain.
    /// </summary>
    /// <typeparam name="TNext">The output type produced by the appended delegates.</typeparam>
    /// <param name="forward">Delegate that maps this chain's <typeparamref name="TOut"/> to <typeparamref name="TNext"/>.</param>
    /// <param name="backward">Delegate that maps <typeparamref name="TNext"/> back to <typeparamref name="TOut"/>.</param>
    /// <returns>
    /// A new <see cref="ReversibleChain{TIn, TNext}"/> that preserves reversibility by using the supplied <paramref name="forward"/>
    /// and <paramref name="backward"/> delegates as the next step in the pipeline.
    /// </returns>
    /// <remarks>
    /// Use this overload when you want to append small inline reversible steps without creating a concrete <see cref="IReversibleConverter{TOut,TNext}"/>.
    /// Exceptions thrown by the provided delegates will propagate to the caller.
    /// </remarks>
    public ReversibleChain<TIn, TNext> Then<TNext>(Func<TOut, TNext> forward, Func<TNext, TOut> backward)
        => new(value => forward(Forward(value)), value => _backward(backward(value)));

    /// <summary>
    /// Attempts to perform the backward conversion and returns a <see cref="ConversionResult{TIn}"/> describing the outcome.
    /// </summary>
    /// <param name="input">The value to convert back.</param>
    /// <returns>
    /// A <see cref="ConversionResult{TIn}"/> that contains either the converted value (when <see cref="ConversionResult{TIn}.Success"/> is <c>true</c>)
    /// or error information when the conversion failed.
    /// </returns>
    public ConversionResult<TIn> TryConvertBack(TOut input) => ConverterExtensions.TryConvertBack(this, input);
}

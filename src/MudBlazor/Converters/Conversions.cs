using MudBlazor.Utilities.Converter.Chain;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory helpers for creating converter chains from existing converters or delegate functions.
/// </summary>
/// <remarks>
/// The returned <see cref="ConverterChain{TIn,TOut}"/> and <see cref="ReversibleChain{TIn,TOut}"/> objects
/// wrap the provided converter or delegates and provide a fluent API for composing conversions
/// (for example via <c>Then</c>). Use these helpers to adapt simple <see cref="IConverter{TIn,TOut}"/>
/// or <see cref="IReversibleConverter{TIn,TOut}"/> implementations into chainable conversion pipelines.
/// </remarks>
public static class Conversions
{
    /// <summary>
    /// Wraps an <see cref="IConverter{TIn,TOut}"/> into a <see cref="ConverterChain{TIn,TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn">Input type of the converter.</typeparam>
    /// <typeparam name="TOut">Output type of the converter.</typeparam>
    /// <param name="converter">The converter instance to wrap.</param>
    /// <returns>A <see cref="ConverterChain{TIn,TOut}"/> that invokes <paramref name="converter"/>.</returns>
    public static ConverterChain<TIn, TOut> From<TIn, TOut>(IConverter<TIn, TOut> converter) => new(converter.Convert);

    /// <summary>
    /// Wraps an <see cref="IReversibleConverter{TIn,TOut}"/> into a <see cref="ReversibleChain{TIn,TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn">Input type of the forward conversion.</typeparam>
    /// <typeparam name="TOut">Output type of the forward conversion (and input of the backward conversion).</typeparam>
    /// <param name="converter">The reversible converter instance to wrap.</param>
    /// <returns>
    /// A <see cref="ReversibleChain{TIn,TOut}"/> that invokes the forward and backward conversion delegates
    /// from <paramref name="converter"/>.
    /// </returns>
    public static ReversibleChain<TIn, TOut> From<TIn, TOut>(IReversibleConverter<TIn, TOut> converter) =>
        new(converter.Convert, converter.ConvertBack);

    /// <summary>
    /// Creates a <see cref="ConverterChain{TIn,TOut}"/> from a forward conversion delegate.
    /// </summary>
    /// <typeparam name="TIn">Input type of the conversion delegate.</typeparam>
    /// <typeparam name="TOut">Output type of the conversion delegate.</typeparam>
    /// <param name="forward">The forward conversion delegate.</param>
    /// <returns>A <see cref="ConverterChain{TIn,TOut}"/> that invokes <paramref name="forward"/>.</returns>
    public static ConverterChain<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> forward) => new(forward);

    /// <summary>
    /// Creates a <see cref="ReversibleChain{TIn,TOut}"/> from forward and backward conversion delegates.
    /// </summary>
    /// <typeparam name="TIn">Input type for the forward conversion and the output type for the backward conversion.</typeparam>
    /// <typeparam name="TOut">Output type for the forward conversion and the input type for the backward conversion.</typeparam>
    /// <param name="forward">The forward conversion delegate.</param>
    /// <param name="backward">The backward conversion delegate.</param>
    /// <returns>
    /// A <see cref="ReversibleChain{TIn,TOut}"/> that invokes <paramref name="forward"/> for conversion and
    /// <paramref name="backward"/> for the reverse conversion.
    /// </returns>
    public static ReversibleChain<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> forward, Func<TOut, TIn> backward) =>
        new(forward, backward);
}

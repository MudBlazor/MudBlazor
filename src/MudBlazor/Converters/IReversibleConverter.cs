namespace MudBlazor;

#nullable enable
/// <summary>
/// Extends <see cref="IConverter{TIn,TOut}"/> with the ability to convert values back from
/// <typeparamref name="TOut"/> to <typeparamref name="TIn"/>.
/// </summary>
/// <typeparam name="TIn">The input type for the forward conversion and the result type for the reverse conversion.</typeparam>
/// <typeparam name="TOut">The output type for the forward conversion and the input type for the reverse conversion.</typeparam>
/// <remarks>
/// Implementations should provide a best-effort inverse for <see cref="IConverter{TIn,TOut}.Convert"/>.
/// Reversibility is not guaranteed for all converters (for example when information is lost during the forward conversion);
/// implementations may throw an <see cref="InvalidOperationException"/> if the reverse conversion cannot be performed.
/// Prefer pure, side-effect-free implementations consistent with <see cref="IConverter{TIn,TOut}"/> semantics.
/// </remarks>
public interface IReversibleConverter<TIn, TOut> : IConverter<TIn, TOut>
{
    /// <summary>
    /// Converts the specified <paramref name="input"/> value from <typeparamref name="TOut"/> back to <typeparamref name="TIn"/>.
    /// </summary>
    /// <param name="input">The value to convert back.</param>
    /// <returns>The converted value as <typeparamref name="TIn"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the conversion cannot be performed or is not supported for the provided value.</exception>
    TIn ConvertBack(TOut input);
}

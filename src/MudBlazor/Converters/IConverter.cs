namespace MudBlazor;

/// <summary>
/// Converts values from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.
/// </summary>
/// <typeparam name="TIn">The input type. Declared contravariant (<c>in</c>) so implementations can accept base types.</typeparam>
/// <typeparam name="TOut">The output type. Declared covariant (<c>out</c>) so implementations can return derived types.</typeparam>
/// <remarks>
/// Implementations should perform a synchronous conversion. Converters are typically used by UI components and utilities;
/// prefer pure, side-effect-free implementations.
/// </remarks>
public interface IConverter<in TIn, out TOut>
{
    /// <summary>
    /// Converts the specified <paramref name="input"/> value to the target type.
    /// </summary>
    /// <param name="input">The value to convert.</param>
    /// <returns>The converted value as <typeparamref name="TOut"/>.</returns>
    TOut Convert(TIn input);
}

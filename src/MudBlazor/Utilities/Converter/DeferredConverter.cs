// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Converter;

#nullable enable
public class DeferredConverter<TIn, TOut> : IReversibleConverter<TIn, TOut>
{
    private Func<TIn, TOut>? _forward;
    private Func<TOut, TIn>? _backward;

    public void SetForward(Func<TIn, TOut> forward) => _forward = forward;

    public void SetBackward(Func<TOut, TIn> backward) => _backward = backward;

    public void Set(Func<TIn, TOut> forward, Func<TOut, TIn> backward)
    {
        _forward = forward;
        _backward = backward;
    }

    /// <summary>
    /// Converts the specified <paramref name="input"/> value to the target type.
    /// </summary>
    /// <param name="input">The value to convert.</param>
    /// <returns>The converted value as <typeparamref name="TOut"/>.</returns>
    public TOut Convert(TIn input) => _forward is null
        ? throw new InvalidOperationException("Conversion not initialized.")
        : _forward(input);

    /// <inheritdoc />
    public TIn ConvertBack(TOut input) => _backward is null ?
        throw new InvalidOperationException("Reverse conversion not initialized.")
        : _backward(input);
}

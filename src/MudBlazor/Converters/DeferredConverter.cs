// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// A simple reversible converter whose forward and backward delegates can be supplied (deferred) after construction.
/// </summary>
/// <remarks>
/// This type is useful to break initialization cycles or to construct converter graphs where delegates are wired
/// at a later time. The converter will throw <see cref="InvalidOperationException"/> if a conversion method is invoked
/// before the corresponding delegate has been set.
/// </remarks>
/// <typeparam name="TIn">The input type for the forward conversion.</typeparam>
/// <typeparam name="TOut">The output type for the forward conversion (and the input type for the backward conversion).</typeparam>
public class DeferredConverter<TIn, TOut> : IReversibleConverter<TIn, TOut>
{
    private Func<TIn, TOut>? _forward;
    private Func<TOut, TIn>? _backward;

    /// <summary>
    /// Sets the forward conversion delegate.
    /// </summary>
    /// <param name="forward">A delegate that converts a <typeparamref name="TIn"/> value to <typeparamref name="TOut"/>.</param>
    /// <remarks>
    /// Calling this method replaces any previously registered forward delegate.
    /// </remarks>
    public void SetForward(Func<TIn, TOut> forward) => _forward = forward;

    /// <summary>
    /// Sets the backward conversion delegate.
    /// </summary>
    /// <param name="backward">A delegate that converts a <typeparamref name="TOut"/> value back to <typeparamref name="TIn"/>.</param>
    /// <remarks>
    /// Calling this method replaces any previously registered backward delegate.
    /// </remarks>
    public void SetBackward(Func<TOut, TIn> backward) => _backward = backward;

    /// <summary>
    /// Sets both forward and backward conversion delegates in a single call.
    /// </summary>
    /// <param name="forward">A delegate that converts a <typeparamref name="TIn"/> value to <typeparamref name="TOut"/>.</param>
    /// <param name="backward">A delegate that converts a <typeparamref name="TOut"/> value back to <typeparamref name="TIn"/>.</param>
    public void Set(Func<TIn, TOut> forward, Func<TOut, TIn> backward)
    {
        _forward = forward;
        _backward = backward;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when the forward conversion delegate has not been set.</exception>
    public TOut Convert(TIn input) => _forward is null
        ? throw new InvalidOperationException("Conversion not initialized.")
        : _forward(input);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when the backward conversion delegate has not been set.</exception>
    public TIn ConvertBack(TOut input) => _backward is null ?
        throw new InvalidOperationException("Reverse conversion not initialized.")
        : _backward(input);
}

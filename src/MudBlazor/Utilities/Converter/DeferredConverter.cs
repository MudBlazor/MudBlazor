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

    public TOut Convert(TIn input)
    {
        return _forward is null ? throw new InvalidOperationException("Conversion not initialized.") : _forward(input);
    }

    public TIn ConvertBack(TOut input)
    {
        return _backward is null ? throw new InvalidOperationException("Reverse conversion not initialized.") : _backward(input);
    }
}

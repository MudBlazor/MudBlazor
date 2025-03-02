// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable

internal sealed class OverlayPointerDownListener : IOverlayPointerDownListener
{
    private readonly string _elementId;
    private readonly IJSRuntime _jsRuntime;
    private bool _disposed;
    private DotNetObjectReference<OverlayPointerDownListener>? _dotNetRef;

    public event EventHandler? OnPointerDown;

    [DynamicDependency(nameof(RaiseOnPointerDown))]
    public OverlayPointerDownListener(string elementId, IJSRuntime jsRuntime)
    {
        _elementId = elementId;
        _jsRuntime = jsRuntime;
    }

    public ValueTask<bool> StartAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _dotNetRef ??= DotNetObjectReference.Create(this);
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudOverlay.listenForPointerDown", _elementId, _dotNetRef);
    }

    public ValueTask<bool> StopAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudOverlay.cancelListener", _elementId);
    }

    [JSInvokable]
    public void RaiseOnPointerDown() => OnPointerDown?.Invoke(this, EventArgs.Empty);

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudOverlay.cancelListener", _elementId);
            _dotNetRef?.Dispose();
        }
    }
}

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
    private bool _started;
    private bool _disposed;
    private DotNetObjectReference<OverlayPointerDownListener>? _dotNetRef;

    public bool IsStarted => _started;

    public event EventHandler? OnPointerDown;

    [DynamicDependency(nameof(RaiseOnPointerDown))]
    public OverlayPointerDownListener(string elementId, IJSRuntime jsRuntime)
    {
        _elementId = elementId;
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<bool> StartAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_started)
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);
            _started = await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudOverlay.listenForPointerDown", _elementId, _dotNetRef);
        }

        return _started;
    }

    public async ValueTask<bool> StopAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_started)
        {
            _started = !await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudOverlay.cancelListener", _elementId);
        }

        return !_started;
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

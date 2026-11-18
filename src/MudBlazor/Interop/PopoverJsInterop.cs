// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.Interop;

internal class PopoverJsInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _providerId;

    public PopoverJsInterop(IJSRuntime jsRuntime, string providerId)
    {
        _jsRuntime = jsRuntime;
        _providerId = providerId;
    }

    public ValueTask<bool> Initialize(string containerClass, int flipMargin, int overflowPadding, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.initialize", cancellationToken, _providerId, containerClass, flipMargin, overflowPadding);
    }

    public ValueTask<bool> Connect(Guid id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.connect", cancellationToken, _providerId, id);
    }

    public ValueTask<bool> Disconnect(Guid id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.disconnect", cancellationToken, _providerId, id);
    }

    public ValueTask<(bool success, int value)> CountProviders(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeAsyncWithErrorHandling<int>("mudpopoverHelper.countProviders", cancellationToken, _providerId);
    }

    public ValueTask DisposeAsync(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudPopover.dispose", cancellationToken, _providerId);
    }
}

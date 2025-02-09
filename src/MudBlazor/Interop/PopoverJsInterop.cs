// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Interop;

#nullable enable
internal class PopoverJsInterop
{
    private readonly IJSRuntime _jsRuntime;

    public PopoverJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask<bool> Initialize(string containerClass, int flipMargin, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.initialize", cancellationToken, containerClass, flipMargin);
    }

    public ValueTask<bool> Connect(Guid id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.connect", cancellationToken, id);
    }

    public ValueTask<bool> Disconnect(Guid id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.disconnect", cancellationToken, id);
    }

    public ValueTask<(bool success, int value)> CountProviders(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeAsyncWithErrorHandling<int>("mudpopoverHelper.countProviders", cancellationToken);
    }

    public ValueTask Dispose(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudPopover.dispose", cancellationToken);
    }
}

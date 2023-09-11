// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

    public ValueTask<bool> Initialize(string containerClass, int flipMargin)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.initialize", containerClass, flipMargin);
    }

    public ValueTask<bool> Connect(Guid id)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.connect", id);
    }

    public ValueTask<bool> Disconnect(Guid id)
    {
       return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.disconnect", id);
    }

    public ValueTask<(bool success, int value)> CountProviders()
    {
        return _jsRuntime.InvokeAsyncWithErrorHandling<int>("mudpopoverHelper.countProviders");
    }

    public ValueTask Dispose()
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudPopover.dispose");
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace MudBlazor.Interop;

#nullable enable
internal class ResizeListenerInterop
{
    private readonly IJSRuntime _jsRuntime;

    public ResizeListenerInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<bool> MatchMedia(string mediaQuery)
    {
        var (success, value) = await _jsRuntime.InvokeAsyncWithErrorHandling(false, "mudResizeListener.matchMedia", mediaQuery);

        return value;
    }

    public ValueTask<bool> ListenForResize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(DotNetObjectReference<T> dotNetObjectReference, ResizeOptions options, Guid javaScriptListerId) where T : class
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.listenForResize", dotNetObjectReference, options, javaScriptListerId);
    }

    public ValueTask<bool> CancelListener(Guid javaScriptListerId)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.cancelListener", javaScriptListerId);
    }

    public ValueTask<bool> CancelListeners(Guid[] jsListenerIds)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.cancelListeners", jsListenerIds);
    }

    public ValueTask Dispose()
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudResizeListenerFactory.dispose");
    }

    public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
    {
        var (success, value) = await _jsRuntime.InvokeAsyncWithErrorHandling(new BrowserWindowSize(), "mudResizeListener.getBrowserWindowSize");

        return value;
    }
}

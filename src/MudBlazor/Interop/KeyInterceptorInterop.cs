// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace MudBlazor.Interop;

#nullable enable
internal class KeyInterceptorInterop
{
    private readonly IJSRuntime _jsRuntime;

    public KeyInterceptorInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask<bool> Connect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(DotNetObjectReference<T> dotNetObjectReference, string elementId, KeyInterceptorOptions options) where T : class
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudKeyInterceptor.connect", dotNetObjectReference, elementId, options);
    }

    public ValueTask Disconnect(string elementId)
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudKeyInterceptor.disconnect", elementId);
    }

    public ValueTask UpdateKey(string elementId, KeyOptions option)
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudKeyInterceptor.updatekey", elementId, option);
    }
}

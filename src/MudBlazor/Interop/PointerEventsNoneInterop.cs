// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor.Interop;

#nullable enable

internal class PointerEventsNoneInterop
{
    private readonly IJSRuntime _jsRuntime;

    public PointerEventsNoneInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask<bool> ListenForPointerEventsAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        DotNetObjectReference<T> dotNetObjectReference,
        string elementId,
        PointerEventsNoneOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPointerEventsNone.listenForPointerEvents", cancellationToken, dotNetObjectReference, elementId, options);
    }

    public ValueTask<bool> CancelListenerAsync(string elementId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPointerEventsNone.cancelListener", cancellationToken, elementId);
    }

    public ValueTask DisposeAsync(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsyncIgnoreErrors("mudPointerEventsNone.dispose", cancellationToken);
    }
}

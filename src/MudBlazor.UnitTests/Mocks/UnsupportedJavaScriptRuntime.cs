// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.UnitTests.Mocks;

// DO NOT rename the class as it's essential for the test to work.
#nullable enable
internal class UnsupportedJavaScriptRuntime : IJSRuntime
{
    private const string Message = "JavaScript interop calls cannot be issued during server-side static rendering, because the page has not yet loaded in the browser. Statically-rendered components must wrap any JavaScript interop calls in conditional logic to ensure those interop calls are not attempted during static rendering.";

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => throw new InvalidOperationException(Message);

    ValueTask<TValue> IJSRuntime.InvokeAsync<TValue>(string identifier, object?[]? args)
        => throw new InvalidOperationException(Message);
}

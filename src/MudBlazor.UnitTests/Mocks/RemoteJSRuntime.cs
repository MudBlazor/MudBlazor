// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.UnitTests.Mocks;

// DO NOT rename the class as it's essential for the test to work.
#nullable enable
internal class RemoteJSRuntime : IJSRuntime
{
    private const string Message =
        "JavaScript interop calls cannot be issued at this time. This is because the component is being " +
        "statically rendered. When prerendering is enabled, JavaScript interop calls can only be performed " +
        "during the OnAfterRenderAsync lifecycle method.";

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => throw new InvalidOperationException(Message);

    ValueTask<TValue> IJSRuntime.InvokeAsync<TValue>(string identifier, object?[]? args)
        => throw new InvalidOperationException(Message);
}

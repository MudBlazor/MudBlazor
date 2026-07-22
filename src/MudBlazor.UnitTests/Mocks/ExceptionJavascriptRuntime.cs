// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.UnitTests.Mocks;

public class ExceptionJavascriptRuntime : IJSRuntime
{
    public Func<Exception> ExceptionFactory { get; set; } = () => new JSException("An error occurred when invoking JavaScript function.");

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
    {
        throw ExceptionFactory();
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
    {
        throw ExceptionFactory();
    }
}

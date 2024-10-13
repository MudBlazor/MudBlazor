// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor;

/// <summary>
/// Provides JavaScript API services for various browser operations.
/// </summary>
internal class JsApiService : IJsApiService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsApiService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime to use for invoking JavaScript functions.</param>
    public JsApiService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc />
    public ValueTask CopyToClipboardAsync(string text)
    {
        return _jsRuntime.InvokeVoidAsync("mudWindow.copyToClipboard", text);
    }

    /// <inheritdoc />
    public ValueTask Open(string link, string target)
    {
        if (target == "_blank")
        {
            return OpenInNewTabAsync(link);
        }

        return _jsRuntime.InvokeVoidAsync("open", link, target);
    }

    /// <inheritdoc />
    public ValueTask OpenInNewTabAsync(string url)
    {
        return _jsRuntime.InvokeVoidAsync("mudWindow.open", url, "_blank");
    }

    /// <inheritdoc />
    public ValueTask UpdateStyleProperty(string elementId, string propertyName, object value)
    {
        return _jsRuntime.InvokeVoidAsync("mudWindow.updateStyleProperty", elementId, propertyName, value);
    }
}

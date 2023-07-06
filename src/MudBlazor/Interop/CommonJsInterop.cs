// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Interop;

#nullable enable
internal class CommonJsInterop
{
    private readonly IJSRuntime _jsRuntime;

    public CommonJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask CopyToClipboard(string text) => _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);

    public ValueTask Open(string link, string target) => _jsRuntime.InvokeVoidAsync("window.open", link, target);

    public ValueTask Open(string url) => _jsRuntime.InvokeVoidAsync("window.open", url, "_blank");
}

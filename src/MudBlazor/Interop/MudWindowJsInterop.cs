// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Interop;

#nullable enable
internal class MudWindowJsInterop
{
    private readonly IJSRuntime _jsRuntime;

    public MudWindowJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask CopyToClipboard(string text) => _jsRuntime.InvokeVoidAsync("mudWindow.copyToClipboard", text);

    public ValueTask Open(string link, string target) => _jsRuntime.InvokeVoidAsync("open", link, target);

    public ValueTask Open(string url) => _jsRuntime.InvokeVoidAsync("mudWindow.open", url, "_blank");
}

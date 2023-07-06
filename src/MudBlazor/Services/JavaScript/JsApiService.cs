using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a concrete implementation of the <see cref="IJsApiService"/> interface.
/// </summary>
public class JsApiService : IJsApiService
{
    private readonly CommonJsInterop _commonJsInterop;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsApiService"/> class with the specified JS runtime.
    /// </summary>
    /// <param name="jsRuntime">The JS runtime used for interop with JavaScript.</param>
    public JsApiService(IJSRuntime jsRuntime)
    {
        _commonJsInterop = new CommonJsInterop(jsRuntime);
    }

    /// <inheritdoc />
    public ValueTask CopyToClipboardAsync(string text) => _commonJsInterop.CopyToClipboard(text);

    /// <inheritdoc />
    public ValueTask Open(string url, string target)
    {
        if (target == "_blank")
        {
            return OpenInNewTabAsync(url);
        }

        return _commonJsInterop.Open(url, target);
    }

    /// <inheritdoc />
    public ValueTask OpenInNewTabAsync(string url) => _commonJsInterop.Open(url);
}

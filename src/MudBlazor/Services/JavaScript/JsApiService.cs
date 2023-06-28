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
    private readonly MudWindowJsInterop _mudWindowJsInterop;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsApiService"/> class with the specified JS runtime.
    /// </summary>
    /// <param name="jsRuntime">The JS runtime used for interop with JavaScript.</param>
    public JsApiService(IJSRuntime jsRuntime)
    {
        _mudWindowJsInterop = new MudWindowJsInterop(jsRuntime);
    }

    /// <inheritdoc />
    public ValueTask CopyToClipboardAsync(string text) => _mudWindowJsInterop.CopyToClipboard(text);

    /// <inheritdoc />
    public ValueTask Open(string link, string target)
    {
        if (target == "_blank")
        {
            return OpenInNewTabAsync(link);
        }

        return _mudWindowJsInterop.Open(link, target);
    }

    /// <inheritdoc />
    public ValueTask OpenInNewTabAsync(string url) => _mudWindowJsInterop.Open(url);
}

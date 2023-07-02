using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace MudBlazor
{
    [Obsolete("This will be removed in v7.")]
    public interface IBrowserWindowSizeProvider
    {
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
    }

    /// <summary>
    /// This provider calls the JS method resizeListener.getBrowserWindowSize to get the browser window size
    /// </summary>
    [Obsolete("This will be removed in v7.")]
    public class BrowserWindowSizeProvider : IBrowserWindowSizeProvider
    {
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        public BrowserWindowSizeProvider(IJSRuntime jsRuntime)
        {
            this._jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Get the current BrowserWindowSize, this includes the Height and Width of the document.
        /// </summary>
        /// <returns></returns>
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize() =>
_jsRuntime.InvokeAsync<BrowserWindowSize>($"mudResizeListener.getBrowserWindowSize");
    }
}

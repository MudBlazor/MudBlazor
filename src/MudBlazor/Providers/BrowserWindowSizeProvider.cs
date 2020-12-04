using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Threading.Tasks;

namespace MudBlazor.Providers
{
    public interface IBrowserWindowSizeProvider
    {
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
    }

    /// <summary>
    /// This provider calls the JS method resizeListener.getBrowserWindowSize to get the browser window size
    /// </summary>
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
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize() =>
            await _jsRuntime.InvokeAsync<BrowserWindowSize>($"resizeListener.getBrowserWindowSize");
    }
}

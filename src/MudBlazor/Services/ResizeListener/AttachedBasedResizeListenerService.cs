using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    /// <summary>
    /// This service listens to browser resize events and allows you to react to a changing window size in Blazor
    /// </summary>
    public class AttachedBasedResizeListenerService : IAttachedBasedResizeListenerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        public AttachedBasedResizeListenerService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null)
        {
            this._options = options?.Value ?? new ResizeOptions();
            this._jsRuntime = jsRuntime;
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }

        private Guid _listenerJsId;
        private readonly IJSRuntime _jsRuntime;
        private readonly IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private readonly ResizeOptions _options;
        private DotNetObjectReference<AttachedBasedResizeListenerService> _dotNetRef;
        private Action<BrowserWindowSize> _callback;

        private bool IsAttached() => _dotNetRef != null;

         /// <summary>
        /// Get the current BrowserWindowSize, this includes the Height and Width of the document.
        /// </summary>
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize() =>
            _browserWindowSizeProvider.GetBrowserWindowSize();

        /// <summary>
        /// Invoked by jsInterop, use the OnResized event handler to subscribe.
        /// </summary>
        /// <param name="browserWindowSize"></param>
        /// <param name="_"></param>
        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint _)
        {
            _callback(browserWindowSize);
        }

        public async Task Attach(Action<BrowserWindowSize> callback) => await Attach(callback, _options);

        public async Task Attach(Action<BrowserWindowSize> callback, ResizeOptions options)
        {
            if (IsAttached() == false)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _dotNetRef = DotNetObjectReference.Create(this);
                _listenerJsId = Guid.NewGuid();
                try
                {
                    await _jsRuntime.InvokeVoidAsync($"mudResizeListenerFactory.listenForResize", _dotNetRef, options, _listenerJsId);
                }
                catch (TaskCanceledException)
                {
                    // no worries here
                }
            }
        }

        public async Task Detach()
        {
            if (IsAttached() == true)
            {

                try
                {
                    await _jsRuntime.InvokeVoidAsync($"mudResizeListenerFactory.cancelListener", _listenerJsId);
                    _dotNetRef.Dispose();
                }
                catch (TaskCanceledException)
                {
                    // no worries here
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Detach();
        }
    }

    public interface IAttachedBasedResizeListenerService : IAsyncDisposable
    {
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
        Task Attach(Action<BrowserWindowSize> callback);
        Task Attach(Action<BrowserWindowSize> callback, ResizeOptions options);
        Task Detach();
    }
}

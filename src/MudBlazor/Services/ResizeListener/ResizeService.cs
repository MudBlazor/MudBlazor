using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    /// <summary>
    /// This service listens to browser resize events and allows you to react to a changing window size in Blazor
    /// </summary>
    public class ResizeService :
        ResizeBasedService<ResizeService, ResizeServiceSubscriptionInfo, BrowserWindowSize, ResizeOptions>,
        IResizeService
    {
        private readonly IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private readonly ResizeOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        [DynamicDependency(nameof(RaiseOnResized))]
        public ResizeService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null) :
            base(jsRuntime)
        {
            this._options = options?.Value ?? new ResizeOptions();
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }

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
        /// <param name="optionId"></param>
        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint _, Guid optionId)
        {
            if (Listeners.ContainsKey(optionId) == false) { return; }

            var listenerInfo = Listeners[optionId];
            listenerInfo.InvokeCallbacks(browserWindowSize);
        }

        /// <summary>
        /// Subscribe to size changes of the browser window. Default ResizeOptions will be used
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <returns>The subscription id. This id is needed for unscribe </returns>
        public async Task<Guid> Subscribe(Action<BrowserWindowSize> callback) => await Subscribe(callback, _options);

        /// <summary>
        /// Subscribe to size changes of the browser window using the provided options
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <param name="options"></param>
        /// <returns>The subscription id. This id is needed for unscribe</returns>
        public async Task<Guid> Subscribe(Action<BrowserWindowSize> callback, ResizeOptions options)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            options ??= _options;

            if (DotNetRef == null)
            {
                DotNetRef = DotNetObjectReference.Create(this);
            }

            var existingOptionId = Listeners.Where(x => x.Value.Option == options).Select(x => x.Key).FirstOrDefault();

            if (existingOptionId == default)
            {
                var subscriptionInfo = new ResizeServiceSubscriptionInfo(options);
                var subscriptionId = subscriptionInfo.AddSubscription(callback);
                var listenerId = Guid.NewGuid();

                Listeners.Add(listenerId, subscriptionInfo);

                await JsRuntime.InvokeVoidAsyncWithErrorHandling($"mudResizeListenerFactory.listenForResize", DotNetRef, options, listenerId);

                return subscriptionId;
            }
            else
            {
                var entry = Listeners[existingOptionId];
                var subscriptionId = entry.AddSubscription(callback);

                return subscriptionId;
            }
        }
    }

    public interface IResizeService : IAsyncDisposable
    {
        /// <summary>
        /// Get the current size of the window
        /// </summary>
        /// <returns>A task representing the current browser size</returns>
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();

        /// <summary>
        /// Subscribe to size changes of the browser window. Default ResizeOptions will be used
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <returns>The subscription id. This id is needed for unscribe </returns>
        Task<Guid> Subscribe(Action<BrowserWindowSize> callback);

        /// <summary>
        /// Subscribe to size changes of the browser window using the provided options
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <param name="options">The options used to subscribe to changes</param>
        /// <returns>The subscription id. This id is needed for unscribe</returns>
        Task<Guid> Subscribe(Action<BrowserWindowSize> callback, ResizeOptions options);

        /// <summary>
        /// Used for cancel the subscription to the resize event.
        /// </summary>
        /// <param name="subscriptionId">The subscription id (return of subscribe) to cancel</param>
        /// <returns>True if the subscription could be cancel, false otherwise</returns>
        Task<bool> Unsubscribe(Guid subscriptionId);
    }
}

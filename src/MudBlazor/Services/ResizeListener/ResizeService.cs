using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
#nullable enable
    /// <summary>
    /// This service listens to browser resize events and allows you to react to a changing window size in Blazor
    /// </summary>
    [Obsolete($"Use {nameof(IBrowserViewportService)} instead. This will be removed in v7.")]
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
        public ResizeService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions>? options = null) :
            base(jsRuntime)
        {
            _options = options?.Value ?? new ResizeOptions();
            _browserWindowSizeProvider = browserWindowSizeProvider;
        }

        /// <inheritdoc />
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
            if (Listeners.TryGetValue(optionId, out var listenerInfo))
            {
                listenerInfo.InvokeCallbacks(browserWindowSize);
            }
        }

        /// <inheritdoc />
        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<Guid> Subscribe(Action<BrowserWindowSize> callback) => SubscribeAsync(callback, _options);

        /// <inheritdoc />
        public Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback) => SubscribeAsync(callback, _options);

        /// <inheritdoc />
        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<Guid> Subscribe(Action<BrowserWindowSize> callback, ResizeOptions? options) => SubscribeAsync(callback, options);

        /// <inheritdoc />
        public async Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback, ResizeOptions? options)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            options ??= _options;

            DotNetRef ??= DotNetObjectReference.Create(this);

            //We capture both key and value, because someone might unsubscribe at meantime
            //This way we do not need to check if key exist and do look up the dictionary again later
            var existingOptionKeyValuePair = Listeners.FirstOrDefault(x => x.Value.Option == options);

            //better way than existingOptionKeyValuePair.Equals(default) to avoid boxing
            //we use ValueTuple to compare if KeyValuePair struct is default
            if ((existingOptionKeyValuePair.Key, existingOptionKeyValuePair.Value) == default)
            {
                return await CreateSubscriptionAsync(callback, options);
            }

            var subscriptionId = existingOptionKeyValuePair.Value.AddSubscription(callback);
            return subscriptionId;
        }

        private async Task<Guid> CreateSubscriptionAsync(Action<BrowserWindowSize> callback, ResizeOptions options)
        {
            var subscriptionInfo = new ResizeServiceSubscriptionInfo(options);
            var subscriptionId = subscriptionInfo.AddSubscription(callback);
            var listenerId = Guid.NewGuid();

            Listeners.TryAdd(listenerId, subscriptionInfo);

            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.listenForResize", DotNetRef, options, listenerId);

            return subscriptionId;
        }
    }

    [Obsolete($"Use {nameof(IBrowserViewportService)} instead. This will be removed in v7.")]
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
        /// <param name="callback">The method (callback) that is invoke as soon as the size of the window has changed</param>
        /// <returns>The subscription id. This id is needed for unsubscribe</returns>
        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        Task<Guid> Subscribe(Action<BrowserWindowSize> callback);

        /// <summary>
        /// Subscribe to size changes of the browser window. Default ResizeOptions will be used
        /// </summary>
        /// <param name="callback">The method (callback) that is invoke as soon as the size of the window has changed</param>
        /// <returns>The subscription id. This id is needed for unsubscribe</returns>
        Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback);

        /// <summary>
        /// Subscribe to size changes of the browser window using the provided options
        /// </summary>
        /// <param name="callback">The method (callback) that is invoke as soon as the size of the window has changed</param>
        /// <param name="options">The options used to subscribe to changes</param>
        /// <returns>The subscription id. This id is needed for unsubscribe</returns>
        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        Task<Guid> Subscribe(Action<BrowserWindowSize> callback, ResizeOptions? options);

        /// <summary>
        /// Subscribe to size changes of the browser window using the provided options
        /// </summary>
        /// <param name="callback">The method (callback) that is invoke as soon as the size of the window has changed</param>
        /// <param name="options">The options used to subscribe to changes</param>
        /// <returns>The subscription id. This id is needed for unsubscribe</returns>
        Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback, ResizeOptions? options);

        /// <summary>
        /// Used for cancel the subscription to the resize event.
        /// </summary>
        /// <param name="subscriptionId">The subscription id (return of subscribe) to cancel</param>
        /// <returns>True if the subscription could be cancel, false otherwise</returns>
        [Obsolete($"Use {nameof(UnsubscribeAsync)} instead. This will be removed in v7.")]
        Task<bool> Unsubscribe(Guid subscriptionId);

        /// <summary>
        /// Used for cancel the subscription to the resize event.
        /// </summary>
        /// <param name="subscriptionId">The subscription id (return of subscribe) to cancel</param>
        /// <returns>True if the subscription could be cancel, false otherwise</returns>
        Task<bool> UnsubscribeAsync(Guid subscriptionId);
    }
}

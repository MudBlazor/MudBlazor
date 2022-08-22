// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{

    public class BreakpointService :
        ResizeBasedService<BreakpointService, BreakpointServiceSubscriptionInfo, Breakpoint, ResizeOptions>,
        IBreakpointService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ResizeOptions _options;
        private IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private BrowserWindowSize _windowSize;
        private Breakpoint _breakpoint = Breakpoint.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        [DynamicDependency(nameof(RaiseOnResized))]
        public BreakpointService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null)
            : base(jsRuntime)
        {
            this._options = options?.Value ?? new ResizeOptions();
            this._jsRuntime = jsRuntime;
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }

        /// <summary>
        /// Invoked by jsInterop, use the OnResized event handler to subscribe.
        /// </summary>
        /// <param name="browserWindowSize"></param>
        /// <param name="breakpoint"></param>
        /// <param name="optionId"></param>
        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint breakpoint, Guid optionId)
        {
            _windowSize = browserWindowSize;
            _breakpoint = breakpoint;

            if (Listeners.ContainsKey(optionId) == false) { return; }

            var listenerInfo = Listeners[optionId];
            listenerInfo.InvokeCallbacks(breakpoint);
        }

        /// <summary>
        /// Determine if the Document matches the provided media query.
        /// </summary>
        /// <param name="mediaQuery"></param>
        /// <returns>Returns true if matched.</returns>
        public async ValueTask<bool> MatchMedia(string mediaQuery) =>
            await _jsRuntime.InvokeAsync<bool>($"mudResizeListener.matchMedia", mediaQuery);

        public static Dictionary<Breakpoint, int> DefaultBreakpointDefinitions { get; set; } = new Dictionary<Breakpoint, int>()
        {
            [Breakpoint.Xl] = 1920,
            [Breakpoint.Lg] = 1280,
            [Breakpoint.Md] = 960,
            [Breakpoint.Sm] = 600,
            [Breakpoint.Xs] = 0,
        };

        public async Task<Breakpoint> GetBreakpoint()
        {
            // note: we don't need to get the size if we are listening for updates, so only if onResized==null, get the actual size
            if (_windowSize == null)
                _windowSize = await _browserWindowSizeProvider.GetBrowserWindowSize();
            if (_windowSize == null)
                return Breakpoint.Xs;
            if (_windowSize.Width >= DefaultBreakpointDefinitions[Breakpoint.Xl])
                return Breakpoint.Xl;
            else if (_windowSize.Width >= DefaultBreakpointDefinitions[Breakpoint.Lg])
                return Breakpoint.Lg;
            else if (_windowSize.Width >= DefaultBreakpointDefinitions[Breakpoint.Md])
                return Breakpoint.Md;
            else if (_windowSize.Width >= DefaultBreakpointDefinitions[Breakpoint.Sm])
                return Breakpoint.Sm;
            else
                return Breakpoint.Xs;
        }

        public async Task<bool> IsMediaSize(Breakpoint breakpoint)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            return IsMediaSize(breakpoint, await GetBreakpoint());
        }

        public bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            return breakpoint switch
            {
                Breakpoint.Xs => reference == Breakpoint.Xs,
                Breakpoint.Sm => reference == Breakpoint.Sm,
                Breakpoint.Md => reference == Breakpoint.Md,
                Breakpoint.Lg => reference == Breakpoint.Lg,
                Breakpoint.Xl => reference == Breakpoint.Xl,
                // * and down
                Breakpoint.SmAndDown => reference <= Breakpoint.Sm,
                Breakpoint.MdAndDown => reference <= Breakpoint.Md,
                Breakpoint.LgAndDown => reference <= Breakpoint.Lg,
                // * and up
                Breakpoint.SmAndUp => reference >= Breakpoint.Sm,
                Breakpoint.MdAndUp => reference >= Breakpoint.Md,
                Breakpoint.LgAndUp => reference >= Breakpoint.Lg,
                _ => false,
            };
        }

        public async Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback) => await Subscribe(callback, _options);

        public async Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback, ResizeOptions options)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            options ??= _options;
            if (options.BreakpointDefinitions == null || options.BreakpointDefinitions.Count == 0)
            {
                options.BreakpointDefinitions = DefaultBreakpointDefinitions.ToDictionary(x => x.Key.ToString(), x => x.Value);
            }

            if (DotNetRef == null)
            {
                DotNetRef = DotNetObjectReference.Create(this);
            }


            try
            {
                await Semaphore.WaitAsync();

                var existingOptionId = Listeners.Where(x => x.Value.Option == options).Select(x => x.Key).FirstOrDefault();

                if (existingOptionId == default)
                {
                    var subscriptionInfo = new BreakpointServiceSubscriptionInfo(options);
                    var subscriptionId = subscriptionInfo.AddSubscription(callback);
                    var listenerId = Guid.NewGuid();

                    Listeners.Add(listenerId, subscriptionInfo);

                    var interopResult = await JsRuntime.InvokeVoidAsyncWithErrorHandling
                        ("mudResizeListenerFactory.listenForResize", DotNetRef, options, listenerId);
                    
                    if (interopResult == true)
                    {
                        if (_breakpoint == Breakpoint.None)
                        {
                            _breakpoint = await GetBreakpoint();

                        }
                    }

                    return new BreakpointServiceSubscribeResult(subscriptionId, _breakpoint);
                }
                else
                {
                    var entry = Listeners[existingOptionId];
                    var subscriptionId = entry.AddSubscription(callback);

                    return new BreakpointServiceSubscribeResult(subscriptionId, _breakpoint);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }

    /// <summary>
    /// The result of a subscription to the BreakpointListener
    /// </summary>
    /// <param name="SubscriptionId">The subscription id, can be used for cancel the subscription later</param>
    /// <param name="Breakpoint">The current breakpoint of the window</param>
    public record BreakpointServiceSubscribeResult(Guid SubscriptionId, Breakpoint Breakpoint);

    public interface IBreakpointService : IAsyncDisposable
    {
        /// <summary>
        /// Check if the current breakpoint fits within the current window size
        /// </summary>
        /// <param name="breakpoint"></param>
        /// <returns>True if the media size is meet, false otherwise. For instance if the current window size is sm and the breakpoint is SmAndSmaller, this method returns true</returns>
        Task<bool> IsMediaSize(Breakpoint breakpoint);

        /// <summary>
        /// Check if the current breakpoint fits within the reference size
        /// </summary>
        /// <param name="breakpoint">The breakpoint to check</param>
        /// <param name="reference">The reference breakpoint (xs,sm,md,lg,xl)</param>
        /// <returns>True if the media size is meet, false otherwise. For instance if the reference size is sm and the breakpoint is SmAndSmaller, this method returns true</returns>
        bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference);

        /// <summary>
        /// Get the current breakpoint
        /// </summary>
        /// <returns></returns>
        Task<Breakpoint> GetBreakpoint();

        /// <summary>
        /// Subscribe to size changes of the browser window with default options
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <returns>Returning an object containing the current breakpoint and a subscription id, that should be used for unsubscribe</returns>
        Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback);

        /// <summary>
        /// Subscribe to size changes of the browser window using the provided options
        /// </summary>
        /// <param name="callback">The method (callbacK) that is invoke as soon as the size of the window has changed</param>
        /// <param name="options">The options used to subscribe to changes</param>
        /// <returns>Returning an object containing the current breakpoint and a subscription id, that should be used for unsubscribe</returns>
        Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback, ResizeOptions options);

        /// <summary>
        /// Used for cancel the subscription to the resize event.
        /// </summary>
        /// <param name="subscriptionId">The subscription id (return of subscribe) to cancel</param>
        /// <returns>True if the subscription could be cancel, false otherwise</returns>
        Task<bool> Unsubscribe(Guid subscriptionId);
    }
}

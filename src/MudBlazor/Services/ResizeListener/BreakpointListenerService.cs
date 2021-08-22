// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    public class BreakpointListenerService : IBreakpointListenerService
    {
        private Guid _listenerJsId;
        private readonly IJSRuntime _jsRuntime;
        private readonly ResizeOptions _options;
        private DotNetObjectReference<BreakpointListenerService> _dotNetRef;
        private Action<Breakpoint> _callback;
        private IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private BrowserWindowSize _windowSize;
        private Breakpoint _breakpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        public BreakpointListenerService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null)
        {
            this._options = options?.Value ?? new ResizeOptions();
            this._jsRuntime = jsRuntime;
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }


        private bool IsAttached() => _dotNetRef != null;

        /// <summary>
        /// Invoked by jsInterop, use the OnResized event handler to subscribe.
        /// </summary>
        /// <param name="browserWindowSize"></param>
        /// <param name="breakpoint"></param>
        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint breakpoint)
        {
            _windowSize = browserWindowSize;
            _breakpoint = breakpoint;
            _callback(breakpoint);
        }

        /// <summary>
        /// Determine if the Document matches the provided media query.
        /// </summary>
        /// <param name="mediaQuery"></param>
        /// <returns>Returns true if matched.</returns>
        public async ValueTask<bool> MatchMedia(string mediaQuery) =>
            await _jsRuntime.InvokeAsync<bool>($"mudResizeListener.matchMedia", mediaQuery);

        public static Dictionary<Breakpoint, int> BreakpointDefinitions { get; set; } = new Dictionary<Breakpoint, int>()
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
            if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Xl])
                return Breakpoint.Xl;
            else if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Lg])
                return Breakpoint.Lg;
            else if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Md])
                return Breakpoint.Md;
            else if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Sm])
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

        public async Task<Breakpoint> Attach(Action<Breakpoint> callback) => await Attach(callback, _options);

        public async Task<Breakpoint> Attach(Action<Breakpoint> callback, ResizeOptions options)
        {
            if (IsAttached() == false)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _dotNetRef = DotNetObjectReference.Create(this);
                _listenerJsId = Guid.NewGuid();

                if (options.BreakpointDefinitions == null)
                {
                    options.BreakpointDefinitions = BreakpointDefinitions.ToDictionary(x => x.Key.ToString(), x => x.Value);
                }

                try
                {
                    await _jsRuntime.InvokeVoidAsync($"mudResizeListenerFactory.listenForResize", _dotNetRef, options, _listenerJsId);
                    var currentBreakpoint = await GetBreakpoint();
                    return currentBreakpoint;
                }
                catch (TaskCanceledException)
                {
                    // no worries here
                    return Breakpoint.None;
                }
            }

            return _breakpoint;
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

        public async ValueTask DisposeAsync() => await Detach();
    }

    public interface IBreakpointListenerService : IAsyncDisposable
    {
        Task<bool> IsMediaSize(Breakpoint breakpoint);
        bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference);
        Task<Breakpoint> Attach(Action<Breakpoint> callback);
        Task<Breakpoint> Attach(Action<Breakpoint> callback, ResizeOptions options);
        Task Detach();
    }
}

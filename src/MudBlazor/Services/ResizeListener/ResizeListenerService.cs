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
    public class ResizeListenerService : IResizeListenerService, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        [DynamicDependency(nameof(RaiseOnResized))]
        public ResizeListenerService(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null)
        {
            this._dotNetRef = DotNetObjectReference.Create(this);
            this._options = options?.Value ?? new ResizeOptions();
            this._options.BreakpointDefinitions = BreakpointDefinitions.ToDictionary(x => x.Key.ToString(), x => x.Value);
            this._jsRuntime = jsRuntime;
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }

        private readonly IJSRuntime _jsRuntime;
        private readonly IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private readonly ResizeOptions _options;
        private readonly DotNetObjectReference<ResizeListenerService> _dotNetRef;
#nullable enable
        private EventHandler<BrowserWindowSize>? _onResized;
        private EventHandler<Breakpoint>? _onBreakpointChanged;

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<BrowserWindowSize>? OnResized
        {
            add
            {
                _options.NotifyOnBreakpointOnly = false;
                Start();
                _onResized += value;
            }
            remove
            {
                _onResized -= value;
                Cancel().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<Breakpoint>? OnBreakpointChanged
        {
            add
            {
                _options.NotifyOnBreakpointOnly = _onResized == null;
                Start();
                _onBreakpointChanged += value;
            }
            remove
            {
                _onBreakpointChanged -= value;
                Cancel().ConfigureAwait(false);
            }
        }
#nullable disable

        private async void Start()
        {
            if (_onResized == null || _onBreakpointChanged == null)
            {
                await _jsRuntime.InvokeVoidAsyncWithErrorHandling($"mudResizeListener.listenForResize", _dotNetRef, _options);
            }
        }

        private async ValueTask Cancel()
        {
            try
            {
                if (_onResized == null && _onBreakpointChanged == null)
                {
                    await _jsRuntime.InvokeVoidAsyncWithErrorHandling($"mudResizeListener.cancelListener");
                }
                else if (_onResized == null && _onBreakpointChanged != null && !_options.NotifyOnBreakpointOnly)
                {
                    _options.NotifyOnBreakpointOnly = true;
                    Start();
                }
            }
            catch (Exception)
            {
                /* ignore */
            }
        }

        /// <summary>
        /// Determine if the Document matches the provided media query.
        /// </summary>
        /// <param name="mediaQuery"></param>
        /// <returns>Returns true if matched.</returns>
        public ValueTask<bool> MatchMedia(string mediaQuery) =>
_jsRuntime.InvokeAsync<bool>($"mudResizeListener.matchMedia", mediaQuery);

        /// <summary>
        /// Get the current BrowserWindowSize, this includes the Height and Width of the document.
        /// </summary>
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize() =>
            _browserWindowSizeProvider.GetBrowserWindowSize();

        /// <summary>
        /// Invoked by jsInterop, use the OnResized event handler to subscribe.
        /// </summary>
        /// <param name="browserWindowSize"></param>
        /// <param name="breakpoint"></param>
        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint breakpoint)
        {
            _windowSize = browserWindowSize;
            _onResized?.Invoke(this, browserWindowSize);
            _onBreakpointChanged?.Invoke(this, breakpoint);
        }

        private BrowserWindowSize _windowSize;

        public static Dictionary<Breakpoint, int> BreakpointDefinitions { get; set; } = new Dictionary<Breakpoint, int>()
        {
            [Breakpoint.Xxl] = 2560,
            [Breakpoint.Xl] = 1920,
            [Breakpoint.Lg] = 1280,
            [Breakpoint.Md] = 960,
            [Breakpoint.Sm] = 600,
            [Breakpoint.Xs] = 0,
        };

        public async Task<Breakpoint> GetBreakpoint()
        {
            // note: we don't need to get the size if we are listening for updates, so only if onResized==null, get the actual size
            if (_onResized == null || _windowSize == null)
                _windowSize = await _browserWindowSizeProvider.GetBrowserWindowSize();
            if (_windowSize == null)
                return Breakpoint.Xs;
            if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Xxl])
                return Breakpoint.Xxl;
            else if (_windowSize.Width >= BreakpointDefinitions[Breakpoint.Xl])
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
                Breakpoint.Xxl => reference == Breakpoint.Xxl,
                // * and down
                Breakpoint.SmAndDown => reference <= Breakpoint.Sm,
                Breakpoint.MdAndDown => reference <= Breakpoint.Md,
                Breakpoint.LgAndDown => reference <= Breakpoint.Lg,
                Breakpoint.XlAndDown => reference <= Breakpoint.Xl,
                // * and up
                Breakpoint.SmAndUp => reference >= Breakpoint.Sm,
                Breakpoint.MdAndUp => reference >= Breakpoint.Md,
                Breakpoint.LgAndUp => reference >= Breakpoint.Lg,
                Breakpoint.XlAndUp => reference >= Breakpoint.Xl,
                _ => false,
            };
        }

        bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _ = Cancel();
                if (disposing)
                {
                    _onResized = null;
                    _onBreakpointChanged = null;
                    _dotNetRef?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


    public interface IResizeListenerService : IDisposable
    {
#nullable enable
        event EventHandler<BrowserWindowSize>? OnResized;
        event EventHandler<Breakpoint>? OnBreakpointChanged;
#nullable disable
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
        Task<bool> IsMediaSize(Breakpoint breakpoint);
        bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference);
        Task<Breakpoint> GetBreakpoint();
    }
}

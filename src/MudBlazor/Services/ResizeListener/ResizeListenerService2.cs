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
    public class ResizeListenerService2 : IResizeListenerService2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="browserWindowSizeProvider"></param>
        /// <param name="options"></param>
        public ResizeListenerService2(IJSRuntime jsRuntime, IBrowserWindowSizeProvider browserWindowSizeProvider, IOptions<ResizeOptions> options = null)
        {
            this._options = options?.Value ?? new ResizeOptions();
            this._options.BreakpointDefinitions = BreakpointDefinitions.ToDictionary(x => x.Key.ToString(), x => x.Value);
            this._jsRuntime = jsRuntime;
            this._browserWindowSizeProvider = browserWindowSizeProvider;
        }

        private Guid _listenerJsId;
        private readonly IJSRuntime _jsRuntime;
        private readonly IBrowserWindowSizeProvider _browserWindowSizeProvider;
        private readonly ResizeOptions _options;
        private DotNetObjectReference<ResizeListenerService2> _dotNetRef;
#nullable enable
        private EventHandler<BrowserWindowSize>? _onResized;
        private EventHandler<Breakpoint>? _onBreakpointChanged;

        private bool IsAttached() => _dotNetRef != null;
        private void ThrowExceptionIfNotAttached()
        {
            if (IsAttached() == false)
            {
                throw new InvalidOperationException("the listener needs to be attached before events can be used");
            }
        }

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<BrowserWindowSize>? OnResized
        {
            add
            {
                // this is a proposal, we can remove it
                ThrowExceptionIfNotAttached();

                _options.NotifyOnBreakpointOnly = false;
                _onResized += value;
            }
            remove
            {
                _onResized -= value;
            }
        }

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<Breakpoint>? OnBreakpointChanged
        {
            add
            {
                // this is a proposal, we can remove it
                ThrowExceptionIfNotAttached();
                _options.NotifyOnBreakpointOnly = _onResized == null;
                _onBreakpointChanged += value;
            }
            remove
            {
                _onBreakpointChanged -= value;
            }
        }
#nullable disable

        /// <summary>
        /// Determine if the Document matches the provided media query.
        /// </summary>
        /// <param name="mediaQuery"></param>
        /// <returns>Returns true if matched.</returns>
        public async ValueTask<bool> MatchMedia(string mediaQuery) =>
            await _jsRuntime.InvokeAsync<bool>($"mudResizeListener.matchMedia", mediaQuery);

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

        public async Task Attach() => await Attach(_options);

        public async Task Attach(ResizeOptions options)
        {
            if (IsAttached() == false)
            {
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

    public interface IResizeListenerService2 : IAsyncDisposable
    {
#nullable enable
        event EventHandler<BrowserWindowSize>? OnResized;
        event EventHandler<Breakpoint>? OnBreakpointChanged;
#nullable disable
        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
        Task<bool> IsMediaSize(Breakpoint breakpoint);
        bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference);
        Task<Breakpoint> GetBreakpoint();
        Task Attach();
        Task Attach(ResizeOptions options);
        Task Detach();
    }
}

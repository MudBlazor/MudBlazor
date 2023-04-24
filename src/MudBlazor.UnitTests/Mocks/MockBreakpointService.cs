using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
#pragma warning disable CS1998 // Justification - Implementing IResizeListenerService
    public class MockBreakpointService : IBreakpointService
    {
        private int _width, _height;

        internal void ApplyScreenSize(int width, int height)
        {
            _width = width;
            _height = height;

            OnResized?.Invoke(this, new BrowserWindowSize()
            {
                Width = _width,
                Height = _height
            });
            OnBreakpointChanged?.Invoke(this, GetBreakpointInternal());
        }

#nullable enable
#pragma warning disable CS0414 // justification implementing interface  
        public event EventHandler<BrowserWindowSize>? OnResized;
        public event EventHandler<Breakpoint>? OnBreakpointChanged;
#pragma warning restore CS0414 
#nullable disable
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new BrowserWindowSize()
            {
                Width = _width,
                Height = _height
            };
        }

        public async Task<bool> IsMediaSize(Breakpoint breakpoint)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            if (breakpoint == Breakpoint.Always)
                return true;

            return IsMediaSize(breakpoint, await GetBreakpoint());
        }

        public bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            if (breakpoint == Breakpoint.Always)
                return true;

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

        public async Task<Breakpoint> GetBreakpoint() => GetBreakpointInternal();

        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback) => SubscribeAsync(callback);

        public Task<BreakpointServiceSubscribeResult> SubscribeAsync(Action<Breakpoint> callback) => Task.FromResult(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Sm));

        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<BreakpointServiceSubscribeResult> Subscribe(Action<Breakpoint> callback, ResizeOptions options) => SubscribeAsync(callback, options);

        public Task<BreakpointServiceSubscribeResult> SubscribeAsync(Action<Breakpoint> callback, ResizeOptions options) => Task.FromResult(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Sm));

        [Obsolete($"Use {nameof(UnsubscribeAsync)} instead. This will be removed in v7.")]
        public Task<bool> Unsubscribe(Guid subscriptionId) => UnsubscribeAsync(subscriptionId);

        public Task<bool> UnsubscribeAsync(Guid subscriptionId) => Task.FromResult(true);

        private Breakpoint GetBreakpointInternal()
        {
            if (_width >= ResizeListenerService.BreakpointDefinitions[Breakpoint.Xl])
                return Breakpoint.Xl;
            else if (_width >= ResizeListenerService.BreakpointDefinitions[Breakpoint.Lg])
                return Breakpoint.Lg;
            else if (_width >= ResizeListenerService.BreakpointDefinitions[Breakpoint.Md])
                return Breakpoint.Md;
            else if (_width >= ResizeListenerService.BreakpointDefinitions[Breakpoint.Sm])
                return Breakpoint.Sm;
            else
                return Breakpoint.Xs;
        }

        public ValueTask DisposeAsync()
        {
            OnResized = null;
            OnBreakpointChanged = null;
            return ValueTask.CompletedTask;
        }
    }
#pragma warning restore CS1998
}

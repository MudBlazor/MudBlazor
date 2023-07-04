using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
#pragma warning disable CS1998 // Justification - Implementing IResizeListenerService
    [Obsolete("Replaced by IBrowserViewportService. Remove in v7.")]
    public class MockResizeListenerService : IResizeListenerService
    {
        private int _width, _height;

        public void Dispose()
        {
            OnResized = null;
            OnBreakpointChanged = null;
        }

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

        public async Task<Breakpoint> GetBreakpoint() => GetBreakpointInternal();

        private Breakpoint GetBreakpointInternal()
        {
            if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Xxl])
                return Breakpoint.Xxl;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Xl])
                return Breakpoint.Xl;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Lg])
                return Breakpoint.Lg;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Md])
                return Breakpoint.Md;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Sm])
                return Breakpoint.Sm;
            else
                return Breakpoint.Xs;
        }
    }
#pragma warning restore CS1998
}

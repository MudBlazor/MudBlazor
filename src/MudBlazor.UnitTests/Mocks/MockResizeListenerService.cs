using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockResizeListenerService : IResizeListenerService
    {
        public void Dispose()
        {
            OnResized = null;
        }

#nullable enable
        public event EventHandler<BrowserWindowSize>? OnResized;
#nullable disable
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new BrowserWindowSize();
        }

        public async Task<bool> IsMediaSize(Breakpoint breakpoint)
        {
            // TODO: implement this fake service for tests
            return false;
        }

        public async Task<Breakpoint> GetBreakpoint()
        {
            // TODO: implement this fake service for tests
            return default(Breakpoint);
        }

    }
}

using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    #pragma warning disable CS1998 // Justification - Implementing IResizeListenerService
    public class MockResizeListenerService : IResizeListenerService
    {
        public void Dispose()
        {
            OnResized = null;
        }

#nullable enable
#pragma warning disable CS0067 // justification implementing interface  
        public event EventHandler<BrowserWindowSize>? OnResized;
#pragma warning restore CS0067 
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
   #pragma warning restore CS1998
}
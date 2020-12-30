using MudBlazor.Providers;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockBrowserWindowSizeProvider : IBrowserWindowSizeProvider
    {
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new BrowserWindowSize();
        }
    }
}

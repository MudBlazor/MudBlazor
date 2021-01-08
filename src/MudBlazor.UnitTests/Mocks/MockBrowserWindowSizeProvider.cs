using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockBrowserWindowSizeProvider : IBrowserWindowSizeProvider
    {
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new ValueTask<BrowserWindowSize>(new BrowserWindowSize());
        }
    }
}

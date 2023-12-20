using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    [Obsolete("Replaced by IBrowserViewportService. Remove in v7.")]
    public class MockBrowserWindowSizeProvider : IBrowserWindowSizeProvider
    {
        public ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new ValueTask<BrowserWindowSize>(new BrowserWindowSize());
        }
    }
}

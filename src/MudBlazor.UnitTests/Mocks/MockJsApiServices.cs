using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    /// <summary>
    /// Mock for scroll listener
    /// </summary>
    public class MockJsApiServices : IJsApiService
    {
        public ValueTask CopyToClipboardAsync(string text) => ValueTask.CompletedTask;

        public ValueTask Open(string link, string target) => ValueTask.CompletedTask;

        public ValueTask OpenInNewTabAsync(string url) => ValueTask.CompletedTask;
    }
}

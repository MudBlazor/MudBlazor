using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockJsApiService : IJsApiService
    {
        public ValueTask CopyToClipboardAsync(string text) => ValueTask.CompletedTask;

        public ValueTask Open(string link, string target) => ValueTask.CompletedTask;

        public ValueTask UpdateStyleProperty(string elementId, string propertyName, object value) => ValueTask.CompletedTask;

        public ValueTask OpenInNewTabAsync(string url) => ValueTask.CompletedTask;
    }
}

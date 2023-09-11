using System.Threading.Tasks;
using MudBlazor.Docs.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockDocsJsApiService : IDocsJsApiService
    {

        public ValueTask<string> GetInnerTextByIdAsync(string id) => ValueTask.FromResult("inner text");

    }
}

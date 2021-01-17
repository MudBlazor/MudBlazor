using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockDomService : IDomService
    {
        public ValueTask ChangeCss(ElementReference elementReference, string css)
            => new ValueTask();

        public ValueTask ChangeCssById(string id, string css)
            => new ValueTask();

        public ValueTask ChangeGlobalVariable(string variableName, int value)
            => new ValueTask();

        public ValueTask ChangeVariable(ElementReference element, string variableName, int value)
            => new ValueTask();

        public ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementReference)
            => new ValueTask<BoundingClientRect>(new BoundingClientRect());
    }
}

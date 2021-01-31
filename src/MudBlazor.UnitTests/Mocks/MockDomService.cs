using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        public ValueTask ChangeGlobalCssVariable(string variableName, int value)
            => new ValueTask();

        public ValueTask ChangeCssVariable(ElementReference element, string variableName, int value)
            => new ValueTask();

        public ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementReference)
            => new ValueTask<BoundingClientRect>(new BoundingClientRect());

        public ValueTask AddEventListener<T>(ElementReference element, DotNetObjectReference<T> dotnet, string @event, string callback) where T : class =>
            new ValueTask();
    }
}

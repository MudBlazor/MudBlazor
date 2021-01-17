using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public interface IDomService
    {
        ValueTask ChangeCssById(string id, string css);
        ValueTask ChangeCss(ElementReference elementReference, string css);
        ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementReference);
        ValueTask ChangeGlobalVariable(string variableName, int value);
        ValueTask ChangeVariable(ElementReference element, string variableName, int value);
    }

    public class DomService : IDomService
    {
        private IJSRuntime _jsRuntime;

        public DomService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask ChangeCssById(string id, string css) =>
            _jsRuntime.InvokeVoidAsync("changeCssById", id, css);

        public ValueTask ChangeCss(ElementReference elementReference, string css) =>
            _jsRuntime.InvokeVoidAsync("changeCss", elementReference, css);

        public ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementReference) =>
            _jsRuntime.InvokeAsync<BoundingClientRect>("getMudBoundingClientRect", elementReference);

        public ValueTask ChangeGlobalVariable(string variableName, int value) =>
            _jsRuntime.InvokeVoidAsync("changeGlobalVariable", variableName, value);

        public ValueTask ChangeVariable(ElementReference element, string variableName, int value) =>
            _jsRuntime.InvokeVoidAsync("changeGlobalVariable", element, variableName, value);
    }
}

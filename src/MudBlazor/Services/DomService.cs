using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
        ValueTask ChangeGlobalCssVariable(string variableName, int value);
        ValueTask ChangeCssVariable(ElementReference element, string variableName, int value);
        ValueTask<int> AddEventListener<T>(ElementReference element, DotNetObjectReference<T> dotnet, string @event, string callback, bool stopPropagation = false) where T : class;
        ValueTask RemoveEventListener(ElementReference element, string @event, int eventId);
        ValueTask OpenInNewTab(string url);
    }

    public class DomService : IDomService
    {
        private IJSRuntime _jsRuntime;

        public DomService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask ChangeCssById(string id, string css) =>
            _jsRuntime.InvokeVoidAsync("domService.changeCssById", id, css);

        public ValueTask ChangeCss(ElementReference elementReference, string css) =>
            _jsRuntime.InvokeVoidAsync("domService.changeCss", elementReference, css);

        public ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementReference) =>
            _jsRuntime.InvokeAsync<BoundingClientRect>("domService.getMudBoundingClientRect", elementReference);

        public ValueTask ChangeGlobalCssVariable(string variableName, int value) =>
            _jsRuntime.InvokeVoidAsync("domService.changeGlobalCssVariable", variableName, value);

        public ValueTask ChangeCssVariable(ElementReference element, string variableName, int value) =>
            _jsRuntime.InvokeVoidAsync("domService.changeCssVariable", element, variableName, value);

		public ValueTask OpenInNewTab(string url) =>
            _jsRuntime.InvokeVoidAsync("domService.mudOpen", new object[2] { url, "_blank" }); 
  
        public ValueTask<int> AddEventListener<T>(ElementReference element, DotNetObjectReference<T> dotnet, string @event, string callback, bool stopPropagation = false) where T : class
        {
            var parameters = dotnet.Value.GetType().GetMethods().First(m => m.Name == callback).GetParameters().Select(p => p.ParameterType);
            var parameterSpecs = new object[parameters.Count()];
            for (int i = 0; i < parameters.Count(); ++i)
            {
                parameterSpecs[i] = GetSerializationSpec(parameters.ElementAt(i));
            }
            return _jsRuntime.InvokeAsync<int>("domService.addMudEventListener", element, dotnet, @event, callback, parameterSpecs, stopPropagation);
        }

        public ValueTask RemoveEventListener(ElementReference element, string @event, int eventId) =>
            _jsRuntime.InvokeVoidAsync("domService.removeMudEventListener", element, eventId);

        private object GetSerializationSpec(Type type)
        {
            var props = type.GetProperties();
            var propsSpec = new Dictionary<string, object>();
            foreach(var prop in props)
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    propsSpec.Add(ToJsString(prop.Name), "*");
                }
                else if (prop.PropertyType.IsArray)
                {
                    propsSpec.Add(ToJsString(prop.Name), GetSerializationSpec(prop.PropertyType.GetElementType()));
                }
                else if (prop.PropertyType.IsClass)
                {
                    propsSpec.Add(ToJsString(prop.Name), GetSerializationSpec(prop.PropertyType));
                }
            }
            return propsSpec;
        }

        private string ToJsString(string s) => char.ToLower(s[0]) + s.Substring(1);
    }
}

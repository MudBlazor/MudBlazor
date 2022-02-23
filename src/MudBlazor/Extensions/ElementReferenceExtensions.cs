using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public static class ElementReferenceExtensions
    {
        private static readonly PropertyInfo jsRuntimeProperty =
            typeof(WebElementReferenceContext).GetProperty("JSRuntime", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static IJSRuntime GetJSRuntime(this ElementReference elementReference)
        {
            if (elementReference.Context is not WebElementReferenceContext context)
            {
                return null;
            }

            return (IJSRuntime)jsRuntimeProperty.GetValue(context);
        }

        public static ValueTask MudFocusFirstAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.focusFirst", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask MudFocusLastAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.focusLast", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask MudSaveFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.saveFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudRestoreFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.restoreFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudSelectAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.select", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudSelectRangeAsync(this ElementReference elementReference, int pos1, int pos2) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.selectRange", elementReference, pos1, pos2) ?? ValueTask.CompletedTask;

        public static ValueTask MudChangeCssAsync(this ElementReference elementReference, string css) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.changeCss", elementReference, css) ?? ValueTask.CompletedTask;

        public static ValueTask<BoundingClientRect> MudGetBoundingClientRectAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("mudElementRef.getBoundingClientRect", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Gets the client rect of the element 
        /// </summary>
        public static ValueTask<BoundingClientRect> MudGetClientRectFromParentAsync(this ElementReference elementReference) =>
           elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("mudElementRef.getClientRectFromParent", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Gets the client rect of the first child of the element.
        /// Useful when you want to know the dimensions of a render fragment and for that you wrap it into a div
        /// </summary>
        public static ValueTask<BoundingClientRect> MudGetClientRectFromFirstChildAsync(this ElementReference elementReference) =>
           elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("mudElementRef.getClientRectFromFirstChild", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Returns true if the element has an ancestor with style position == "fixed"
        /// </summary>
        /// <param name="elementReference"></param>
        public static ValueTask<bool> MudHasFixedAncestorsAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?
            .InvokeAsync<bool>("mudElementRef.hasFixedAncestors", elementReference) ?? ValueTask.FromResult(false);


        public static ValueTask MudChangeCssVariableAsync(this ElementReference elementReference, string variableName, int value) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.changeCssVariable", elementReference, variableName, value) ?? ValueTask.CompletedTask;

        public static ValueTask<int> MudAddEventListenerAsync<T>(this ElementReference elementReference, DotNetObjectReference<T> dotnet, string @event, string callback, bool stopPropagation = false) where T : class
        {
            var parameters = dotnet?.Value.GetType().GetMethods().First(m => m.Name == callback).GetParameters().Select(p => p.ParameterType);
            if (parameters != null)
            {
                var parameterSpecs = new object[parameters.Count()];
                for (var i = 0; i < parameters.Count(); ++i)
                {
                    parameterSpecs[i] = GetSerializationSpec(parameters.ElementAt(i));
                }
                return elementReference.GetJSRuntime()?.InvokeAsync<int>("mudElementRef.addEventListener", elementReference, dotnet, @event, callback, parameterSpecs, stopPropagation) ?? ValueTask.FromResult(0);
            }
            else
            {
                return new ValueTask<int>(0);
            }
        }

        public static ValueTask MudRemoveEventListenerAsync(this ElementReference elementReference, string @event, int eventId) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.removeEventListener", elementReference, eventId) ?? ValueTask.CompletedTask;

        private static object GetSerializationSpec(Type type)
        {
            var props = type.GetProperties();
            var propsSpec = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    propsSpec.Add(prop.Name.ToJsString(), "*");
                }
                else if (prop.PropertyType.IsArray)
                {
                    propsSpec.Add(prop.Name.ToJsString(), GetSerializationSpec(prop.PropertyType.GetElementType()));
                }
                else if (prop.PropertyType.IsClass)
                {
                    propsSpec.Add(prop.Name.ToJsString(), GetSerializationSpec(prop.PropertyType));
                }
            }
            return propsSpec;
        }
    }
}

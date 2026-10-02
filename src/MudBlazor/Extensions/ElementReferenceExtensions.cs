using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public static class ElementReferenceExtensions
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<JSRuntime>k__BackingField")]
        private static extern ref IJSRuntime GetJsRuntime(WebElementReferenceContext context);

        internal static IJSRuntime? GetJSRuntime(this ElementReference elementReference)
        {
            if (elementReference.Context is WebElementReferenceContext context)
            {
                var jsRuntime = GetJsRuntime(context);

                return jsRuntime;
            }

            return null;
        }

        public static ValueTask MudFocusFirstAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.focusFirst", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask MudFocusLastAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.focusLast", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask MudSaveFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.saveFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudRestoreFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.restoreFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudBlurAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.blur", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudSelectAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.select", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask MudSelectRangeAsync(this ElementReference elementReference, int pos1, int pos2) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.selectRange", elementReference, pos1, pos2) ?? ValueTask.CompletedTask;

        public static ValueTask MudChangeCssAsync(this ElementReference elementReference, string css) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("mudElementRef.changeCss", elementReference, css) ?? ValueTask.CompletedTask;

        public static async ValueTask<BoundingClientRect> MudGetBoundingClientRectAsync(this ElementReference elementReference)
        {
            var jsRuntime = elementReference.GetJSRuntime();
            if (jsRuntime is null)
            {
                return new BoundingClientRect();
            }

            var (_, boundingClientRect) = await jsRuntime.InvokeAsyncWithErrorHandling(new BoundingClientRect(), "mudElementRef.getBoundingClientRect", elementReference);

            return boundingClientRect;
        }

        public static async ValueTask<int[]> AddDefaultPreventingHandlers(this ElementReference elementReference, string[] eventNames)
        {
            var jsRuntime = elementReference.GetJSRuntime();
            if (jsRuntime is null)
            {
                return Array.Empty<int>();
            }

            var (_, listenerIds) = await jsRuntime.InvokeAsyncWithErrorHandling(Array.Empty<int>(), "mudElementRef.addDefaultPreventingHandlers", elementReference, eventNames);

            return listenerIds;
        }

        public static ValueTask RemoveDefaultPreventingHandlers(this ElementReference elementReference, string[] eventNames, int[] listenerIds)
        {
            // No handlers were attached (for example, the script was unavailable), so there is nothing to remove.
            if (listenerIds.Length == 0)
            {
                return ValueTask.CompletedTask;
            }

            if (eventNames.Length != listenerIds.Length)
            {
                throw new ArgumentException($"Number of elements in {nameof(eventNames)} and {nameof(listenerIds)} has to match.");
            }

            return elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.removeDefaultPreventingHandlers", elementReference, eventNames, listenerIds) ?? ValueTask.CompletedTask;
        }

        public static ValueTask MudAttachBlurEventWithJS<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
            this ElementReference elementReference,
            DotNetObjectReference<T> obj) where T : class =>
            elementReference.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors("mudElementRef.addOnBlurEvent", elementReference, obj) ?? ValueTask.CompletedTask;
    }
}

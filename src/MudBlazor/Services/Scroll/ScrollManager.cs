// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    /// <summary>
    /// Inject with the AddMudBlazorScrollServices extension.
    /// </summary>
    public interface IScrollManager
    {
        ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior);
        ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior);
        ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
        ValueTask ScrollToYearAsync(string elementId);
        ValueTask ScrollToListItemAsync(string elementId);
        ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
    }

    public class ScrollManager : IScrollManager
    {
        private readonly IJSRuntime _jSRuntime;

        public ScrollManager(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        /// <summary>
        /// Scrolls to the coordinates of the element.
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="left">x coordinate</param>
        /// <param name="top">y coordinate</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollTo", id, left, top, behavior.ToDescriptionString());

        /// <summary>
        /// Scrolls the first instance of the selector into view.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollIntoView", selector, behavior.ToDescriptionString());

        /// <summary>
        /// Scrolls to the top of the element.
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="scrollBehavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ScrollToAsync(id, 0, 0, scrollBehavior);

        /// <summary>
        /// Scroll to the bottom of the element (or if not found to the bottom of the page).
        /// </summary>
        /// <param name="id">id of element of null to scroll to page bottom</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToBottomAsync(string id, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToBottom", id, behavior.ToDescriptionString());

        public ValueTask ScrollToYearAsync(string elementId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToYear", elementId);

        public ValueTask ScrollToListItemAsync(string elementId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToListItem", elementId);

        public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.lockScroll", selector, cssClass);

        public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            _jSRuntime.InvokeVoidAsyncIgnoreErrors("mudScrollManager.unlockScroll", selector, cssClass);
    }

    /// <summary>
    /// Smooth: scrolls in a smooth fashion;
    /// Auto: is immediate
    /// </summary>
    public enum ScrollBehavior
    {
        [Description("smooth")]
        Smooth,
        [Description("auto")]
        Auto
    }
}

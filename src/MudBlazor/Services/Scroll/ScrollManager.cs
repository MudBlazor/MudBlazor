using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Extensions;

namespace MudBlazor
{
    /// <summary>
    /// Inject with the AddMudBlazorScrollServices extension
    /// </summary>
    public interface IScrollManager
    {
        [Obsolete]
        string Selector { get; set; }
        [Obsolete]
        Task ScrollTo(int left, int top, ScrollBehavior scrollBehavior);
        [Obsolete]
        Task ScrollToFragment(string id, ScrollBehavior behavior);
        [Obsolete]
        Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto);

        ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior);
        ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior);
        ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior);
        ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
        ValueTask ScrollToYearAsync(string elementId);
        ValueTask ScrollToListItemAsync(string elementId);
        ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
    }

    public class ScrollManager : IScrollManager
    {
        [Obsolete]
        public string Selector { get; set; }
        private readonly IJSRuntime _jSRuntime;

        public ScrollManager(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        /// <summary>
        /// Scroll to an url fragment
        /// </summary>
        /// <param name="id">The id of the selector that is going to be scrolled to</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        [Obsolete("Please use ScrollIntoViewAsync instead")]
        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) => ScrollIntoViewAsync(id, behavior);

        [Obsolete]
        public async Task ScrollToFragment(string id, ScrollBehavior behavior) =>
            await ScrollToFragmentAsync(id, behavior);

        /// <summary>
        /// Scrolls to the coordinates of the element
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="left">x coordinate</param>
        /// <param name="top">y coordinate</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollTo", id, left, top, behavior.ToDescriptionString());

        [Obsolete]
        public async Task ScrollTo(int left, int top, ScrollBehavior behavior) =>
            await ScrollToAsync(Selector, left, top, behavior);

        /// <summary>
        /// Scrolls the first instance of the selector into view
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollIntoView", selector, behavior.ToDescriptionString());

        /// <summary>
        /// Scrolls to the top of the element
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="scrollBehavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ScrollToAsync(id, 0, 0, scrollBehavior);

        public async Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            await ScrollToAsync(Selector, 0, 0, scrollBehavior);
#pragma warning restore CS0612 // Type or member is obsolete
        }

        /// <summary>
        /// Scroll to the bottom of the element (or if not found to the bottom of the page)
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
        Smooth,
        Auto
    }
}

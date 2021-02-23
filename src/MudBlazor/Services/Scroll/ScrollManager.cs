﻿using System;
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
        ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior);
        ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
        ValueTask ScrollToYearAsync(string elementId);
        ValueTask ScrollToListItemAsync(string elementId, int increment, bool onEdges);
        ValueTask LockScrollAsync(string elementId, string cssClass);
        ValueTask UnlockScrollAsync(string elementId, string cssClass);
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
        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToFragment", id, behavior.ToDescriptionString());

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

        public ValueTask ScrollToYearAsync(string elementId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToYear", elementId);

        public ValueTask ScrollToListItemAsync(string elementId, int increment, bool onEdges) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToListItem", elementId, increment, onEdges);

        public ValueTask LockScrollAsync(string elementId, string cssClass) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.lockScroll", elementId, cssClass);

        public ValueTask UnlockScrollAsync(string elementId, string cssClass) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.unlockScroll", elementId, cssClass);
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

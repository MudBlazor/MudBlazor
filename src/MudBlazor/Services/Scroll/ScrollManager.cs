using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;

namespace MudBlazor
{
    /// <summary>
    /// Inject with the AddMudBlazorScrollServices extension
    /// </summary>
    public interface IScrollManager
    {
        string Selector { get; set; }
        Task ScrollTo(int left, int top, ScrollBehavior scrollBehavior);
        Task ScrollToFragment(string id, ScrollBehavior behavior);
        Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
    }




    public class ScrollManager : IScrollManager
    {
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
        public async Task ScrollToFragment(string id, ScrollBehavior behavior)
        {
            await _jSRuntime
                .InvokeVoidAsync("scrollHelpers.scrollToFragment",
                                            id,
                                            behavior.ToDescriptionString());
        }

        /// <summary>
        /// Scrolls to the coordinates of the element defined in Selector property
        /// </summary>
        /// <param name="left">x coordinate</param>
        /// <param name="top">y coordinate</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public async Task ScrollTo(int left = 0, int top = 0, ScrollBehavior behavior = ScrollBehavior.Auto)
        {

            await _jSRuntime
                .InvokeVoidAsync("scrollHelpers.scrollTo",
                                            Selector,
                                            left,
                                            top,
                                            behavior.ToDescriptionString());
        }

        /// <summary>
        /// Scrolls to the top of the element defined in Selector property
        /// </summary>
        /// <param name="scrollBehavior">smooth or auto</param>
        /// <returns></returns>
        public async Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto)
        {
            await ScrollTo(0, 0, scrollBehavior);
        }
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

﻿using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudScrollToTop : IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-scroll-to-top")
            .AddClass("visible", Visible && string.IsNullOrWhiteSpace(VisibleCssClass))
            .AddClass("hidden", !Visible && string.IsNullOrWhiteSpace(HiddenCssClass))
            .AddClass(VisibleCssClass, Visible && !string.IsNullOrWhiteSpace(VisibleCssClass))
            .AddClass(HiddenCssClass, !Visible && !string.IsNullOrWhiteSpace(HiddenCssClass))
            .AddClass(Class)
            .Build();

        [Inject] IScrollListener ScrollListener { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        [Parameter] public string Selector { get; set; }

        /// <summary>
        /// If set to true, it starts Visible. If sets to false, it will become visible when the TopOffset amount of scrolled pixels is reached
        /// </summary>
        [Parameter] public bool Visible { get; set; }

        /// <summary>
        /// CSS class for the Visible state. Here, apply some transitions and animations that will happen when the component becomes visible
        /// </summary>
        [Parameter] public string VisibleCssClass { get; set; }

        /// <summary>
        /// CSS class for the Hidden state. Here, apply some transitions and animations that will happen when the component becomes invisible
        /// </summary>
        [Parameter] public string HiddenCssClass { get; set; }

        /// <summary>
        /// The distance in pixels scrolled from the top of the selected element from which 
        /// the component becomes visible
        /// </summary>
        [Parameter] public int TopOffset { get; set; } = 300;

        /// <summary>
        /// Smooth or Auto
        /// </summary>
        [Parameter] public ScrollBehavior ScrollBehavior { get; set; } = ScrollBehavior.Smooth;

        /// <summary>
        /// Called when scroll event is fired
        /// </summary>
        [Parameter] public EventCallback<ScrollEventArgs> OnScroll { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                var selector = !string.IsNullOrWhiteSpace(Selector)
                    ? Selector
                    : null;// null is defaulted to document element in JS function
                ScrollListener.Selector = selector;

                //subscribe to event
                ScrollListener.OnScroll += ScrollListener_OnScroll;
            }
        }

        /// <summary>
        /// event received when scroll in the selected element happens
        /// </summary>
        /// <param name="sender">ScrollListener instance</param>
        /// <param name="e">Information about the position of the scrolled element</param>
        private async void ScrollListener_OnScroll(object sender, ScrollEventArgs e)
        {
            await OnScroll.InvokeAsync(e);

            var topOffset = e.NodeName == "#document"
                ? e.FirstChildBoundingClientRect.Top * -1
                : e.ScrollTop;

            if (topOffset >= TopOffset && Visible != true)
            {
                Visible = true;
                await InvokeAsync(() => StateHasChanged());
            }

            if (topOffset < TopOffset && Visible == true)
            {
                Visible = false;
                await InvokeAsync(() => StateHasChanged());
            }
        }

        /// <summary>
        /// Scrolls to top when clicked
        /// </summary>
        private void OnClick()
        {
            ScrollManager.ScrollToTopAsync(ScrollListener.Selector, ScrollBehavior);
        }

        /// <summary>
        /// Remove the event
        /// </summary>
        public void Dispose()
        {
            ScrollListener.OnScroll -= ScrollListener_OnScroll;
        }
    }
}

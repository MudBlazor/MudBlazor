using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A button which lets the user jump to the top of the page.
    /// </summary>
    public partial class MudScrollToTop : IDisposable
    {
        private IScrollListener? _scrollListener;

        /// <summary>
        /// The CSS classes applied to this component.
        /// </summary>
        protected string Classname =>
            new CssBuilder("mud-scroll-to-top")
                .AddClass("visible", Visible && string.IsNullOrWhiteSpace(VisibleCssClass))
                .AddClass("hidden", !Visible && string.IsNullOrWhiteSpace(HiddenCssClass))
                .AddClass(VisibleCssClass, Visible && !string.IsNullOrWhiteSpace(VisibleCssClass))
                .AddClass(HiddenCssClass, !Visible && !string.IsNullOrWhiteSpace(HiddenCssClass))
                .AddClass(Class)
                .Build();

        [Inject]
        private IScrollListenerFactory ScrollListenerFactory { get; set; } = null!;

        [Inject]
        private IScrollManager ScrollManager { get; set; } = null!;

        /// <summary>
        /// The content within this button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The CSS selector to which the scroll event will be attached.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Behavior)]
        public string? Selector { get; set; }

        /// <summary>
        /// Displays this button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>false</c>, this will become <c>true</c> once the user scrolls down the number of pixels in <see cref="TopOffset"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// The CSS classes applied when <see cref="Visible"/> becomes <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This is typically set to transition and animation CSS classes.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Appearance)]
        public string? VisibleCssClass { get; set; }

        /// <summary>
        /// The CSS classes applied when <see cref="Visible"/> becomes <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is typically set to transition and animation CSS classes.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Appearance)]
        public string? HiddenCssClass { get; set; }

        /// <summary>
        /// The number of pixels scrolled before the scroll-to-top button becomes visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c> (300 pixels).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Behavior)]
        public int TopOffset { get; set; } = 300;

        /// <summary>
        /// The scroll behavior when the scroll-to-top button is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ScrollBehavior.Smooth"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ScrollToTop.Behavior)]
        public ScrollBehavior ScrollBehavior { get; set; } = ScrollBehavior.Smooth;

        /// <summary>
        /// Occurs when the page is scrolled.
        /// </summary>
        [Parameter]
        public EventCallback<ScrollEventArgs> OnScroll { get; set; }

        /// <summary>
        /// Occurs when the scroll-to-top button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {

                var selector = !string.IsNullOrWhiteSpace(Selector)
                    ? Selector
                    : null;// null is defaulted to document element in JS function

                _scrollListener = ScrollListenerFactory.Create(selector);

                //subscribe to event
                _scrollListener.OnScroll += ScrollListener_OnScroll;
            }
        }

        /// <summary>
        /// Occurs when the selected element is scrolled.
        /// </summary>
        /// <param name="sender">The <see cref="ScrollListener"/> instance.</param>
        /// <param name="e">Information about the position of the scrolled element.</param>
        private async void ScrollListener_OnScroll(object? sender, ScrollEventArgs e)
        {
            await OnScroll.InvokeAsync(e);

            var topOffset = e.NodeName == "#document"
                ? e.FirstChildBoundingClientRect.Top * -1
                : e.ScrollTop;

            if (topOffset >= TopOffset && Visible != true)
            {
                Visible = true;
                await InvokeAsync(StateHasChanged);
            }

            if (topOffset < TopOffset && Visible)
            {
                Visible = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Scrolls to top when clicked and invokes OnClick
        /// </summary>
        private async Task OnButtonClick(MouseEventArgs args)
        {
            await ScrollManager.ScrollToTopAsync(_scrollListener?.Selector, ScrollBehavior);
            await OnClick.InvokeAsync(args);
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            if (_scrollListener == null) { return; }

            _scrollListener.OnScroll -= ScrollListener_OnScroll;
            _scrollListener.Dispose();
        }
    }
}

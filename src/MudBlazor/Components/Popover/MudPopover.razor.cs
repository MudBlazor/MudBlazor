using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;


namespace MudBlazor
{
    public partial class MudPopover : MudComponentBase
    {
        protected string PopoverClass =>
           new CssBuilder("mud-popover")
            .AddClass("mud-popover-open", Open)
            .AddClass("mud-paper")
            .AddClass("mud-paper-square", Square)
            .AddClass($"mud-elevation-{Elevation.ToString()}")
            .AddClass(Class)
           .Build();

        protected string PopoverStyles =>
            new StyleBuilder()
            .AddStyle("max-height", $"{MaxHeight}px", MaxHeight != null)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow set to 8 by default.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 8;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// Sets the maxheight the popover can have when open.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// If true, the popover is visible.
        /// </summary>
        [Parameter] public bool Open { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

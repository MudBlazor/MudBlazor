using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;


namespace MudBlazor
{
    public partial class MudPopover : MudComponentBase
    {
        private ElementReference _popoverRef;
        protected string PopoverClass =>
           new CssBuilder("mud-popover")
            .AddClass("mud-popover-open", Open)
            .AddClass($"mud-popover-{Direction.ToDescriptionString()}")
            .AddClass("mud-popover-offset-y", OffsetY)
            .AddClass("mud-popover-offset-x", OffsetX)
            .AddClass("mud-paper")
            .AddClass("mud-paper-square", Square)
            .AddClass($"mud-elevation-{Elevation}")
            .AddClass(Class)
           .Build();

        protected string PopoverStyles =>
            new StyleBuilder()
            .AddStyle("max-height", $"{MaxHeight}px", MaxHeight != null)
            .AddStyle(Style)
            .Build();

        [Inject] public IBrowserWindowSizeProvider WindowSize { get; set; }

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
        /// Sets the direction the popover will start from relative to its parent.
        /// </summary>
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the select menu will open either above or bellow the input depending on the direction.
        /// </summary>
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the select menu will open either before or after the input depending on the direction.
        /// </summary>
        [Parameter] public bool OffsetY { get; set; } = true;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }



        //private async Task SetDirection()
        //{
        //    if (!Open) return;
        //    var rect = await _popoverRef.MudGetBoundingClientRectAsync();
        //    var viewport = await WindowSize.GetBrowserWindowSize();
        //    var direction = Direction;

        //    switch (Direction)
        //    {
        //        case Direction.Bottom:
        //            if (rect.Bottom > viewport.Height)
        //            {
        //                direction = Direction.Top;
        //            }
        //            break;

        //        case Direction.Top:
        //            if (rect.Top < 0)
        //            {
        //                direction = Direction.Bottom;
        //            }
        //            break;
        //        case Direction.Left:
        //            if (rect.Left < 0)
        //            {
        //                direction = Direction.Right;
        //            }
        //            break;
        //        case Direction.Right:
        //            if (rect.Right > viewport.Width)
        //            {
        //                direction = Direction.Left;
        //            }
        //            break;
        //    }
        //    if (direction != Direction)
        //    {
        //        Direction = direction;
        //        StateHasChanged();
        //    }
        //}
    }
}

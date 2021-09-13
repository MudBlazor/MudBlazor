using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPopover : MudComponentBase, IAsyncDisposable
    {
        [Inject] public IJSRuntime JsInterop { get; set; }

        private Boolean _connected; 

        private Guid _id = Guid.NewGuid();

        protected string PopoverClass =>
           new CssBuilder("mud-popover")
            .AddClass("mud-popover-open", Open)
            .AddClass($"mud-popover-{ConvertDirection(Direction).ToDescriptionString()}")
            .AddClass("mud-popover-offset-y", OffsetY)
            .AddClass("mud-popover-offset-x", OffsetX)
            .AddClass("mud-popover-relative-width ", RelativeWidth)
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

        private Direction ConvertDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Start => RightToLeft ? Direction.Right : Direction.Left,
                Direction.End => RightToLeft ? Direction.Left : Direction.Right,
                _ => direction
            };
        }

        [CascadingParameter] public bool RightToLeft { get; set; }

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
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the popover will have the same width at its parent element, default to false
        /// </summary>
        [Parameter] public bool RelativeWidth { get; set; } = false;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender == true)
            {
                _connected = true;
                await JsInterop.InvokeVoidAsync("mudPopover.connect", _id);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async ValueTask DisposeAsync()
        {
            if(_connected == false) { return; }

            await JsInterop.InvokeVoidAsync("mudPopover.disconnect", _id);
        }
    }
}

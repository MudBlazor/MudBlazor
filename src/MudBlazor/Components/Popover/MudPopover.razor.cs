using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudPopover : MudPopoverBase
    {
        protected internal override string PopoverClass =>
            new CssBuilder("mud-popover")
                .AddClass($"mud-popover-fixed", Fixed)
                .AddClass($"mud-popover-open", Open)
                .AddClass($"mud-popover-{TransformOrigin.ToDescriptionString()}")
                .AddClass($"mud-popover-anchor-{AnchorOrigin.ToDescriptionString()}")
                .AddClass($"mud-popover-overflow-{OverflowBehavior.ToDescriptionString()}")
                .AddClass($"mud-popover-relative-width", RelativeWidth)
                .AddClass($"mud-paper", Paper)
                .AddClass($"mud-paper-square", Paper && Square)
                .AddClass($"mud-elevation-{Elevation}", Paper && DropShadow)
                .AddClass($"overflow-y-auto", MaxHeight != null)
                .AddClass(Class)
                .Build();

        protected internal override string PopoverStyles =>
            new StyleBuilder()
                .AddStyle("transition-duration", $"{Duration}ms")
                .AddStyle("transition-delay", $"{Delay}ms")
                .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
                .AddStyle(Style)
                .Build();

        internal Direction ConvertDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Start => RightToLeft ? Direction.Right : Direction.Left,
                Direction.End => RightToLeft ? Direction.Left : Direction.Right,
                _ => direction
            };
        }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the maxheight the popover can have when open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// If true, will apply default MudPaper classes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Paper { get; set; } = true;

        /// <summary>
        /// Determines whether the popover has a drop-shadow. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int Elevation { set; get; } = MudGlobal.PopoverDefaults.Elevation;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true the popover will be fixed position instead of absolute.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Behavior)]
        public bool Fixed { get; set; }

        /// <summary>
        /// Sets the length of time that the opening transition takes to complete.
        /// </summary>
        /// <remarks>
        /// Set globally via <see cref="MudGlobal.TransitionDefaults.Duration"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Duration { get; set; } = MudGlobal.TransitionDefaults.Duration.TotalMilliseconds;

        /// <summary>
        /// Sets the amount of time in milliseconds to wait from opening the popover before beginning to perform the transition. 
        /// </summary>
        /// <remarks>
        /// Set globally via <see cref="MudGlobal.TransitionDefaults.Delay"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Delay { get; set; } = MudGlobal.TransitionDefaults.Delay.TotalMilliseconds;

        /// <summary>
        /// Set the anchor point on the element of the popover.
        /// The anchor point will determinate where the popover will be placed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Sets the intersection point if the anchor element. At this point the popover will lay above the popover. 
        /// This property in conjunction with AnchorPlacement determinate where the popover will be placed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Set the overflow behavior of a popover and controls how the element should react if there is not enough space for the element to be visible
        /// Defaults to none, which doens't apply any overflow logic
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

        /// <summary>
        /// If true, the popover will have the same width at its parent element, default to false
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool RelativeWidth { get; set; } = false;
    }
}

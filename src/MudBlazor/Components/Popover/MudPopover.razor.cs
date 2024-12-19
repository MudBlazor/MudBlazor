// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Displays content as a window over other content.
    /// </summary>
    public partial class MudPopover : MudPopoverBase
    {
        protected internal override string PopoverClass =>
            new CssBuilder("mud-popover")
                .AddClass($"mud-popover-fixed", Fixed)
                .AddClass($"mud-popover-open", Open)
                .AddClass($"mud-popover-{TransformOrigin.ToDescriptionString()}")
                .AddClass($"mud-popover-anchor-{AnchorOrigin.ToDescriptionString()}")
                .AddClass($"mud-popover-overflow-{OverflowBehavior.ToDescriptionString()}")
                .AddClass($"mud-popover-relative-width", RelativeWidth is true)
                .AddClass($"mud-popover-adaptive-width", RelativeWidth is false)
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

        /// <summary>
        /// Displays text Right-to-Left.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This property is set via the <see cref="MudRTLProvider"/>.
        /// </remarks>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the maximum height, in pixels, of this popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// Displays content within a <see cref="MudPaper"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Paper { get; set; } = true;

        /// <summary>
        /// Shows a drop shadow to help this popover stand out.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// The amount of drop shadow to apply.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudGlobal.PopoverDefaults.Elevation"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int Elevation { set; get; } = MudGlobal.PopoverDefaults.Elevation;

        /// <summary>
        /// Displays square borders around this popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the CSS <c>border-radius</c> is set to <c>0</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Displays this popover in a fixed position, even through scrolling.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>False</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Behavior)]
        public bool Fixed { get; set; }

        /// <summary>
        /// The length of time that the opening transition takes to complete.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudGlobal.TransitionDefaults.Duration"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Duration { get; set; } = MudGlobal.TransitionDefaults.Duration.TotalMilliseconds;

        /// <summary>
        /// The amount of time, in milliseconds, from opening the popover to beginning the transition. 
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudGlobal.TransitionDefaults.Delay"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Delay { get; set; } = MudGlobal.TransitionDefaults.Delay.TotalMilliseconds;

        /// <summary>
        /// The location this popover will appear relative to its parent container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.  Use <see cref="TransformOrigin"/> to control the direction of the popover from this point.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// The direction this popover will appear relative to the <see cref="Origin"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// The behavior applied when there is not enough space for this popover to be visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="OverflowBehavior.FlipOnOpen"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

        /// <summary>
        /// Determines the width of this popover in relation the parent container.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <c>null</c>. </para>
        /// <para>When <c>true</c>, restricts the max-width of the component to the width of the parent container</para>
        /// <para>When <c>false</c>, restricts the min-width of the component to the width of the parent container</para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool? RelativeWidth { get; set; }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Displays items in chronological order.
    /// </summary>
    /// <seealso cref="MudTimelineItem"/>
    public partial class MudTimeline : MudBaseItemsControl<MudTimelineItem>
    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass($"mud-timeline-{TimelineOrientation.ToDescriptionString()}")
                .AddClass($"mud-timeline-position-{ConvertTimelinePosition().ToDescriptionString()}")
                .AddClass($"mud-timeline-reverse", Reverse && TimelinePosition == TimelinePosition.Alternate)
                .AddClass($"mud-timeline-align-{TimelineAlign.ToDescriptionString()}")
                .AddClass($"mud-timeline-modifiers", Modifiers)
                .AddClass($"mud-timeline-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The orientation of the timeline and its items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimelineOrientation.Vertical"/>.<br />
        /// When set to <see cref="TimelineOrientation.Vertical"/>, <see cref="TimelinePosition"/> can be set to <c>Left</c>, <c>Right</c>, <c>Alternate</c>, <c>Start</c>, or <c>End</c>.<br />
        /// When set to <see cref="TimelineOrientation.Horizontal"/>, <see cref="TimelinePosition"/> can be set to <c>Top</c>, <c>Bottom</c>, or <c>Alternate</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineOrientation TimelineOrientation { get; set; } = TimelineOrientation.Vertical;

        /// <summary>
        /// The position the timeline and how its items are displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimelinePosition.Alternate"/>.<br />
        /// Can be set to <c>Left</c>, <c>Right</c>, <c>Alternate</c>, <c>Start</c>, or <c>End</c> when <see cref="TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.<br />
        /// Can be set to <c>Top</c>, <c>Bottom</c>, or <c>Alternate</c> when <see cref="TimelineOrientation"/> is <see cref="TimelineOrientation.Horizontal"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelinePosition TimelinePosition { get; set; } = TimelinePosition.Alternate;

        /// <summary>
        /// The position of each item's dot relative to its text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimelineAlign.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineAlign TimelineAlign { get; set; } = TimelineAlign.Default;

        /// <summary>
        /// Reverses the order of items when <see cref="TimelinePosition"/> is <see cref="TimelinePosition.Alternate"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public bool Reverse { get; set; } = false;

        /// <summary>
        /// Enables modifiers for items, such as adding a caret for a <see cref="MudCard"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public bool Modifiers { get; set; } = true;

        private TimelinePosition ConvertTimelinePosition()
        {
            if (TimelineOrientation == TimelineOrientation.Vertical)
            {
                return TimelinePosition switch
                {
                    TimelinePosition.Left => RightToLeft ? TimelinePosition.End : TimelinePosition.Start,
                    TimelinePosition.Right => RightToLeft ? TimelinePosition.Start : TimelinePosition.End,
                    TimelinePosition.Top => TimelinePosition.Alternate,
                    TimelinePosition.Bottom => TimelinePosition.Alternate,
                    _ => TimelinePosition
                };
            }

            return TimelinePosition switch
            {
                TimelinePosition.Start => TimelinePosition.Alternate,
                TimelinePosition.Left => TimelinePosition.Alternate,
                TimelinePosition.Right => TimelinePosition.Alternate,
                TimelinePosition.End => TimelinePosition.Alternate,
                _ => TimelinePosition
            };
        }
    }
}

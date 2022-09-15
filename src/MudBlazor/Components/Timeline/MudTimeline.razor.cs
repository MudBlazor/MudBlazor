// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimeline : MudBaseItemsControl<MudTimelineItem>
    {
        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the orientation of the timeline and its timeline items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineOrientation TimelineOrientation { get; set; } = TimelineOrientation.Vertical;

        /// <summary>
        /// The position the timeline itself and how the timeline items should be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelinePosition TimelinePosition { get; set; } = TimelinePosition.Alternate;

        /// <summary>
        /// Aligns the dot and any item modifiers is changed, in default mode they are centered to the item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineAlign TimelineAlign { get; set; } = TimelineAlign.Default;

        /// <summary>
        /// Reverse the order of TimelineItems when TimelinePosition is set to Alternate.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public bool Reverse { get; set; } = false;

        /// <summary>
        /// If true, disables all TimelineItem modifiers, like adding a caret to a MudCard.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public bool DisableModifiers { get; set; } = false;

        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass($"mud-timeline-{TimelineOrientation.ToDescriptionString()}")
                .AddClass($"mud-timeline-position-{ConvertTimelinePosition().ToDescriptionString()}")
                .AddClass($"mud-timeline-reverse", Reverse && TimelinePosition == TimelinePosition.Alternate)
                .AddClass($"mud-timeline-align-{TimelineAlign.ToDescriptionString()}")
                .AddClass($"mud-timeline-modifiers", !DisableModifiers)
                .AddClass($"mud-timeline-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

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
            else
            {
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
}

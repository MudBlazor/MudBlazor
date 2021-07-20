// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimeline : MudBaseItemsControl<MudTimelineItem>
    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass($"mud-timeline-{GetTimelineDirection()}")
                .AddClass($"mud-timeline-mode-{ConvertTimelineMode(TimelineMode).ToDescriptionString()}")
                .AddClass("mud-timeline-alternate", !Reverse && Alternate && TimelineMode == TimelineMode.Vertical || !Reverse && Alternate && TimelineMode == TimelineMode.Horizontal)
                .AddClass("mud-timeline-reverse", Reverse && Alternate && TimelineMode == TimelineMode.Vertical || Reverse && Alternate && TimelineMode == TimelineMode.Horizontal)
                .AddClass("mud-timeline-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        private string GetTimelineDirection()
        {
            if (TimelineMode == TimelineMode.Horizontal || TimelineMode == TimelineMode.Top || TimelineMode == TimelineMode.Bottom)
            {
                return "horizontal";
            }
            else
            {
                return "vertical";
            }
        }
        private TimelineMode ConvertTimelineMode(TimelineMode timelineMode)
        {
            return timelineMode switch
            {
                TimelineMode.Left => RightToLeft ? TimelineMode.End : TimelineMode.Start,
                TimelineMode.Right => RightToLeft ? TimelineMode.Start : TimelineMode.End,
                _ => timelineMode
            };
        }
        
        [CascadingParameter] public bool RightToLeft { get; set; }

        /// <summary>
        /// Displays the TimelineItems on alternating sides if TimelineMode is set to Vertical or Horizontal.
        /// </summary>
        [Parameter] public bool Alternate { get; set; } = true;

        /// <summary>
        /// Reverse the order of the TimelineItems if Alternating is used.
        /// </summary>
        [Parameter] public bool Reverse { get; set; }

        /// <summary>
        /// The visual mode of the Timeline and how it will display itself and its items.
        /// </summary>
        [Parameter] public TimelineMode TimelineMode { get; set; } = TimelineMode.Vertical;


    }
}

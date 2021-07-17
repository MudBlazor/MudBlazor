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
                .AddClass($"mud-timeline-mode-{ConvertTimelineMode(TimelineMode).ToDescriptionString()}")
                .AddClass("mud-timeline-rtl", RightToLeft)
                .AddClass(Class)
                .Build();
        
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
        /// Sets the position of all timelineitems, left and right on every second or all left or right.
        /// </summary>
        [Parameter] public TimelineMode TimelineMode { get; set; } = TimelineMode.Default;


    }
}

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
    public partial class MudTimeline : MudBaseItemsControl<MudTimelineItem>, IAsyncDisposable

    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass($"mud-timeline-mode-{TimelineMode.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Sets the position of all timelineitems, left and right on every second or all left or right.
        /// </summary>
        [Parameter] public TimelineMode TimelineMode { get; set; } = TimelineMode.Default;

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                //TODO
                await Task.Delay(1);
            }
        }

    }
}

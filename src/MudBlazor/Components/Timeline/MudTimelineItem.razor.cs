// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimelineItem : MudComponentBase, IDisposable

    {
        protected string Classnames =>
            new CssBuilder("mud-timeline-item")
                .AddClass(Class)
                .Build();

        protected string DotClassnames =>
        new CssBuilder("mud-timeline-item-dot")
          .AddClass($"mud-timeline-dot-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-elevation-{Elevation.ToString()}")
        .Build();

        protected string DotInnerClassnames =>
        new CssBuilder("mud-timeline-item-dot-inner")
          .AddClass($"mud-timeline-dot-fill", FillDot)
          .AddClass($"mud-{Color.ToDescriptionString()}")
        .Build();

        [CascadingParameter] protected internal MudBaseItemsControl<MudTimelineItem> Parent { get; set; }

        /// <summary>
        /// Dot Icon
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Color of the dot.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Size of the dot.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Elevation of the dot. The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// If true, dot will be filled with one color.
        /// </summary>
        [Parameter] public bool FillDot { get; set; }

        /// <summary>
        /// If true, dot will not be displayed.
        /// </summary>
        [Parameter] public bool HideDot { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public void Dispose()
        {
            Parent?.Items.Remove(this);
        }


    }
}

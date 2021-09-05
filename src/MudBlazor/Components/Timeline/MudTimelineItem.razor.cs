﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimelineItem : MudComponentBase, IDisposable

    {
        protected string Classnames =>
            new CssBuilder("mud-timeline-item")
                .AddClass($"mud-timeline-item-{TimelineAlign.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        protected string DotClassnames =>
        new CssBuilder("mud-timeline-item-dot")
          .AddClass($"mud-timeline-dot-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-elevation-{Elevation.ToString()}")
        .Build();

        protected string DotInnerClassnames =>
        new CssBuilder("mud-timeline-item-dot-inner")
          .AddClass($"mud-timeline-dot-fill", Variant == Variant.Filled)
          .AddClass($"mud-timeline-dot-{Color.ToDescriptionString()}")
        .Build();

        [CascadingParameter] protected internal MudBaseItemsControl<MudTimelineItem> Parent { get; set; }

        /// <summary>
        /// Dot Icon
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Variant of the dot.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Outlined;

        /// <summary>
        /// Color of the dot.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Size of the dot.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// Elevation of the dot. The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// Overrides Timeline Parents default sorting method in Default and Reverse mode.
        /// </summary>
        [Parameter] public TimelineAlign TimelineAlign { get; set; }

        /// <summary>
        /// If true, dot will not be displayed.
        /// </summary>
        [Parameter] public bool HideDot { get; set; }

        /// <summary>
        /// If used renders child content of the ItemOpposite.
        /// </summary>
        [Parameter] public RenderFragment ItemOpposite { get; set; }

        /// <summary>
        /// If used renders child content of the ItemContent.
        /// </summary>
        [Parameter] public RenderFragment ItemContent { get; set; }

        /// <summary>
        /// If used renders child content of the ItemDot.
        /// </summary>
        [Parameter] public RenderFragment ItemDot { get; set; }

        /// <summary>
        /// Optional child content if no other RenderFragments is used.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override Task OnInitializedAsync()
        {
            Parent?.Items.Add(this);
            return Task.CompletedTask;
        }

        private void Select()
        {
            var myIndex = Parent?.Items.IndexOf(this);
            Parent?.MoveTo(myIndex ?? 0);
        }

        public void Dispose()
        {
            Parent?.Items.Remove(this);
        }


    }
}

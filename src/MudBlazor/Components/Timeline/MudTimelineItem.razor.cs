// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimelineItem : MudComponentBase, IDisposable

    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass($"mud-elevation-{Elevation}", Elevation != 0)
                .AddClass(Class)
                .Build();

        [CascadingParameter] protected internal MudBaseItemsControl<MudTimelineItem> Parent { get; set; }
        [CascadingParameter] public bool RightToLeft { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// TimeLineItem's Alignment on main TimeLine component
        /// </summary>
        [Parameter] public Align Align { get; set; } = Align.Left;

        /// <summary>
        /// Icon for the TimeLineItem
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Color for the TimeLineItem symbol
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

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

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
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
                .AddClass($"mud-elevation-{Elevation}")
                .Build();

        protected string DotInnerClassnames =>
            new CssBuilder("mud-timeline-item-dot-inner")
                .AddClass($"mud-timeline-dot-fill", Variant == Variant.Filled)
                .AddClass($"mud-timeline-dot-{Color.ToDescriptionString()}")
                .Build();

        [CascadingParameter]
        protected internal MudBaseItemsControl<MudTimelineItem>? Parent { get; set; }

        /// <summary>
        /// Dot Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public string? Icon { get; set; }

        /// <summary>
        /// Variant of the dot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Variant Variant { get; set; } = Variant.Outlined;

        /// <summary>
        /// User styles, applied to the lineItem dot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public string? DotStyle { get; set; }

        /// <summary>
        /// Color of the dot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Size of the dot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// Elevation of the dot. The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Overrides Timeline Parents default sorting method in Default and Reverse mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineAlign TimelineAlign { get; set; }

        /// <summary>
        /// If true, dot will not be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public bool HideDot { get; set; }

        /// <summary>
        /// If used renders child content of the ItemOpposite.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ItemOpposite { get; set; }

        /// <summary>
        /// If used renders child content of the ItemContent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ItemContent { get; set; }

        /// <summary>
        /// If used renders child content of the ItemDot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public RenderFragment? ItemDot { get; set; }

        /// <summary>
        /// Optional child content if no other RenderFragments is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ChildContent { get; set; }

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

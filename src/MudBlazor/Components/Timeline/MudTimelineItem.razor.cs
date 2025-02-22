// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A chronological item displayed as part of a <see cref="MudTimeline"/>
    /// </summary>
    /// <seealso cref="MudTimeline"/>
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
        /// (Obsolete) The icon displayed for the dot.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public string? Icon { get; set; }

        /// <summary>
        /// The display variant for the dot.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Outlined"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Variant Variant { get; set; } = Variant.Outlined;

        /// <summary>
        /// The CSS styles applied to the dot.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Styles such as <c>background-color</c> can be applied (e.g. <c>background-color:red;</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public string? DotStyle { get; set; }

        /// <summary>
        /// The color of the dot.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the dot.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Small"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// The size of the dot's drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>. A higher number creates a heavier drop shadow. Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Overrides <see cref="MudTimeline.TimelineAlign"/> with a custom value.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimelineAlign.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public TimelineAlign TimelineAlign { get; set; }

        /// <summary>
        /// Hides the dot for this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public bool HideDot { get; set; }

        /// <summary>
        /// The custom content for the opposite side of this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ItemOpposite { get; set; }

        /// <summary>
        /// The custom content for this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Only applies if <see cref="ChildContent"/> is <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ItemContent { get; set; }

        /// <summary>
        /// The custom content for the dot.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Dot)]
        public RenderFragment? ItemDot { get; set; }

        /// <summary>
        /// The custom content for the entire item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When set, <see cref="ItemContent"/> will not be displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Timeline.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <inheritdoc />
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

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            Parent?.Items.Remove(this);
        }
    }
}

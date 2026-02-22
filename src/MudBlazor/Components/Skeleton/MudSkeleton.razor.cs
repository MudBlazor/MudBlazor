using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// Displays a placeholder preview of content before the data gets loaded, reducing load-time frustration.
    /// </summary>
    public partial class MudSkeleton : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-skeleton")
                .AddClass($"mud-skeleton-{SkeletonType.ToStringFast(true)}")
                .AddClass($"mud-skeleton-{Animation.ToStringFast(true)}")
                .AddClass(Class)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("height", $"{Height}", !string.IsNullOrEmpty(Height))
                .AddStyle("width", $"{Width}", !string.IsNullOrEmpty(Width))
                .AddStyle(Style)
                .Build();

        /// <summary>
        /// The width of this skeleton.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values can be in pixels (e.g. <c>"300px"</c>) or percentages (e.g. <c>"30%"</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public string? Width { set; get; }

        /// <summary>
        /// The height of this skeleton.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values can be in pixels (e.g. <c>"300px"</c>) or percentages (e.g. <c>"30%"</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public string? Height { set; get; }

        /// <summary>
        /// The shape of this skeleton.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SkeletonType.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public SkeletonType SkeletonType { set; get; } = SkeletonType.Text;

        /// <summary>
        /// The type of animation to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Animation.Pulse"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public Animation Animation { set; get; } = Animation.Pulse;
    }
}

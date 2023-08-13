using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-icon-root")
                .AddClass("mud-icon-default", Color == Color.Default)
                .AddClass("mud-svg-icon", !string.IsNullOrEmpty(Icon) && Icon.Trim().StartsWith("<"))
                .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default && Color != Color.Inherit)
                .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Icon to be used can either be svg paths for font icons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// The viewbox size of an svg element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string ViewBox { get; set; } = "0 0 24 24";

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}

using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-icon-root")
                .AddClass("mud-icon-default", _iconProperties.HasDefaultColor())
                .AddClass("mud-svg-icon", _iconProperties.IsSvg())
                .AddClass($"mud-{_iconProperties.Color.ToDescriptionString()}-text", _iconProperties.HasCustomColor())
                .AddClass($"mud-icon-size-{_iconProperties.Size.ToDescriptionString()}")
                .AddClass(_iconProperties.Class, _iconProperties.HasClass())
                .Build();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (IconProperties is not null)
            {
                _iconProperties = IconProperties;
                
                // Backwards compatibility

                if (_iconProperties.HasIcon()) Icon = _iconProperties.Icon;
                if (_iconProperties.HasTitle()) Title = _iconProperties.Title;
                if (_iconProperties.HasStyle()) Style = _iconProperties.Style;
            }
            else
            {
                _iconProperties.Icon = Icon;
                _iconProperties.Title = Title;
                _iconProperties.Size = Size;
                _iconProperties.Color = Color;
                _iconProperties.Style = Style;
                _iconProperties.ViewBox = ViewBox;
            }
        }


        IconProperties _iconProperties = new();

        /// <summary>
        /// The icon properties.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public IconProperties? IconProperties { get; set; }

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

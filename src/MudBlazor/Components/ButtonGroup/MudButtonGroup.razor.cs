using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudButtonGroup : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-button-group-root")
                .AddClass($"mud-button-group-override-styles", OverrideStyles)
                .AddClass($"mud-button-group-{Variant.ToDescriptionString()}")
                .AddClass($"mud-button-group-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
                .AddClass($"mud-button-group-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
                .AddClass("mud-button-group-vertical", VerticalAlign)
                .AddClass("mud-button-group-horizontal", !VerticalAlign)
                .AddClass("mud-button-group-disable-elevation", DisableElevation)
                .AddClass("mud-button-group-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// If true, the button group will override the styles of the individual buttons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool OverrideStyles { get; set; } = true;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If true, the button group will be displayed vertically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool VerticalAlign { get; set; } = false;

        /// <summary>
        /// If true, no drop-shadow will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool DisableElevation { get; set; } = false;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;
    }
}

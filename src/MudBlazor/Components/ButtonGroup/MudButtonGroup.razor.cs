using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a group of connected <see cref="MudButton"/> components.
    /// </summary>
    public partial class MudButtonGroup : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-button-group-root")
            .AddClass($"mud-button-group-override-styles", OverrideStyles)
            .AddClass($"mud-button-group-{Variant.ToDescriptionString()}")
            .AddClass($"mud-button-group-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
            .AddClass($"mud-button-group-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
            .AddClass("mud-button-group-vertical", Vertical)
            .AddClass("mud-button-group-horizontal", !Vertical)
            .AddClass("mud-button-group-disable-elevation", !DropShadow)
            .AddClass("mud-button-group-rtl", RightToLeft)
            .AddClass("mud-width-full", FullWidth)
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Overrides individual button styles with this group's style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, the button styles are defined by this group.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool OverrideStyles { get; set; } = true;

        /// <summary>
        /// The custom content within this group.
        /// </summary>
        /// <remarks>
        /// This property allows for custom content to displayed inside of the group, but it is not required.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Displays buttons vertically.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, buttons will be displayed vertically, otherwise horizontally.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool Vertical { get; set; }

        /// <summary>
        /// Displays a shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// The color of all buttons in this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default" />.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of all buttons in the group.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The display variant of all buttons in the group.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.  Other supported values are <see cref="Variant.Outlined"/> and <see cref="Variant.Filled"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the button group will take up 100% of available width.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ButtonGroup.Appearance)]
        public bool FullWidth { get; set; }
    }
}

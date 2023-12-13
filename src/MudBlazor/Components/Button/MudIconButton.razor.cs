using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudIconButton : MudBaseButton
    {
        protected string Classname =>
            new CssBuilder("mud-button-root mud-icon-button")
                .AddClass("mud-button", when: AsButton)
                .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", !AsButton && Color != Color.Default)
                .AddClass($"mud-button-{Variant.ToDescriptionString()}", AsButton)
                .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}", AsButton)
                .AddClass($"mud-button-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}", AsButton)
                .AddClass($"mud-ripple", !DisableRipple)
                .AddClass($"mud-ripple-icon", !DisableRipple && !AsButton)
                .AddClass($"mud-icon-button-size-{Size.ToDescriptionString()}", when: () => Size != Size.Medium)
                .AddClass($"mud-icon-button-edge-{Edge.ToDescriptionString()}", when: () => Edge != Edge.False)
                .AddClass($"mud-button-disable-elevation", DisableElevation)
                .AddClass(Class)
                .Build();

        protected bool AsButton => Variant != Variant.Text;

        /// <summary>
        /// The Icon that will be used in the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// If set uses a negative margin.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Edge Edge { get; set; }

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Child content of component, only shows if Icon is null or Empty.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}

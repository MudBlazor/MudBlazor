using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudButton : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-button")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-width-full", FullWidth)
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass($"mud-button-disable-elevation", DisableElevation)
          .AddClass(Class)
        .Build();

        protected string StartIconClass =>
        new CssBuilder("mud-button-icon-start")
          .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
          .AddClass(IconClass)
        .Build();

        protected string EndIconClass =>
        new CssBuilder("mud-button-icon-end")
          .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
          .AddClass(IconClass)
        .Build();

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string StartIcon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the icon. When null, the value of Size is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size? IconSize { get; set; }

        /// <summary>
        /// Icon class names, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string IconClass { get; set; }

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
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the button will take up 100% of available width.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment ChildContent { get; set; }

    }
}

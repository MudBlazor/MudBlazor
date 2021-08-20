using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudFab : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-fab")
          .AddClass($"mud-fab-extended", !string.IsNullOrEmpty(Label))
          .AddClass($"mud-fab-{Color.ToDescriptionString()}")
          .AddClass($"mud-fab-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-ripple", !DisableRipple && !Disabled)
          .AddClass($"mud-fab-disable-elevation", DisableElevation)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// If applied Icon will be added to the component.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the icon.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter] public string Label { get; set; }

    }
}

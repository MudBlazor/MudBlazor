using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSwitch<T> : MudBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("mud-switch")
            .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
        .Build();
        protected string SwitchClassname =>
        new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
            .AddClass($"mud-ripple mud-ripple-switch", !DisableRipple)
            .AddClass($"mud-switch-{Color.ToDescriptionString()}")
            .AddClass($"mud-switch-disabled", Disabled)
            .AddClass($"mud-checked", BoolValue)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

    }
}

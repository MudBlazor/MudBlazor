using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSwitch<T> : MudBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("mud-switch")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
          .AddClass(Class)
        .Build();
        protected string SwitchClassname =>
        new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
            .AddClass($"mud-ripple mud-ripple-switch", !DisableRipple)
            .AddClass($"mud-switch-{Color.ToDescriptionString()}")
            .AddClass($"mud-switch-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
            .AddClass($"mud-checked", BoolValue)
        .Build();

        protected string SpanClassname =>
        new CssBuilder("mud-switch-span mud-flip-x-rtl")
            .AddClass($"mud-switch-span-{Size.ToDescriptionString()}")
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter] public string Label { get; set; }

        [Parameter] public string SwitchIcon { get; set; } = Icons.Material.Filled.Done;

        [Parameter] public string SwitchIconOff { get; set; }

        [Parameter] public Size Size { get; set; } = Size.Small;

        [Parameter] public bool Progress { get; set; } = false;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        private string GetIcon()
        {
            if (string.IsNullOrEmpty(SwitchIcon) && string.IsNullOrEmpty(SwitchIconOff))
            {
                return "";
            }
            else if (string.IsNullOrEmpty(SwitchIconOff))
            {
                return SwitchIcon;
            }
            else
            {
                if (BoolValue == true)
                {
                    return SwitchIcon;
                }
                else
                {
                    return SwitchIconOff;
                }
            }
        }

        protected void HandleKeyDown(KeyboardEventArgs obj)
        {
            //Space key works by default, so we didn't write it again.
            if (Disabled || ReadOnly)
                return;
            switch (obj.Key)
            {
                case "ArrowLeft":
                case "Escape":
                    SetBoolValueAsync(false);
                    break;
                case "ArrowRight":
                case "Enter":
                case "NumpadEnter":
                    SetBoolValueAsync(true);
                    break;
            }
        }
    }
}

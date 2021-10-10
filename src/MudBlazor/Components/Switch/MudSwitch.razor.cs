﻿using Microsoft.AspNetCore.Components;
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
            .AddClass($"mud-ripple mud-ripple-switch", !DisableRipple && !ReadOnly)
            .AddClass($"mud-switch-{Color.ToDescriptionString()}")
            .AddClass($"mud-switch-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
            .AddClass($"mud-checked", BoolValue)
        .Build();

        protected string SpanClassname =>
        new CssBuilder("mud-switch-span mud-flip-x-rtl")
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
        /// Shows an icon on Switch's thumb.
        /// </summary>
        [Parameter] public string ThumbIcon { get; set; }

        /// <summary>
        /// The color of the thumb icon. Supports the theme colors.
        /// </summary>
        [Parameter] public Color ThumbIconColor { get; set; } = Color.Info;

        /// <summary>
        /// Shows an icon when thumb is off. Only works when there is a ThumbIcon.
        /// </summary>
        [Parameter] public string ThumbIconOff { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        private string GetThumbIcon()
        {
            if (string.IsNullOrEmpty(ThumbIcon) && string.IsNullOrEmpty(ThumbIconOff))
            {
                return "";
            }
            else if (string.IsNullOrEmpty(ThumbIconOff))
            {
                return ThumbIcon;
            }
            else
            {
                if (BoolValue == true)
                {
                    return ThumbIcon;
                }
                else
                {
                    return ThumbIconOff;
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

using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCheckBox<T> : MudBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
        .Build();

        protected string CheckBoxClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-checkbox-{Color.ToDescriptionString()}")
            .AddClass($"mud-ripple mud-ripple-checkbox", !DisableRipple)
            .AddClass($"mud-disabled", Disabled)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom checked icon, leave null for default.
        /// </summary>
        [Parameter]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon, leave null for default.
        /// </summary>
        [Parameter]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon, leave null for default.
        /// </summary>
        [Parameter]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        private string GetIcon()
        {
            if (BoolValue == true)
            {
                return CheckedIcon;
            }

            if (BoolValue == false)
            {
                return UncheckedIcon;
            }

            return IndeterminateIcon;
        }
    }
}

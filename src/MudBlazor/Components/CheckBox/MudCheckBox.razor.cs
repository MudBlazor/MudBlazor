using System.Threading.Tasks;
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
            .AddClass($"mud-checkbox-dense", Dense)
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
        /// If true, compact padding will be applied.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

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

        /// <summary>
        /// Define if the checkbox can cycle again through indeterminate status.
        /// </summary>
        [Parameter] public bool TriState { get; set; }

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

        protected override Task OnChange(ChangeEventArgs args)
        {
            Touched = true;

            // Apply only when TriState parameter is set to true and T is bool?
            if (TriState && typeof(T) == typeof(bool?))
            {
                // The cycle is forced with the following steps: true, false, indeterminate, true, false, indeterminate...
                if (!((bool?)(object)_value).HasValue)
                {
                    return SetBoolValueAsync(true);
                }
                else
                {
                    return ((bool?)(object)_value).Value ? SetBoolValueAsync(false) : SetBoolValueAsync(default);
                }
            }
            else
            {
                return SetBoolValueAsync((bool?)args.Value);
            }
        }
    }
}

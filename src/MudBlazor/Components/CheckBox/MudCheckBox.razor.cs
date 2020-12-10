using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;

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
            .AddClass($"mud-ripple-checkbox", !DisableRipple)
            .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
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
        /// If true, the checkbox will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom checked icon, leave null for default.
        /// </summary>
        [Parameter]
        public string CheckedIcon { get; set; } = null;

        protected string _checkedIcon => CheckedIcon ??
                                         "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";

        /// <summary>
        /// Custom unchecked icon, leave null for default.
        /// </summary>
        [Parameter]
        public string UncheckedIcon { get; set; } = null;
        
        protected string _uncheckedIcon => UncheckedIcon ??
                                           "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";

        /// <summary>
        /// Custom indeterminate icon, leave null for default.
        /// </summary>
        [Parameter]
        public string IndeterminateIcon { get; set; } = null;

        protected string _indeterminateIcon => IndeterminateIcon ??
                                           "M17,13H7V11H17M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3Z";

    }
}

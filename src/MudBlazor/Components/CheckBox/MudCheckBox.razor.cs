using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Base;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public partial class MudCheckBox<T> : MudFormComponent<T>
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
        /// Fired when Checked changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> CheckedChanged { get; set; }

        private BoolConverter<T> _boolConverter = new BoolConverter<T>();

        protected bool? BoolValue
        {
            get => _boolConverter.Set(_value);
            set => Checked = _boolConverter.Get(value);
        }

        /// <summary>
        /// The state of the checkbox
        /// </summary>
        [Parameter] public T Checked
        {
            get => _value;
            set
            {
                if (object.Equals(value, _value))
                    return;
                _value = value;
                CheckedChanged.InvokeAsync(value);
                ValidateValue(value);
                EditFormValidate();
            }
        }

        protected override Task OnInitializedAsync()
        {
            if (_boolConverter != null)
                _boolConverter.OnError = OnConversionError;
            return base.OnInitializedAsync();
        }
    }
}

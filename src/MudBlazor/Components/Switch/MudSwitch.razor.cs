using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudSwitch<T> : MudComponentBase
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
            .AddClass($"mud-checked", _value)
          .AddClass(Class)
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

        /// <summary>
        /// If true, the switch will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Fired when Checked changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> CheckedChanged { get; set; }

        private T _checked;

        private BoolConverter<T> _boolConverter = new BoolConverter<T>();

        private bool? _value
        {
            get => _boolConverter.Set(_checked);
            set => Checked = _boolConverter.Get(value);
        }

        /// <summary>
        /// The state of the switch
        /// </summary>
        [Parameter]
        public T Checked
        {
            get => _checked;
            set
            {
                if (object.Equals(value, _checked))
                    return;
                _checked = value;
                CheckedChanged.InvokeAsync(value);
            }
        }
    }
}

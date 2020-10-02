using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudSwitch : MudComponentBase
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
            .AddClass($"mud-checked", Checked)
          .AddClass(Class)
        .Build();

        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public string Label { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool Disabled { get; set; }

        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

        private bool _checked;
        [Parameter] public bool Checked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    CheckedChanged.InvokeAsync(value);
                }
            }   
        }
    }
}

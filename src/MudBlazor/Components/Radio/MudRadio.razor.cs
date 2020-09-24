using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudRadio : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-ripple mud-ripple-switch", !DisableRipple)
            .AddClass($"mud-radio-color-{Color.ToDescriptionString()}")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-checked", Checked)
          .AddClass(Class)
        .Build();

        protected string RadioIconsClassNames =>
        new CssBuilder("mud-radio-icons")
            .AddClass($"mud-checked", Checked)
            .Build();

        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public string Label { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
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

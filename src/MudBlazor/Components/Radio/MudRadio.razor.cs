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
        [CascadingParameter] public MudRadioGroup RadioGroup { get; set; }

        protected string Classname =>
        new CssBuilder("mud-radio")
            .AddClass($"mud-radio-label-placement-{Placement.ToDescriptionString()}", when: () => Placement != Placement.End)
          .AddClass(Class)
        .Build();

        protected string ButtonClassname =>
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
        [Parameter] public Placement Placement { get; set; } = Placement.End;
        [Parameter] public string Label { get; set; }
        [Parameter] public string Option { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _checked;
        internal bool Checked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    StateHasChanged();
                }
            }   
        }

        public void Select()
        {
            if (RadioGroup == null)
                return;
            RadioGroup.SetSelectedRadio(this);
        }

        private void OnValueChanged(ChangeEventArgs args)
        {
            if (RadioGroup == null)
                return;
            if ((string) args.Value == "on")
                RadioGroup.SetSelectedRadio(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (RadioGroup == null)
                return;
            RadioGroup.RegisterRadio(this);
        }

    }
}

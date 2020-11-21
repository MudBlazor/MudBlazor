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
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-radio-label-placement-{Placement.ToDescriptionString()}", when: () => Placement != Placement.End)
          .AddClass(Class)
        .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-ripple mud-ripple-radio", !DisableRipple)
            .AddClass($"mud-radio-color-{Color.ToDescriptionString()}")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-checked", Checked)
          .AddClass(Class)
        .Build();

        protected string RadioIconsClassNames =>
        new CssBuilder("mud-radio-icons")
            .AddClass($"mud-checked", Checked)
            .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The position of the label.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.End;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter] public string Label { get; set; }
        [Parameter] public string Option { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
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

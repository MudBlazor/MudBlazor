using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseCheckBox : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-icon-button mud-checkbox")
            .AddClass($"mud-checkbox-{Color.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public string Label { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Checked { get; set; }

        public void CheckboxClicked(object aChecked)
        {
            if((bool)aChecked)
            {
                Checked = true;
            }else
            {
                Checked = false;
            }
            StateHasChanged();
        }
    }
}

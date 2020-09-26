using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class ComponentBaseMudSelect : MudBaseInputText
    {
        protected string Classname =>
        new CssBuilder("mud-select mud-formcontrol")
        .AddClass("mud-formcontrol-full-width", FullWidth)
        .AddClass(Class)
       .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback<string> SelectedValue { get; set; }

        public bool isMenuOpen { get; set; }

        public async Task OnSelect(string value)
        {
            Value = value;
            StateHasChanged();
            await SelectedValue.InvokeAsync(value);
        }

        public void ShowSelect()
        {
            if(!Disabled)
            {
                isMenuOpen = !isMenuOpen;
                StateHasChanged();
            }
        }
    }
}

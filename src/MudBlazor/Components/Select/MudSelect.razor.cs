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
        new CssBuilder().AddClass(Class)
       .Build();

        [Parameter] public string Class { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback<string> OnSelected { get; set; }

        public bool isMenuOpen { get; set; }

        public async Task OnSelect(string value)
        {
            isMenuOpen = false;
            StateHasChanged();
            await OnSelected.InvokeAsync(value);
        }

        public void ShowSelect()
        {
            isMenuOpen = !isMenuOpen;
            StateHasChanged();
        }
    }
}

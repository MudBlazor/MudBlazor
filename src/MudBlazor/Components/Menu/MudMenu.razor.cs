using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class ComponentBaseMudMenu : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-menu")
        .AddClass(Class)
       .Build();

        public bool isMenuOpen { get; set; }

        [Parameter] public string Label { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public string StartIcon { get; set; }
        [Parameter] public string EndIcon { get; set; }
        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public Variant Variant { get; set; } = Variant.Text;
        [Parameter] public bool DisableElevation { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        public async Task OnClick()
        {
            isMenuOpen = false;
            StateHasChanged();
        }

        public void ShowMenu()
        {
            if(!Disabled)
            {
                isMenuOpen = !isMenuOpen;
                StateHasChanged();
            }
        }
    }
}

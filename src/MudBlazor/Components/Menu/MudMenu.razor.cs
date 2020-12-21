using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseMudMenu : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-menu")
        .AddClass(Class)
       .Build();

        public bool isOpen { get; set; }

        [Parameter] public string Label { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public string StartIcon { get; set; }
        [Parameter] public string EndIcon { get; set; }
        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, compact vertical padding will be applied to all menu items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool DisableElevation { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        public void CloseMenu()
        {
            isOpen = false;
            StateHasChanged();
        }

        public void OpenMenu()
        {
            if (Disabled)
                return;
            isOpen = false;
            StateHasChanged();
        }

        public void ToggleMenu()
        {
            if (Disabled)
                return;
            isOpen = !isOpen;
            StateHasChanged();
        }
    }
}

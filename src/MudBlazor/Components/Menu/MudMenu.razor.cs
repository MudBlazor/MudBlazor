using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMenu : MudBaseButton, IActivatable
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

        /// <summary>
        /// Place a MudButton, a MudIconButton or any other component capable of acting as an activator. This will
        /// override the standard button and all parameters which concern it.
        /// </summary>
        [Parameter] public RenderFragment ActivatorContent { get; set; }
        
        /// <summary>
        /// Add menu items here
        /// </summary>
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

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="args"></param>
        public void Activate(object activator, MouseEventArgs args)
        {
            ToggleMenu();
        }
    }
}

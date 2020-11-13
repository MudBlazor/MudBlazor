using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavLink : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-nav-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();


        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }
        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [CascadingParameter] public MudDrawer Drawer { get; set; }

        private void OnNavigation()
        {
            if (Drawer == null || !Drawer.OpenChanged.HasDelegate)
            {
                return;
            }

            Drawer.OpenChanged.InvokeAsync(false);
        }
    }
}

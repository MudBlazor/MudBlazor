using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
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

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;
        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [CascadingParameter] public MudDrawer Drawer { get; set; }

        private void OnNavigation(MouseEventArgs args)
        {
            if (Drawer != null)
            {
                Drawer.OnNavigation();
            }
        }
    }
}

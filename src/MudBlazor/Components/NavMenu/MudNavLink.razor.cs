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
        /// Font icon to use if set.
        /// </summary>
        [Parameter] public string FontIcon { get; set; }

        /// <summary>
        /// Font class to use if set.
        /// </summary>
        [Parameter] public string FontClass { get; set; }

        /// <summary>
        /// Size of icon to use if set.
        /// </summary>
        [Parameter] public Size IconSize { get; set; }

        /// <summary>
        /// Color of icon to use if set.
        /// </summary>
        [Parameter] public Color IconColor { get; set; }
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

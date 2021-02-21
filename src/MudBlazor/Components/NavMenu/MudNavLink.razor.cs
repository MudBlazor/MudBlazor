using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavLink : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-nav-item")
          .AddClass($"mud-ripple", !DisableRipple && !Disabled)
          .AddClass(Class)
          .Build();

        protected string LinkClassname =>
        new CssBuilder("mud-nav-link")
          .AddClass($"mud-nav-link-disabled", Disabled)
          .Build();

        private Dictionary<string, object> Attributes
        {
            get => Disabled ? null : new Dictionary<string, object>()
            {
                { "href", Href },
                { "target", Target },
                { "rel", !string.IsNullOrWhiteSpace(Target) ? "noopener noreferrer" : string.Empty }
            };
        }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [Parameter] public string Target { get; set; }

        [CascadingParameter] INavigationEventReceiver NavigationEventReceiver { get; set; }

        private Task HandleNavigation()
        {
            if (!Disabled && NavigationEventReceiver != null)
            {
                return NavigationEventReceiver.OnNavigation();
            }

            return Task.CompletedTask;
        }
    }
}

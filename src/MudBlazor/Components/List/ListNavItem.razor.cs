using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseListNavItem : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
    }
}

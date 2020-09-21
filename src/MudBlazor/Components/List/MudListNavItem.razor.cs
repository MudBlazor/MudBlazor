using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListNavItem : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseListSubItem : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass(Class)
        .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string Href { get; set; } = "";
        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListNested : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass(Class)
        .Build();

        protected string ClassnameItem =>
        new CssBuilder("mud-list-nav-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

        [Parameter] public string ClassItem { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected void ExpandedToggle()
        {
            Expanded = !Expanded;
        }
    }
}

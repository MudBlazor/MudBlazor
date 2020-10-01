using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListItem : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
           .AddClass("mud-list-item-button", Button)
          .AddClass("mud-list-item-gutters")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();
        [Parameter] public string Text { get; set; }
        [Parameter] public string Avatar { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public bool Inset { get; set; }

        [Parameter] public bool Button { get; set; }
    }
}

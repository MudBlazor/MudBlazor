using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListItem : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

    }
}

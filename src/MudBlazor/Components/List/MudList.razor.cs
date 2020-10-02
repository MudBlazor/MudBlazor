using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudList : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Clickable { get; set; }
        [Parameter] public bool DisablePadding { get; set; }
    }
}

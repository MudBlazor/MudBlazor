using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavMenu : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-navmenu")
          .AddClass(Class)
        .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

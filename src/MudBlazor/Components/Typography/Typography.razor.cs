using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseTypography : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass("mud-typography-gutterbottom")
          .AddClass(Class)
        .Build();

        [Parameter] public Typo Typo { get; set; } = Typo.body1;
        [Parameter] public string Class { get; set; }
        [Parameter]  public RenderFragment ChildContent { get; set; }
    }
}

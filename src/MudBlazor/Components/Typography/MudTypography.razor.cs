using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudTypography : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass("mud-typography-gutterbottom", GutterBottom)
          .AddClass($"mud-typography-align-{Align.ToDescriptionString()}", Align != Align.Inherit)
          .AddClass(Class)
        .Build();

        [Parameter] public Typo Typo { get; set; } = Typo.body1;
        [Parameter] public Align Align { get; set; } = Align.Inherit;
        [Parameter] public bool GutterBottom { get; set; } = true;
        [Parameter]  public RenderFragment ChildContent { get; set; }
    }
}

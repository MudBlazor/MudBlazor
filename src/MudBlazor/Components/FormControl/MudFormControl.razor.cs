using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudFormControl : MudComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-formcontrol")
         .AddClass(Class)
       .Build();

        [Parameter] public string Label { get; set; }
        [Parameter] public string HelperText { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseFormControl : ComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-form-control")
         .AddClass(Class)
       .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

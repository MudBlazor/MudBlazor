using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseInputLabel : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder()
         .AddClass("mud-input-label")
         .AddClass("mud-input-label-animated")
         .AddClass($"mud-input-label-{Variant.ToDescriptionString()}")
         .AddClass(Class)
       .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

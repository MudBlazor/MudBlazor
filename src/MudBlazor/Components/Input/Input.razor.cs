using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseInput : MudBaseInputTextComponent
    {
        protected string Classname =>
       new CssBuilder("mud-input-root")
         .AddClass("mud-input-underline")
         .AddClass("mud-disabled", Disabled)
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        protected string InputClassname =>
       new CssBuilder("mud-input")
         .AddClass("mud-input-underline")
         .AddClass("mud-disabled", Disabled)
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        [Parameter] public string Class { get; set; }

        protected string InputType => new CssBuilder().AddClass(Type.ToDescriptionString()).Build();
    }
}

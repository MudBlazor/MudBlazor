using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseInput : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder("mud-input")
         .AddClass($"mud-input-{Variant.ToDescriptionString()}")
         .AddClass("mud-input-underline", when: () => Variant != Variant.Outlined)
         .AddClass("mud-shrink", when: () => !string.IsNullOrEmpty(Value))
         .AddClass("mud-disabled", Disabled)
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        protected string InputClassname =>
       new CssBuilder("mud-input-root")
         .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
         .AddClass(Class)
       .Build();

        [Parameter] public string Class { get; set; }

        protected string _InputType => new CssBuilder().AddClass(InputType.ToDescriptionString()).Build();
    }
}

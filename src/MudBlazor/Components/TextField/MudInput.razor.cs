using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseMudInput : ComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-input-root")
         .AddClass("mud-input-underline")
         .AddClass("mud-disabled", Disabled)
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        [Parameter] public string Placeholder { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Value { get; set; }

        [Parameter] public bool Error { get; set; }

        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        [Parameter] public Type Type { get; set; } = Type.Text;

        protected string InputType => new CssBuilder().AddClass(Type.ToDescriptionString()).Build();
    }
}

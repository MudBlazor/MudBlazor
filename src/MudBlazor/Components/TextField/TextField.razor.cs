using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public class ComponentBaseTextField : ComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-text-field-label")
        .AddClass("mud-label-animated")
        .AddClass($"mud-input-label-{Variant.ToDescriptionString()}")
        .AddClass(Class)
       .Build();


        [Parameter] public string Label { get; set; }

        [Parameter] public string Placeholder { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Value { get; set; }

        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        [Parameter] public Type Type { get; set; } = Type.Text;

    }
}

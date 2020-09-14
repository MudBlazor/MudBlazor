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
        protected string LabelClassname =>
        new CssBuilder()
           .AddClass($"mud-input-label-shrink mud-focused", Focused)
          .AddClass(Class)
        .Build();
        protected string Classname =>
        new CssBuilder()
           .AddClass($"mud-focused", Focused)
          .AddClass(Class)
        .Build();

        [Parameter] public string Label { get; set; }

        [Parameter] public string Placeholder { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Value { get; set; }

        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        private bool Focused { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            Focused = true;
        }

    }
}

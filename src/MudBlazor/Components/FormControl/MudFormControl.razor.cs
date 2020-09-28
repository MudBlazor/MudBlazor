using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudFormControl : MudComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-formcontrol")
          .AddClass($"mud-formcontrol-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
         .AddClass("mud-formcontrol-full-width", FullWidth)
         .AddClass(Class)
       .Build();

        [Parameter] public string Label { get; set; }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public string HelperText { get; set; }
        [Parameter] public Margin Margin { get; set; } = Margin.None;
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

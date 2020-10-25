using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudInputControl : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder("mud-input-control")
          .AddClass($"mud-input-control-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
         .AddClass("mud-input-control-full-width", FullWidth)
         .AddClass(Class)
       .Build();

        protected string HelperClass =>
       new CssBuilder("mud-input-helper-text")
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ControlChild { get; set; }

        /// <summary>
        /// Should be the Input
        /// </summary>
        [Parameter] public RenderFragment ControlInput { get; set; }

    }
}

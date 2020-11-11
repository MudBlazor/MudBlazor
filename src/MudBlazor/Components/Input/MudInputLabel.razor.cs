using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudInputLabel : MudComponentBase
    {
        protected string Classname =>
       new CssBuilder()
         .AddClass("mud-input-label")
         .AddClass("mud-input-label-animated")
         .AddClass($"mud-input-label-{Variant.ToDescriptionString()}")
         .AddClass($"mud-input-label-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
         .AddClass($"mud-disabled", Disabled)
         .AddClass("mud-input-error", Error)
         .AddClass(Class)
       .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
        
        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

    }
}

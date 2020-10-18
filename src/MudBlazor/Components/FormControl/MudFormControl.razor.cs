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

        /// <summary>
        /// If string has value, label text will be added.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, the component will take up the full width of its container.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// If string has value, helpertext will be applied.
        /// </summary>
        [Parameter] public string HelperText { get; set; }
        /// <summary>
        /// 	If dense or normal, will adjust vertical spacing of this and contained components.
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;
        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

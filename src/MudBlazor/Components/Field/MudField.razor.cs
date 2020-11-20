using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudField : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-field-content")
                .AddClass($"mud-field-content-{Variant.ToDescriptionString()}")
                .AddClass($"mud-field-content-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-field-content-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
                .AddClass("mud-field-content-nopadding", when: () => InnerPadding == false)
                .AddClass("mud-shrink", when: () => !string.IsNullOrWhiteSpace(ChildContent.ToString()))
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", Error)
                .AddClass(Class)
                .Build();

        protected string InputControlClassname =>
            new CssBuilder()
                .AddClass("mud-field")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }

        /// <summary>
        /// The ErrorText that will be displayed if Error true
        /// </summary>
        [Parameter] public string ErrorText { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// If true, the field will take up the full width of its container.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the field has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// Variant can be Text, Filled or Outlined.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the inner contents padding is removed.
        /// </summary>
        [Parameter] public bool InnerPadding { get; set; } = true;

        /// <summary>
        /// If true, the field will not have an underline.
        /// </summary>
        [Parameter] public bool DisableUnderLine { get; set; }
    }
}
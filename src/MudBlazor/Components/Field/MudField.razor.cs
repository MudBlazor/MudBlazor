using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudField : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: () => !string.IsNullOrWhiteSpace(ChildContent?.ToString()))
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", Error || !string.IsNullOrEmpty(ErrorText))
                .AddClass(Class)
                .Build();

        protected string InnerClassname =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass("mud-input-slot-nopadding", when: () => InnerPadding == false)
                .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-root-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass(Class)
                .Build();

        protected string AdornmentClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .AddClass(Class)
                .Build();

        protected string InputControlClassname =>
            new CssBuilder("mud-field")
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
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter] public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter] public string AdornmentText { get; set; }

        /// <summary>
        /// Sets Start or End Adornment if not set to None.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Small;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

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
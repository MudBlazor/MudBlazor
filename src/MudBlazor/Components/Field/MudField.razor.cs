using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    //TODO Maybe can inherit from MudBaseInput?
    public partial class MudField : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass("mud-input-adorned-start", HasStartAdornment)
                .AddClass("mud-input-adorned-end", HasEndAdornment)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: () => !string.IsNullOrWhiteSpace(ChildContent?.ToString()) || HasStartAdornment)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", Error && !string.IsNullOrEmpty(ErrorText))
                .Build();

        protected string InnerClassname =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass("mud-input-slot-nopadding", when: () => InnerPadding == false)
                .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
                .AddClass("mud-input-adorned-start", HasStartAdornment)
                .AddClass("mud-input-adorned-end", HasEndAdornment)
                .AddClass($"mud-input-root-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .Build();

        protected string AdornmentStartClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-start")
                .AddClass("mud-text", !string.IsNullOrEmpty(AdornmentStartText) || !string.IsNullOrEmpty(AdornmentEndText))
                .AddClass("mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        protected string AdornmentEndClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-end")
                .AddClass("mud-text", !string.IsNullOrEmpty(AdornmentStartText) || !string.IsNullOrEmpty(AdornmentEndText))
                .AddClass("mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        protected string InputControlClassname =>
            new CssBuilder("mud-field")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Validation)]
        public bool Error { get; set; }

        /// <summary>
        /// The ErrorText that will be displayed if Error true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Validation)]
        public string? ErrorText { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? HelperText { get; set; }

        /// <summary>
        /// If true, the field will take up the full width of its container.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the field has value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Variant can be Text, Filled or Outlined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Icon that will be used at the start if specified.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentStartIcon { get; set; }

        /// <summary>
        /// Text that will be used at the start if specified, the Text overrides Icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentStartText { get; set; }
        
        /// <summary>
        /// The color of the start adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentStartColor { get; set; } = Color.Default;

        /// <summary>
        /// Button click event if set and start Adornment used.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentStartClick { get; set; }


        /// <summary>
        /// Icon that will be used at the end if specified.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentEndIcon { get; set; }

        /// <summary>
        /// Text that will be used at the end if specified, the Text overrides Icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentEndText { get; set; }

        /// <summary>
        /// The color of the end adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentEndColor { get; set; } = Color.Default;

        /// <summary>
        /// Button click event if set and end Adornment used.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentEndClick { get; set; }


        public bool HasStartAdornment =>
            !string.IsNullOrEmpty(AdornmentStartText) || !string.IsNullOrEmpty(AdornmentStartIcon);

        public bool HasEndAdornment =>
            !string.IsNullOrEmpty(AdornmentEndText) || !string.IsNullOrEmpty(AdornmentEndIcon);

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;
        
        /// <summary>
        /// If true, the inner contents padding is removed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool InnerPadding { get; set; } = true;

        /// <summary>
        /// If true, the field will not have an underline.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool DisableUnderLine { get; set; }
    }
}

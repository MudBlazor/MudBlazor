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
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: () => !string.IsNullOrWhiteSpace(ChildContent?.ToString()) || Adornment == Adornment.Start)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", Error && !string.IsNullOrEmpty(ErrorText))
                .Build();

        protected string InnerClassname =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass("mud-input-slot-nopadding", when: () => InnerPadding == false)
                .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-root-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .Build();

        protected string AdornmentClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
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
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentText { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

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

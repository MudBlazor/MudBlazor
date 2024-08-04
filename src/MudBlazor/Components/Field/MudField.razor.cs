using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    //TODO Maybe can inherit from MudBaseInput?

    /// <summary>
    /// A component similar to <see cref="MudTextField{T}"/> which supports custom content.
    /// </summary>
    public partial class MudField : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => Underline && Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: () => !string.IsNullOrWhiteSpace(ChildContent?.ToString()) || Adornment == Adornment.Start)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", Error && !string.IsNullOrEmpty(ErrorText))
                .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
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
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The content within this field.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The vertical spacing for this field.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// Typography for the field text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Typo Typo { get; set; } = Typo.input;

        /// <summary>
        /// Displays the error in <see cref="ErrorText"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Validation)]
        public bool Error { get; set; }

        /// <summary>
        /// A description of this field's error that is displayed under the field when <see cref="Error"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Field.Validation)]
        public string? ErrorText { get; set; }

        /// <summary>
        /// The text displayed below the text field.
        /// </summary>
        /// <remarks>
        /// Typically used to help the user understand what kind of input is allowed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? HelperText { get; set; }

        /// <summary>
        /// Sets the width of the field to the width of the container.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// If no value is specified, the label will be displayed in the input.  Otherwise, it will be scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// The display variant of the field.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Prevents the user from interacting with this field.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The icon displayed for the adornment.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This icon will be displayed when <see cref="Adornment"/> is <c>Start</c> or <c>End</c>, and no value for <see cref="AdornmentText"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentIcon { get; set; }

        /// <summary>
        /// The text displayed for the adornment.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This text will be displayed when <see cref="Adornment"/> is <c>Start</c> or <c>End</c>.  The <see cref="AdornmentIcon"/> property will be ignored if this property is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public string? AdornmentText { get; set; }

        /// <summary>
        /// The location of the adornment icon or text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Adornment.None"/>.  When set to <c>Start</c> or <c>End</c>, the <see cref="AdornmentText"/> will be displayed, or <see cref="AdornmentIcon"/> if no adornment text is specified.  
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// The color of <see cref="AdornmentText"/> or <see cref="AdornmentIcon"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The size of the icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Occurs when the adornment text or icon has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Displays padding for the content within this field.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool InnerPadding { get; set; } = true;

        /// <summary>
        /// Displays an underline for this field.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Field.Appearance)]
        public bool Underline { get; set; } = true;
    }
}

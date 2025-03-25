using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A base class for designing input components.
    /// </summary>
    public partial class MudInputControl : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-input-control")
                .AddClass("mud-input-required", when: () => Required)
                .AddClass($"mud-input-control-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-control-full-width", FullWidth)
                .AddClass("mud-input-error", Error)
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass(Class)
                .Build();

        protected string HelperContainer =>
            new CssBuilder("mud-input-control-helper-container")
                .AddClass("px-1", Variant == Variant.Filled)
                .AddClass("px-2", Variant == Variant.Outlined)
                .Build();

        protected string HelperClass =>
            new CssBuilder("mud-input-helper-text")
                .AddClass("mud-input-helper-onfocus", HelperTextOnFocus)
                .AddClass("mud-input-error", Error)
                .Build();

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The input component within this component.
        /// </summary>
        [Parameter]
        public RenderFragment? InputContent { get; set; }

        /// <summary>
        /// The spacing above and below this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// Displays an asterisk to indicate an input is required.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Required { get; set; }

        /// <summary>
        /// Displays the <see cref="Label"/> in an error state.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the error in <see cref="ErrorText"/> will be displayed.
        /// </remarks>
        [Parameter]
        public bool Error { get; set; }

        /// <summary>
        /// The description of the error to display when <see cref="Error"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        public string? ErrorText { get; set; }

        /// <summary>
        /// The ID that will be used by aria-describedby if <see cref="ErrorText"/> is set.
        /// </summary>
        /// <remarks>
        /// When set, the <c>aria-describedby</c> attribute is set to the ID.  
        /// </remarks>
        [Parameter]
        public string? ErrorId { get; set; }

        /// <summary>
        /// The text which describes which kind of input is expected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This text is be displayed below the text field.
        /// </remarks>
        [Parameter]
        public string? HelperText { get; set; }

        /// <summary>
        /// The ID that will be used by aria-describedby if <see cref="HelperText"/> is set.
        /// </summary>
        [Parameter]
        public string? HelperId { get; set; }

        /// <summary>
        /// Displays the <see cref="HelperText"/> only when this input has focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// The current and maximum number of characters, displayed below the text field.
        /// </summary>
        /// <remarks>
        /// Used to help users know when they are at a maximum character limit.
        /// </remarks>
        [Parameter]
        public string? CounterText { get; set; }

        /// <summary>
        /// Expands this input to the width of its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool FullWidth { get; set; }

        /// <summary>
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// When this input has no value, the label is displayed inside the text box.  Otherwise, the label is scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// The display variant for this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.  Other values include <see cref="Variant.Filled"/> and <see cref="Variant.Outlined"/>.
        /// </remarks>
        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Prevents the user from changing this input's value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// The ID of the input component related to the label specified in <see cref="Label"/>.
        /// </summary>
        [Parameter]
        public string? ForId { get; set; } = string.Empty;
    }
}

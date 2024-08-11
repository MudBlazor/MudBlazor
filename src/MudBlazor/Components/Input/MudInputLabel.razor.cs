using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A label which describes a <see cref="MudInput{T}"/> component.
    /// </summary>
    public partial class MudInputLabel : MudComponentBase
    {
        protected string Classname => new CssBuilder()
            .AddClass("mud-input-label")
            .AddClass("mud-input-label-animated")
            .AddClass($"mud-input-label-{Variant.ToDescriptionString()}")
            .AddClass($"mud-input-label-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
            .AddClass("mud-disabled", Disabled)
            .AddClass("mud-input-error", Error)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this label.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this label.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Displays this label in an error state.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Error { get; set; }

        /// <summary>
        /// The display variant of this label.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The amount of vertical spacing to apply.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// For WCAG accessibility, the ID of the input component related to this label.
        /// </summary>
        /// <remarks>
        /// Defaults to an empty string.
        /// </remarks>
        [Parameter]
        public string ForId { get; set; } = string.Empty;
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Primitive component which allows rendering any HTML element we want
    /// through the HtmlTag property
    /// </summary>
    public class MudElement : MudComponentBase
    {
        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The HTML element that will be rendered in the root by the component
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public string HtmlTag { get; set; } = "span";

        /// <summary>
        /// The title of this element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> and won't be added unless it has a value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public string? Title { get; set; }

        /// <summary>
        /// The ARIA label for this element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> and won't be added unless it has a value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// The ElementReference to bind to.
        /// Use like @bind-Ref="myRef"
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public ElementReference? Ref { get; set; }

        [Parameter]
        public EventCallback<ElementReference> RefChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool PreventDefault { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            // Open element
            builder.OpenElement(0, HtmlTag);

            // Splatted attributes
            foreach (var attribute in UserAttributes)
            {
                if (attribute.Value != null)
                    builder.AddAttribute(1, attribute.Key, attribute.Value);
            }

            // Add class and style attributes
            builder.AddAttribute(2, "class", Class);
            builder.AddAttribute(3, "style", Style);

            // Only add if not null so it won't overwrite anything.
            if (Title != null)
            {
                builder.AddAttribute(4, "title", Title);
            }

            // Only add if not null so it won't overwrite anything.
            if (AriaLabel != null)
            {
                builder.AddAttribute(5, "aria-label", AriaLabel);
            }

            // Add event attributes
            builder.AddEventStopPropagationAttribute(6, "onclick", !ClickPropagation);
            builder.AddEventPreventDefaultAttribute(7, "onclick", PreventDefault);

            // Reference capture
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(8, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            // Add child content
            builder.AddContent(9, ChildContent);

            // Close element
            builder.CloseElement();
        }
    }
}

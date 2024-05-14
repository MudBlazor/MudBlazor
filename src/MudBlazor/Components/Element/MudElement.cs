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

            // Initialize the sequence number
            var s = 0;

            // Open element
            builder.OpenElement(s++, HtmlTag);

            // Splatted attributes
            foreach (var attribute in UserAttributes)
            {
                if (attribute.Value is not null)
                    builder.AddAttribute(s++, attribute.Key, attribute.Value);
            }

            // Add class and style attributes
            builder.AddAttribute(s++, "class", Class);
            builder.AddAttribute(s++, "style", Style);

            // Add event attributes
            builder.AddEventStopPropagationAttribute(s++, "onclick", !ClickPropagation);
            builder.AddEventPreventDefaultAttribute(s++, "onclick", PreventDefault);

            // Reference capture
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(s++, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            // Add child content
            builder.AddContent(s++, ChildContent);

            // Close element
            builder.CloseElement();
        }
    }
}

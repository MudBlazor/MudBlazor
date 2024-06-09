﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A primitive component that allows rendering any HTML element specified through the <see cref="HtmlTag"/> property.
    /// </summary>
    /// <remarks>
    /// Useful for creating custom elements with dynamic tag names.
    /// </remarks>
    public class MudElement : MudComponentBase
    {
        /// <summary>
        /// The content to be rendered inside the element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Defines the HTML tag that will be rendered at the root of this component.
        /// </summary>
        /// <remarks>
        /// Default is <c>span</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public string HtmlTag { get; set; } = "span";

        /// <summary>
        /// The <see cref="ElementReference"/> to bind to.
        /// </summary>
        /// <remarks>
        /// Use like <c>@bind-Ref="myRef"</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public ElementReference? Ref { get; set; }

        /// <summary>
        /// Callback invoked when the element reference changes.
        /// </summary>
        [Parameter]
        public EventCallback<ElementReference> RefChanged { get; set; }

        /// <summary>
        /// Propagates click events beyond this element.
        /// </summary>
        /// <remarks>
        /// Default is <c>true</c>, allowing propagation.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; } = true;

        /// <summary>
        /// Prevents the default action for the <c>onclick</c> event.
        /// </summary>
        /// <remarks>
        /// Default is <c>false</c>, allowing default actions.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool PreventDefault { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            // Initialize the sequence number.
            // https://learn.microsoft.com/en-us/aspnet/core/blazor/advanced-scenarios.
            var seq = 0;

            // Open element.
            builder.OpenElement(seq++, HtmlTag);

            // Splatted attributes.
            foreach (var attribute in UserAttributes)
            {
                // Check if the attribute value is not null before adding it to the builder.
                // This avoids adding null event handlers, such as `@onmouseenter=@(Open ? HandleEnter : null)`.
                // This is useful because Blazor always adds the attribute value and creates an EventCallback in normal HTML elements.
                if (attribute.Value is not null)
                {
                    builder.AddAttribute(seq++, attribute.Key, attribute.Value);
                }
            }

            // Add class and style attributes.
            builder.AddAttribute(seq++, "class", Class);
            builder.AddAttribute(seq++, "style", Style);

            // Add event attributes.
            builder.AddEventStopPropagationAttribute(seq++, "onclick", !ClickPropagation);
            builder.AddEventPreventDefaultAttribute(seq++, "onclick", PreventDefault);

            // Capture the element reference if specified.
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(seq++, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            // Add child content.
            builder.AddContent(seq++, ChildContent);

            // Close element.
            builder.CloseElement();
        }
    }
}

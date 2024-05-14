using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A primitive component that allows rendering any HTML element specified through the HtmlTag property.
    /// </summary>
    /// <remarks>
    /// Useful for creating custom elements with dynamic tag names.
    /// </remarks>
    public class MudElement : MudComponentBase
    {
        /// <summary>
        /// Specifies the content to be rendered inside the element.
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
        /// Use like <c>@bind-Ref="myRef"</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public ElementReference? Ref { get; set; }

        /// <summary>
        /// Callback invoked when the element reference changes.
        /// </summary>
        [Parameter]
        public EventCallback<ElementReference> RefChanged { get; set; }

        /// <summary>
        /// Determines whether click events should propagate beyond this element.
        /// </summary>
        /// <remarks>
        /// Default is <c>true</c>, allowing propagation.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; } = true;

        /// <summary>
        /// Determines whether the default action for the onclick event should be prevented.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool PreventDefault { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            // Initialize the sequence number.
            var s = 0;

            // Open element.
            builder.OpenElement(s++, HtmlTag);

            // Splatted attributes.
            foreach (var attribute in UserAttributes)
            {
                // checking if the value is null, we can get rid of null event handlers
                // for example `@onmouseenter=@(Open ? HandleEnter : null)`
                // this is a powerful feature that in normal HTML elements doesn't work, because
                // Blazor adds always the attribute value and creates an EventCallback.
                if (attribute.Value is not null)
                {
                    builder.AddAttribute(s++, attribute.Key, attribute.Value);
                }
            }

            // Add class and style attributes.
            builder.AddAttribute(s++, "class", Class);
            builder.AddAttribute(s++, "style", Style);

            // Add event attributes.
            builder.AddEventStopPropagationAttribute(s++, "onclick", !ClickPropagation);
            builder.AddEventPreventDefaultAttribute(s++, "onclick", PreventDefault);

            // Capture the element reference if specified.
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(s++, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            // Add child content.
            builder.AddContent(s++, ChildContent);

            // Close element.
            builder.CloseElement();
        }
    }
}

﻿using System.Diagnostics;
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
        public bool StopOnClickPropagation { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool PreventOnClickDefault { get; set; }

        /// <summary>
        /// Calling StateHasChanged to refresh the component's state
        /// </summary>
        public void Refresh() => StateHasChanged();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            //Open
            builder.OpenElement(0, HtmlTag);

            //splatted attributes
            foreach (var attribute in UserAttributes)
            {
                // checking if the value is null, we can get rid of null event handlers
                // for example `@onmouseenter=@(IsOpen ? HandleEnter : null)`
                // this is a powerful feature that in normal HTML elements doesn't work, because
                // Blazor adds always the attribute value and creates an EventCallback
                if (attribute.Value != null)
                    builder.AddAttribute(1, attribute.Key, attribute.Value);
            }
            //Class
            builder.AddAttribute(2, "class", Class);
            //Style
            builder.AddAttribute(3, "style", Style);

            builder.AddEventStopPropagationAttribute(5, "onclick", StopOnClickPropagation);
            builder.AddEventPreventDefaultAttribute(6, "onclick", PreventOnClickDefault);

            //Reference capture
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(7, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            //Content
            builder.AddContent(10, ChildContent);

            //Close
            builder.CloseElement();
        }
    }
}

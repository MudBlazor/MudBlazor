using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    /// <summary>
    /// Primitive component which allows rendering any HTML element we want
    /// through the HtmlTag property
    /// </summary>
    public class MudElement : MudComponentBase
    {
        /// <summary>
        /// Child content
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The HTML element that will be rendered in the root by the component
        /// </summary>
        [Parameter] public string HtmlTag { get; set; }
        /// <summary>
        /// The ElementReference to bind to.
        /// Use like @bind-Ref="myRef"
        /// </summary>
        [Parameter] public ElementReference? Ref { get; set; }

        [Parameter] public EventCallback<ElementReference> RefChanged { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            //Open
            builder.OpenElement(0, HtmlTag);

            //splatted attributes
            foreach (var att in UserAttributes)
            {
                //checking if the value is null, we can get rid of null event handlers
                // for example `@onmouseenter=@(IsOpen ? HandleEnter : null)`
                //this is a powerful feature that in normal HTML elements doesn't work, because
                //Blazor adds always the attribute value and creates an EventCallback
                if(att.Value!= null)
                builder.AddAttribute(1, att.Key, att.Value);

            }
            //Class
            builder.AddAttribute(2, "class", Class);
            //Style
            builder.AddAttribute(3, "style", Style);

            // StopPropagation
            //the order matters. This has to be before content is added
            if (HtmlTag == "button")
                builder.AddEventStopPropagationAttribute(5, "onclick", true);

            //Reference capture
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(6, async capturedRef =>
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

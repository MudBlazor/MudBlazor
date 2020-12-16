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

        protected  override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, HtmlTag);
            builder.AddMultipleAttributes(1, UserAttributes);
            builder.AddAttribute(2, "class", Class);
            builder.AddAttribute(3, "style", Style);
           
            //the order matters. This has to be before content is added
            if (HtmlTag == "button")
             builder.AddEventStopPropagationAttribute(5, "onclick", true); 
            
            builder.AddContent(10, ChildContent);


            builder.CloseElement();
        }
    }
}

﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudBlazor
{
    /// <summary>
    /// Primitive component which allows rendering any HTML element we want
    /// through the HtmlTag property
    /// </summary>
    public class MudElement : MudComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

       protected  override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, HtmlTag);
            builder.AddMultipleAttributes(1, UserAttributes);
            builder.AddAttribute(2, "class", Class);
            builder.AddAttribute(3, "style", Style);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}

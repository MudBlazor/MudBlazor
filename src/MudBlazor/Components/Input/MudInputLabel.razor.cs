﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudInputLabel : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder()
         .AddClass("mud-input-label")
         .AddClass("mud-input-label-animated")
         .AddClass($"mud-input-label-{Variant.ToDescriptionString()}")
         .AddClass(Class)
       .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

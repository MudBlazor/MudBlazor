﻿using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseMudList : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list")
          .AddClass(Class)
        .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}

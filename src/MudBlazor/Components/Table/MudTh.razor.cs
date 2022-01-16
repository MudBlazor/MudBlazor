﻿

using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTh : MudIComponent
    {
        protected string Classname => new CssBuilder("mud-table-cell")
            .AddClass(Class).Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}


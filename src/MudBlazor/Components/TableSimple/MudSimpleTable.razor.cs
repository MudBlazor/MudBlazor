using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public partial class MudSimpleTable : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-table mud-simple-table")
           .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-hover", Hover)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
          .AddClass(Class)
        .Build();

        [Parameter] public int Elevation { set; get; } = 1;
        [Parameter] public bool Hover { get; set; }
        [Parameter] public bool Square { get; set; }
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool Outlined { get; set; }
        // todo: implement
        [Parameter] public bool StickyHeader { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public partial class SimpleTable : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-simple-table")
          .AddClass(Class)
          .AddClass("mud-simple-table-dense", Dense)
        .Build();
        [Parameter] public string Class { get; set; }

        [Parameter] public bool Dense { get; set; }

        // todo: implement
        [Parameter] public bool StickyHeader { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public partial class SimpleTable : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-simple-table")
          .AddClass(Class)
          .AddClass("mud-simple-table-dense", Dense)
        .Build();

        [Parameter] public bool Dense { get; set; }

        // todo: implement
        [Parameter] public bool StickyHeader { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

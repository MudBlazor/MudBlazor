using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public class ComponentBaseSimpleTable : ComponentBase
    {
        protected string Classname =>
        new CssBuilder()
          .AddClass(Class)
        .Build();
        [Parameter] public string Class { get; set; }

        // todo: implement
        [Parameter] public bool Dense { get; set; }

        // todo: implement
        [Parameter] public bool StickyHeader { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}

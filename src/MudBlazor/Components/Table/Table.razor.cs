using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public class ComponentBaseTable<THeader, TItem> : ComponentBase
    {
        protected string Classname =>
        new CssBuilder()
          .AddClass(Class)
        .Build();
        [Parameter] public string Class { get; set; }

        [Parameter] public bool Dense { get; set; }

        [Parameter] public bool StickyHeader { get; set; }

        [Parameter] public bool HideToolbar { get; set; } = false;

        [Parameter] public string Title { get; set; }

        [Parameter] public bool HideHeader { get; set; } = false;

        [Parameter] public RenderFragment<THeader> HeaderTemplate { get; set; }

        [Parameter] public IEnumerable<THeader> Headers { get; set; }

        [Parameter] public RenderFragment<THeader> ItemTemplate { get; set; }

        [Parameter] public IEnumerable<THeader> Items { get; set; }

    }
}

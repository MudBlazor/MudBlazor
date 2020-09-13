
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;


namespace MudBlazor
{
    public class ComponentBaseTable : ComponentBase
    {
        protected string Classname =>
        new CssBuilder()
          .AddClass(Class)
        .Build();
        [Parameter] public string Class { get; set; }

        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool StickyHeader { get; set; }

    }
}

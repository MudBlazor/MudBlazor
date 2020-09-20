using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public class MudTableBase : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-table")
           .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-hover", Hover)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
          .AddClass(Class)
        .Build();

        [Parameter] public int Elevation { set; get; } = 1;
        [Parameter] public bool Square { get; set; }
        [Parameter] public bool Outlined { get; set; }
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool Hover { get; set; }
        [Parameter] public bool FixedHeader { get; set; }
        [Parameter] public int RowsPerPage { get; set; } = 10;
        [Parameter] public RenderFragment ToolBarContent { get; set; }
        [Parameter] public RenderFragment HeaderContent { get; set; }
        //[Parameter] public RenderFragment<T> RowTemplate { get; set; } <-- see MudTable.razor
        [Parameter] public RenderFragment PagerContent { get; set; }

        public void NavigateTo(Page page)
        {
         
        }

        public void SetRowsPerPage(int size)
        {
            RowsPerPage = size;
        }
    }
}

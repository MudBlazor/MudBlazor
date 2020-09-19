using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public partial class MudTable : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-table")
            .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
          .AddClass(Class)
        .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public int Elevation { set; get; } = 1;
        [Parameter] public bool Square { get; set; }
        [Parameter] public bool Outlined { get; set; }
        [Parameter] public bool Dense { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool FixedHeader { get; set; }
        [Parameter] public bool HideHeader { get; set; } = false;
        [Parameter] public int RowsPerPage { get; set; } = 10;
        [Parameter] public RenderFragment MudTableToolBar { get; set; }
        [Parameter] public RenderFragment MudTableHeader { get; set; }
        [Parameter] public RenderFragment MudTableBody { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }


        public void NavigateTo(Page page)
        {
         
        }

        public void SetRowsPerPage(int size)
        {
            RowsPerPage = size;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public abstract class MudTableBase : MudComponentBase
    {
        // note: the MudTable code is split. Everything that has nothing to do with the type parameter of MudTable<T> is here in MudTableBase

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
        [Parameter] public int CurrentPage { get; set; } = 0;

        [Parameter] public RenderFragment ToolBarContent { get; set; }
        [Parameter] public RenderFragment HeaderContent { get; set; }
        //[Parameter] public RenderFragment<T> RowTemplate { get; set; } <-- see MudTable.razor
        [Parameter] public RenderFragment PagerContent { get; set; }

        public void NavigateTo(Page page)
        {
            switch (page)
            {
                case Page.First:
                    CurrentPage = 0;
                    break;
                case Page.Last:
                    CurrentPage = Math.Max(0, NumPages-1);
                    break;
                case Page.Next:
                    CurrentPage = Math.Min(NumPages - 1, CurrentPage +1);
                    break;
                case Page.Previous:
                    CurrentPage = Math.Max(0, CurrentPage-1);
                    break;
            }
            StateHasChanged();
        }

        public void SetRowsPerPage(int size)
        {
            RowsPerPage = size;
            StateHasChanged();
        }

        protected abstract int NumPages { get; }

        public abstract int GetFilteredItemsCount();

        public abstract void SetSelectedItem(object item);
    }
}

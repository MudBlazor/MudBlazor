using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public abstract class MudTableBase : MudComponentBase
    {
        internal object _editingItem = null;
        
        private int _currentPage = 0;
        // note: the MudTable code is split. Everything that has nothing to do with the type parameter of MudTable<T> is here in MudTableBase

        protected string Classname =>
        new CssBuilder("mud-table")
           .AddClass($"mud-sm-table", Breakpoint == Breakpoint.Sm)
           .AddClass($"mud-md-table", Breakpoint == Breakpoint.Md)
           .AddClass($"mud-lg-table", Breakpoint == Breakpoint.Lg)
           .AddClass($"mud-xl-table", Breakpoint == Breakpoint.Xl)
           .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-hover", Hover)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-table-sticky-header", FixedHeader)
           .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// Set true to disable rounded corners
        /// </summary>
        [Parameter] public bool Square { get; set; }

        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter] public bool Hover { get; set; }

        /// <summary>
        /// At what breakpoint the table should switch to mobile layout. Takes Sm, Md, Lg and Xl the default behavior is breaking on Xs.
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; }

        /// <summary>
        /// When true, the header will stay in place when the table is scrolled. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter] public bool FixedHeader { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the table. If not set, it will try to grow in height. You can set this to any CSS value that the
        /// attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// If table is in smalldevice mode and uses any kind of sorting the text applied here will be the sort selects label.
        /// </summary>
        [Parameter] public string SortLabel { get; set; }

        /// <summary>
        /// If the table has more items than this number, it will break the rows into pages of said size.
        /// Note: requires a MudTablePager in PagerContent.
        /// </summary>
        [Parameter] public int RowsPerPage { get; set; } = 10;

        /// <summary>
        /// The page index of the currently displayed page. Usually called by MudTablePager.
        /// Note: requires a MudTablePager in PagerContent.
        /// </summary>
        [Parameter]
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Set to true to enable selection of multiple rows with check boxes. 
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        /// <summary>
        /// Optional. Add any kind of toolbar to this render fragment.
        /// </summary>
        [Parameter] public RenderFragment ToolBarContent { get; set; }

        /// <summary>
        /// Add MudTh cells here to define the table header.
        /// </summary>
        [Parameter] public RenderFragment HeaderContent { get; set; }

        /// <summary>
        /// Specifies a group of one or more columns in a table for formatting.
        /// Ex:
        /// table
        ///     colgroup
        ///        col span="2" style="background-color:red"
        ///        col style="background-color:yellow"
        ///      colgroup
        ///      header
        ///      body
        /// table
        /// </summary>
        [Parameter] public RenderFragment ColGroup { get; set; }

        //[Parameter] public RenderFragment<T> RowTemplate { get; set; } <-- see MudTable.razor

        /// <summary>
        /// Add MudTablePager here to enable breaking the rows in to multiple pages.
        /// </summary>
        [Parameter] public RenderFragment PagerContent { get; set; }

        public abstract TableContext TableContext { get; }

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
        }

        public void SetRowsPerPage(int size)
        {
            RowsPerPage = size;
            StateHasChanged();
        }

        protected abstract int NumPages { get; }

        public abstract int GetFilteredItemsCount();

        public abstract void SetSelectedItem(object item);

        public abstract void SetEditingItem(object item);

        protected string TableStyle 
            => new StyleBuilder()
                .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
                .Build();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}

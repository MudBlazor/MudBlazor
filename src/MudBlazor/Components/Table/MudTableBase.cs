using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    // note: the MudTable code is split. Everything that has nothing to do with the type parameter of MudTable<T> is here in MudTableBase

    public abstract class MudTableBase : MudComponentBase
    {
        internal object _editingItem = null;

        private int _currentPage = 0;
        private int? _rowsPerPage;

        protected string Classname =>
        new CssBuilder("mud-table")
           .AddClass($"mud-xs-table", Breakpoint == Breakpoint.Xs)
           .AddClass($"mud-sm-table", Breakpoint == Breakpoint.Sm)
           .AddClass($"mud-md-table", Breakpoint == Breakpoint.Md)
           .AddClass($"mud-lg-table", Breakpoint == Breakpoint.Lg || Breakpoint == Breakpoint.Always)
           .AddClass($"mud-xl-table", Breakpoint == Breakpoint.Xl || Breakpoint == Breakpoint.Always)
           .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-hover", Hover)
           .AddClass($"mud-table-bordered", Bordered)
           .AddClass($"mud-table-striped", Striped)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-table-sticky-header", FixedHeader)
           .AddClass($"mud-elevation-{Elevation}", !Outlined)
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

        /// <summary>
        /// If true, table will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// If true, table's cells will have left/right borders.
        /// </summary>
        [Parameter] public bool Bordered { get; set; }

        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter] public bool Hover { get; set; }

        /// <summary>
        /// If true, striped table rows will be used.
        /// </summary>
        [Parameter] public bool Striped { get; set; }

        /// <summary>
        /// At what breakpoint the table should switch to mobile layout. Takes None, Xs, Sm, Md, Lg and Xl the default behavior is breaking on Xs.
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;

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
        [Parameter]
        public int RowsPerPage
        {
            get => _rowsPerPage ?? 10;
            set
            {
                if (_rowsPerPage == null)
                    SetRowsPerPage(value);
            }
        }

        /// <summary>
        /// The page index of the currently displayed page (Zero based). Usually called by MudTablePager.
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
                InvokeServerLoadFunc();
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


        /// <summary>
        /// Locks Inline Edit mode, if true.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Button click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnCommitEditClick { get; set; }

        /// <summary>
        /// Command executed when the user clicks on the CommitEdit Button.
        /// </summary>
        [Parameter] public ICommand CommitEditCommand { get; set; }

        /// <summary>
        /// Command parameter for the CommitEdit Button. By default, will be the row level item model, if you won't set anything else.
        /// </summary>
        [Parameter] public object CommitEditCommandParameter { get; set; }

        /// <summary>
        /// Tooltip for the CommitEdit Button.
        /// </summary>
        [Parameter] public string CommitEditTooltip { get; set; }

        /// <summary>
        /// Number of items. Used only with ServerData="true"
        /// </summary>
        [Parameter] public int TotalItems { get; set; }

        /// <summary>
        /// CSS class for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowClass { get; set; }

        /// <summary>
        /// CSS styles for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowStyle { get; set; }


        #region --> Obsolete Forwarders for Backwards-Compatiblilty
        /// <summary>
        /// Alignment of the table cell text when breakpoint is smaller than <see cref="Breakpoint" />
        /// </summary>
        [Obsolete("This property is obsolete. And not needed anymore, the cells width/alignment is done automaticly.")] [Parameter] public bool RightAlignSmall { get; set; } = true;
        #endregion

        public abstract TableContext TableContext { get; }

        public void NavigateTo(Page page)
        {
            switch (page)
            {
                case Page.First:
                    CurrentPage = 0;
                    break;
                case Page.Last:
                    CurrentPage = Math.Max(0, NumPages - 1);
                    break;
                case Page.Next:
                    CurrentPage = Math.Min(NumPages - 1, CurrentPage + 1);
                    break;
                case Page.Previous:
                    CurrentPage = Math.Max(0, CurrentPage - 1);
                    break;
            }
        }

        public void SetRowsPerPage(int size)
        {
            if (_rowsPerPage == size)
                return;
            _rowsPerPage = size;
            CurrentPage = 0;
            StateHasChanged();
            InvokeServerLoadFunc();
        }

        protected abstract int NumPages { get; }

        public abstract int GetFilteredItemsCount();

        public abstract void SetSelectedItem(object item);

        public abstract void SetEditingItem(object item);

        internal async Task OnCommitEditHandler(MouseEventArgs ev, object item)
        {
            await OnCommitEditClick.InvokeAsync(ev);
            if (CommitEditCommand?.CanExecute(CommitEditCommandParameter) ?? false)
            {
                var parameter = CommitEditCommandParameter;
                if (parameter == null)
                    parameter = item;
                CommitEditCommand.Execute(parameter);
            }
        }

        protected string TableStyle
            => new StyleBuilder()
                .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
                .Build();

        internal abstract bool HasServerData { get; }

        internal abstract Task InvokeServerLoadFunc();

        internal abstract void FireRowClickEvent(MouseEventArgs args, MudTr mudTr, object item);

        public Interfaces.IForm Validator { get; set; } = new TableRowValidator();
    }
}

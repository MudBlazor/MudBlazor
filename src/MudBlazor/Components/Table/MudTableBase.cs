using System;
using System.Diagnostics.CodeAnalysis;
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
        internal bool IsEditing => _editingItem != null;

        private int _currentPage = 0;
        private int? _rowsPerPage;
        private bool _isFirstRendered = false;

        protected string Classname =>
        new CssBuilder("mud-table")
           .AddClass("mud-xs-table", Breakpoint == Breakpoint.Xs)
           .AddClass("mud-sm-table", Breakpoint == Breakpoint.Sm)
           .AddClass("mud-md-table", Breakpoint == Breakpoint.Md)
           .AddClass("mud-lg-table", Breakpoint is Breakpoint.Lg or Breakpoint.Always)
           .AddClass("mud-xl-table", Breakpoint is Breakpoint.Xl or Breakpoint.Always)
           .AddClass("mud-xxl-table", Breakpoint is Breakpoint.Xxl or Breakpoint.Always)
           .AddClass("mud-table-dense", Dense)
           .AddClass("mud-table-hover", Hover)
           .AddClass("mud-table-bordered", Bordered)
           .AddClass("mud-table-striped", Striped)
           .AddClass("mud-table-outlined", Outlined)
           .AddClass("mud-table-square", Square)
           .AddClass("mud-table-sticky-header", FixedHeader)
           .AddClass("mud-table-sticky-footer", FixedFooter)
           .AddClass($"mud-elevation-{Elevation}", !Outlined)
          .AddClass(Class)
        .Build();

        protected string HeadClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();
        protected string FootClassname => new CssBuilder("mud-table-foot")
            .AddClass(FooterClass).Build();

        /// <summary>
        /// When editing a row and this is true, the editing row must be saved/cancelled before a new row will be selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool IsEditRowSwitchingBlocked { get; set; } = false;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Set true to disable rounded corners
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, table will be outlined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Outlined { get; set; }

        /// <summary>
        /// If true, table's cells will have left/right borders.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// If true, striped table rows will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Striped { get; set; }

        /// <summary>
        /// At what breakpoint the table should switch to mobile layout. Takes None, Xs, Sm, Md, Lg and Xl the default behavior is breaking on Xs.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;

        /// <summary>
        /// When true, the header will stay in place when the table is scrolled. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public bool FixedHeader { get; set; }

        /// <summary>
        /// When true, the footer will be visible is not scrolled to the bottom. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public bool FixedFooter { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the table. If not set, it will try to grow in height. You can set this to any CSS value that the
        /// attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public string Height { get; set; }

        /// <summary>
        /// If table is in smalldevice mode and uses any kind of sorting the text applied here will be the sort selects label.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Sorting)]
        public string SortLabel { get; set; }

        /// <summary>
        /// If true allows table to be in an unsorted state through column clicks (i.e. first click sorts "Ascending", second "Descending", third "None").
        /// If false only "Ascending" and "Descending" states are allowed (i.e. there always should be a column to sort).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Sorting)]
        public bool AllowUnsorted { get; set; } = true;

        /// <summary>
        /// If the table has more items than this number, it will break the rows into pages of said size.
        /// Note: requires a MudTablePager in PagerContent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public int RowsPerPage
        {
            get => _rowsPerPage ?? 10;
            set
            {
                if (_rowsPerPage is null || _rowsPerPage != value)
                    SetRowsPerPage(value);
            }
        }

        /// <summary>
        /// Rows Per Page two-way bindable parameter
        /// </summary>
        [Parameter] public EventCallback<int> RowsPerPageChanged { get; set; }

        /// <summary>
        /// The page index of the currently displayed page (Zero based). Usually called by MudTablePager.
        /// Note: requires a MudTablePager in PagerContent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                InvokeAsync(StateHasChanged);
                if (_isFirstRendered)
                    InvokeServerLoadFunc();
            }
        }

        /// <summary>
        /// Set to true to enable selection of multiple rows with check boxes. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// Optional. Add any kind of toolbar to this render fragment.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment ToolBarContent { get; set; }

        /// <summary>
        /// Show a loading animation, if true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public bool Loading { get; set; }

        /// <summary>
        /// The color of the loading progress if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public Color LoadingProgressColor { get; set; } = Color.Info;

        /// <summary>
        /// Add MudTh cells here to define the table header. If <see cref="CustomHeader"/> is set, add one or more MudTHeadRow instead.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public RenderFragment HeaderContent { get; set; }

        /// <summary>
        /// Specify if the header has multiple rows. In that case, you need to provide the MudTHeadRow tags.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public bool CustomHeader { get; set; }

        /// <summary>
        /// Add a class to the thead tag
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public string HeaderClass { get; set; }

        /// <summary>
        /// Add MudTd cells here to define the table footer. If<see cref="CustomFooter"/> is set, add one or more MudTFootRow instead.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public RenderFragment FooterContent { get; set; }

        /// <summary>
        /// Specify if the footer has multiple rows. In that case, you need to provide the MudTFootRow tags.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public bool CustomFooter { get; set; }

        /// <summary>
        /// Add a class to the tfoot tag
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public string FooterClass { get; set; }

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
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment ColGroup { get; set; }

        //[Parameter] public RenderFragment<T> RowTemplate { get; set; } <-- see MudTable.razor

        /// <summary>
        /// Add MudTablePager here to enable breaking the rows in to multiple pages.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public RenderFragment PagerContent { get; set; }


        /// <summary>
        /// Locks Inline Edit mode, if true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Button commit edit click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnCommitEditClick { get; set; }

        /// <summary>
        /// Button cancel edit click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnCancelEditClick { get; set; }

        /// <summary>
        /// Event is called before the item is modified in inline editing.
        /// </summary>
        [Parameter] public EventCallback<object> OnPreviewEditClick { get; set; }

        /// <summary>
        /// Command executed when the user clicks on the CommitEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public ICommand CommitEditCommand { get; set; }

        /// <summary>
        /// Command parameter for the CommitEdit Button. By default, will be the row level item model, if you won't set anything else.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public object CommitEditCommandParameter { get; set; }

        /// <summary>
        /// Tooltip for the CommitEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CommitEditTooltip { get; set; }

        /// <summary>
        /// Tooltip for the CancelEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CancelEditTooltip { get; set; }

        /// <summary>
        /// Sets the Icon of the CommitEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CommitEditIcon { get; set; } = Icons.Material.Filled.Done;

        /// <summary>
        /// Sets the Icon of the CancelEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CancelEditIcon { get; set; } = Icons.Material.Filled.Cancel;

        /// <summary>
        /// Define if Cancel button is present or not for inline editing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool CanCancelEdit { get; set; }

        /// <summary>
        /// Set the positon of the CommitEdit and CancelEdit button, if <see cref="IsEditable"/> IsEditable is true. Defaults to the end of the row
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public TableApplyButtonPosition ApplyButtonPosition { get; set; } = TableApplyButtonPosition.End;


        /// <summary>
        /// The method is called before the item is modified in inline editing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object> RowEditPreview { get; set; }

        /// <summary>
        /// The method is called when the edition of the item has been committed in inline editing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object> RowEditCommit { get; set; }

        /// <summary>
        /// The method is called when the edition of the item has been canceled in inline editing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object> RowEditCancel { get; set; }

        /// <summary>
        /// Number of items. Used only with ServerData="true"
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public int TotalItems { get; set; }

        /// <summary>
        /// CSS class for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public string RowClass { get; set; }

        /// <summary>
        /// CSS styles for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public string RowStyle { get; set; }

        /// <summary>
        /// If true, the results are displayed in a Virtualize component, allowing a boost in rendering speed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public bool Virtualize { get; set; }

        #region --> Obsolete Forwarders for Backwards-Compatiblilty
        /// <summary>
        /// Alignment of the table cell text when breakpoint is smaller than <see cref="Breakpoint" />
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("This property is not needed anymore, the cells width/alignment is done automatically.", true)]
        [Parameter] public bool RightAlignSmall { get; set; } = true;
        #endregion

        public abstract TableContext TableContext { get; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                _isFirstRendered = true;

            return base.OnAfterRenderAsync(firstRender);
        }

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
        /// <summary>
        /// Navigate to page with specified index.
        /// </summary>
        /// <param name="pageIndex"> The index of the page number.</param>
        public void NavigateTo(int pageIndex)
        {
            CurrentPage = Math.Min(Math.Max(0, pageIndex), NumPages - 1);
        }

        public void SetRowsPerPage(int size)
        {
            if (_rowsPerPage == size)
                return;
            _rowsPerPage = size;
            CurrentPage = 0;
            StateHasChanged();
            RowsPerPageChanged.InvokeAsync(_rowsPerPage.Value);
            if (_isFirstRendered)
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

        internal Task OnPreviewEditHandler(object item)
        {
            return OnPreviewEditClick.InvokeAsync(item);
        }

        internal Task OnCancelEditHandler(MouseEventArgs ev)
        {
            return OnCancelEditClick.InvokeAsync(ev);
        }

        protected string TableStyle
            => new StyleBuilder()
                .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
                .Build();

        internal abstract bool HasServerData { get; }

        internal abstract Task InvokeServerLoadFunc();

        internal abstract void FireRowClickEvent(MouseEventArgs args, MudTr mudTr, object item);

        internal abstract void OnHeaderCheckboxClicked(bool value);

        internal abstract bool IsEditable { get; }

        public Interfaces.IForm Validator { get; set; } = new TableRowValidator();
    }
}

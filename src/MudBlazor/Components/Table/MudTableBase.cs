using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    // note: the MudTable code is split. Everything that has nothing to do with the type parameter of MudTable<T> is here in MudTableBase

    /// <summary>
    /// A base class for designing table components.
    /// </summary>
    public abstract class MudTableBase : MudComponentBase
    {
        private int _currentPage = 0;
        private bool _isFirstRendered = false;
        internal int? _rowsPerPage;
        internal object? _editingItem = null;
        internal bool Editing => _editingItem != null;

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
            .AddClass(HeaderClass)
            .Build();

        protected string FootClassname => new CssBuilder("mud-table-foot")
            .AddClass(FooterClass)
            .Build();

        /// <summary>
        /// Forces a row being edited to be saved or canceled before a new row can be selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>false</c>, a new row can be edited even if the previous one has not been saved or canceled.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool IsEditRowSwitchingBlocked { get; set; } = false;

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Uses square corners for the table.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Shows borders around the table.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Outlined { get; set; }

        /// <summary>
        /// Shows left and right borders for each table cell.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// Uses compact padding for all rows.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Highlights rows when hovering over them.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// Uses alternating colors for table rows.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public bool Striped { get; set; }

        /// <summary>
        /// The screen width at which this table switches to small-device mode.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Breakpoint.Xs"/>.  Allowed values are <c>None</c>, <c>Xs</c>, <c>Sm</c>, <c>Md</c>, <c>Lg</c>, and <c>Xl</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;

        /// <summary>
        /// Fixes the table header in place while the table is scrolled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When set, <see cref="Height"/> must also be set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public bool FixedHeader { get; set; }

        /// <summary>
        /// Fixes the table footer in place while the table is scrolled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When set, <see cref="Height"/> must also be set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public bool FixedFooter { get; set; }

        /// <summary>
        /// The fixed height of this table, as a CSS Value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Accepts values such as <c>50%</c> or <c>500px</c>.  When <c>null</c>, the table will try to grow to fit its content.  When set, <see cref="FixedHeader"/> and <see cref="FixedFooter"/> can be enabled.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// The sort label shown when this table is in small-device mode and is sorting by a column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Sorting)]
        public string? SortLabel { get; set; }

        /// <summary>
        /// Allows a sort direction of <see cref="SortDirection.None"/> in addition to <see cref="SortDirection.Ascending"/> and <see cref="SortDirection.Descending"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, the sort mode will only toggle between <see cref="SortDirection.Ascending"/> and <see cref="SortDirection.Descending"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Sorting)]
        public bool AllowUnsorted { get; set; } = true;

        /// <summary>
        /// The maximum rows to display per page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, rows beyond this number will overflow into separate pages.  Requires a <see cref="MudTablePager"/> in the <see cref="PagerContent"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public int RowsPerPage
        {
            get => _rowsPerPage ?? 10;
            set
            {
                if (_rowsPerPage is null || _rowsPerPage != value)
                {
                    SetRowsPerPage(value);
                }
            }
        }

        /// <summary>
        /// Occurs when <see cref="RowsPerPage"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> RowsPerPageChanged { get; set; }

        /// <summary>
        /// The index of the current page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c> (the first page).  Requires a <see cref="MudTablePager"/> in the <see cref="PagerContent"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;
                InvokeAsync(StateHasChanged);
                if (_isFirstRendered)
                {
                    InvokeServerLoadFunc();
                }
            }
        }

        /// <summary>
        /// Allows multiple rows to be selected with checkboxes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// Disables the selection of rows but keep showing the selected rows.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Requires <see cref="MultiSelection"/> to be <c>true</c> and <see cref="Editable"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public bool SelectionChangeable { get; set; } = true;

        /// <summary>
        /// Toggles the checkbox when a row is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Requires <see cref="MultiSelection"/> to be <c>true</c>, <see cref="SelectionChangeable"/> to be <c>true</c> and <see cref="Editable"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public bool SelectOnRowClick { get; set; } = true;

        /// <summary>
        /// The custom content for the toolbar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment? ToolBarContent { get; set; }

        /// <summary>
        /// Displays a loading animation while <c>ServerData</c> executes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Becomes <c>true</c> before <c>ServerData</c> is called, then becomes <c>false</c>.  When <c>true</c>, either a <see cref="MudProgressLinear"/> is displayed or custom content if <c>LoadingContent</c> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public bool Loading { get; set; }

        /// <summary>
        /// The color of the <see cref="MudProgressLinear"/> while <see cref="Loading"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Info"/>.  Has no effect if <c>LoadingContent</c> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public Color LoadingProgressColor { get; set; } = Color.Info;

        /// <summary>
        /// The content of this table's header.
        /// </summary>
        /// <remarks>
        /// For basic headers, add <see cref="MudTh"/> components here to describe each column.  For more customized headers (such as multi-row headers), set <see cref="CustomHeader"/> to <c>true</c> and use <see cref="MudTHeadRow"/> components here instead.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public RenderFragment? HeaderContent { get; set; }

        /// <summary>
        /// Enables customized headers beyond basic header columns.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, custom headers such as multiple-row headers can be used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public bool CustomHeader { get; set; }

        /// <summary>
        /// The CSS classes applied to the header.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Header)]
        public string? HeaderClass { get; set; }

        /// <summary>
        /// The CSS styles applied to this table.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public string? ContainerStyle { get; set; }

        /// <summary>
        /// The CSS classes applied to this table.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public string? ContainerClass { get; set; }

        /// <summary>
        /// The content of this table's footer.
        /// </summary>
        /// <remarks>
        /// For basic footers, add <see cref="MudTd"/> components here to describe each column.  For more customized footers (such as multi-row footers), set <see cref="CustomFooter"/> to <c>true</c> and use <see cref="MudTFootRow"/> components here instead.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public RenderFragment? FooterContent { get; set; }

        /// <summary>
        /// Enables customized footers beyond basic footer columns.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, custom footers such as multiple-row footers can be used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public bool CustomFooter { get; set; }

        /// <summary>
        /// Add a class to the tfoot tag
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Footer)]
        public string? FooterClass { get; set; }

        /// <summary>
        /// Specifies formatting information for this table's columns such as size and style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Add a set of <c>col</c> elements to style each column.  For example:
        /// <para>
        /// &lt;ColGroup&gt;<br />
        /// &lt;col span="2" style="max-midth:120px; background-color:red"&gt;<br />
        /// &lt;col style="background-color:yellow"&gt;<br />
        /// &lt;/ColGroup&gt;
        /// </para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment? ColGroup { get; set; }

        /// <summary>
        /// The custom pagination content for this table.
        /// </summary>
        /// <remarks>
        /// Add a <see cref="MudTablePager"/> here to navigate multiple pages of data.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Pagination)]
        public RenderFragment? PagerContent { get; set; }

        /// <summary>
        /// Prevents rows from being edited inline.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Occurs when changes to a row being edited are committed.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnCommitEditClick { get; set; }

        /// <summary>
        /// Occurs when changes to a row being edited are canceled.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnCancelEditClick { get; set; }

        /// <summary>
        /// Occurs before inline editing is enabled for a row.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        public EventCallback<object?> OnPreviewEditClick { get; set; }

        /// <summary>
        /// The tooltip shown next to the button which commits inline edits.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string? CommitEditTooltip { get; set; }

        /// <summary>
        /// The tooltip shown next to the button which cancels inline edits.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string? CancelEditTooltip { get; set; }

        /// <summary>
        /// The icon shown for the button which commits inline edits.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Done"/>.  Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CommitEditIcon { get; set; } = Icons.Material.Filled.Done;

        /// <summary>
        /// The icon shown for the button which cancels inline edits.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Cancel"/>.  Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public string CancelEditIcon { get; set; } = Icons.Material.Filled.Cancel;

        /// <summary>
        /// Shows the cancel button during inline editing.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public bool CanCancelEdit { get; set; }

        /// <summary>
        /// The position of the button which commits inline edits.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TableApplyButtonPosition.End"/>.  Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public TableApplyButtonPosition ApplyButtonPosition { get; set; } = TableApplyButtonPosition.End;

        /// <summary>
        /// The position of the button which begins inline editing.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TableEditButtonPosition.End"/>.  Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public TableEditButtonPosition EditButtonPosition { get; set; } = TableEditButtonPosition.End;

        /// <summary>
        /// The behavior which begins inline editing.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TableEditTrigger.RowClick"/>.  Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public TableEditTrigger EditTrigger { get; set; } = TableEditTrigger.RowClick;

        /// <summary>
        /// The content of the Edit button which starts inline editing.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="Editable"/> to be <c>true</c> and <see cref="ReadOnly"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public RenderFragment<MudBlazorFix.EditButtonContext>? EditButtonContent { get; set; }

        /// <summary>
        /// Occurs before inline editing begins for a row.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object?>? RowEditPreview { get; set; }

        /// <summary>
        /// Occurs when changes are committed for an row being edited.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object?>? RowEditCommit { get; set; }

        /// <summary>
        /// Occurs when changed are canceled for a row being edited.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Action<object?>? RowEditCancel { get; set; }

        /// <summary>
        /// The total number of rows (excluding pages) when using <c>ServerData</c> for data.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public int TotalItems { get; set; }

        /// <summary>
        /// The CSS classes applied to each row.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.  Some CSS classes will be overridden by <see cref="MudTd"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public string? RowClass { get; set; }

        /// <summary>
        /// The CSS styles applied to each row.
        /// </summary>
        /// <remarks>
        /// Some CSS styles will be overridden by <see cref="MudTd"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public string? RowStyle { get; set; }

        /// <summary>
        /// Uses virtualization to display large amounts of items efficiently.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Typically used for more than <c>1000</c> items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public bool Virtualize { get; set; }

        /// <summary>
        /// The number of additional items to render outside of view when <see cref="Virtualize"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>3</c>.  Can reduce the frequency of rendering during scrolling, but higher values can reduce performance.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// The height of each row, in pixels, when <see cref="Virtualize"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>50</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public float ItemSize { get; set; } = 50f;

        /// <summary>
        /// The current state of this table.
        /// </summary>
        /// <remarks>
        /// Typically used to interact with other components such as <see cref="MudTablePager"/>.
        /// </remarks>
        public abstract TableContext TableContext { get; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                _isFirstRendered = true;

            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Changes the current page.
        /// </summary>
        /// <param name="page">The <c>Next</c>, <c>Previous</c>, <c>First</c>, or <c>Last</c> page to navigate to.</param>
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
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="pageIndex">The index of the page to navigate to.</param>
        public void NavigateTo(int pageIndex)
        {
            CurrentPage = Math.Min(Math.Max(0, pageIndex), NumPages - 1);
        }

        /// <summary>
        /// Changes the rows per page to the specified value.
        /// </summary>
        /// <param name="size">The number of rows per page.</param>
        public void SetRowsPerPage(int size)
        {
            if (_rowsPerPage == size)
            {
                return;
            }

            _rowsPerPage = size;
            _currentPage = 0;
            StateHasChanged();
            RowsPerPageChanged.InvokeAsync(_rowsPerPage.Value);
            if (_isFirstRendered)
            {
                InvokeServerLoadFunc();
            }
        }

        protected abstract int NumPages { get; }

        /// <summary>
        /// Gets the number of items after applying filters.
        /// </summary>
        /// <returns>The number of filtered items.</returns>
        public abstract int GetFilteredItemsCount();

        /// <summary>
        /// Changes the currently selected item.
        /// </summary>
        /// <param name="item">The new item to select.</param>
        public abstract void SetSelectedItem(object? item);

        /// <summary>
        /// Changes the item being edited.
        /// </summary>
        /// <param name="item">The new item to edit.</param>
        public abstract void SetEditingItem(object? item);

        internal async Task OnCommitEditHandler(MouseEventArgs ev, object? item)
        {
            await OnCommitEditClick.InvokeAsync(ev);
        }

        internal Task OnPreviewEditHandler(object? item)
        {
            return OnPreviewEditClick.InvokeAsync(item);
        }

        internal Task OnCancelEditHandler(MouseEventArgs ev)
        {
            return OnCancelEditClick.InvokeAsync(ev);
        }

        protected string TableContainerStyle
            => new StyleBuilder()
                .AddStyle(ContainerStyle)
                .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
                .Build();

        protected string TableContainerClass
            => new CssBuilder(ContainerClass)
                .Build();

        internal abstract bool HasServerData { get; }

        internal abstract Task InvokeServerLoadFunc();

        internal abstract Task FireRowClickEventAsync(MouseEventArgs args, MudTr mudTr, object? item);

        internal abstract Task FireRowMouseEnterEventAsync(PointerEventArgs args, MudTr mudTr, object? item);

        internal abstract Task FireRowMouseLeaveEventAsync(PointerEventArgs args, MudTr mudTr, object? item);

        internal abstract void OnHeaderCheckboxClicked(bool checkedState);

        internal abstract bool HasRowMouseEnterEventHandler { get; }

        internal abstract bool HasRowMouseLeaveEventHandler { get; }

        internal abstract bool Editable { get; }

        /// <summary>
        /// Gets whether the specified item exists.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns></returns>
        public abstract bool ContainsItem(object? item);

        /// <summary>
        /// Refreshes the table's current selection.
        /// </summary>
        public abstract void UpdateSelection();

        /// <summary>
        /// The validator for this table.
        /// </summary>
        /// <remarks>
        /// Defaults to a new <see cref="TableRowValidator"/>.
        /// </remarks>
        public Interfaces.IForm Validator { get; set; } = new TableRowValidator();
    }
}

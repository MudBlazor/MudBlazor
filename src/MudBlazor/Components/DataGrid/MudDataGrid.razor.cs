// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.DataGrid;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDataGrid<T> : MudComponentBase
    {
        private int _currentPage = 0;
        private int? _rowsPerPage;
        private bool _isFirstRendered = false;
        private bool _filtersMenuVisible = false;
        internal readonly List<Column<T>> _columns = new List<Column<T>>();
        internal readonly List<FilterDefinition<T>> _filterDefinitions = new List<FilterDefinition<T>>();
        internal T _editingItem;
        internal T _previousEditingItem;

        #region Notify Children Delegates

        internal Action<string> OnSortChanged { get; set; }
        internal Action<HashSet<T>> OnSelectedItemsChanged { get; set; }
        internal Action StartedEditingItem { get; set; }
        internal Action EditingCancelled { get; set; }
        internal Action<T> StartedCommittingItemChanges { get; set; }

        #endregion

        /// <summary>
        /// Controls whether data in the DataGrid can be sorted. This is overridable by each column.
        /// </summary>
        [Parameter] public bool Sortable { get; set; } = false;
        /// <summary>
        /// Controls whether data in the DataGrid can be filtered. This is overridable by each column.
        /// </summary>
        [Parameter] public bool Filterable { get; set; } = false;
        /// <summary>
        /// Controls whether to hide or show the column options. This is overridable by each column.
        /// </summary>
        [Parameter] public bool ShowColumnOptions { get; set; } = true;
        /// <summary>
        /// At what breakpoint the table should switch to mobile layout. Takes None, Xs, Sm, Md, Lg and Xl the default behavior is breaking on Xs.
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;
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
        /// When true, the header will stay in place when the table is scrolled. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter] public bool FixedHeader { get; set; }
        /// <summary>
        /// When true, the footer will be visible is not scrolled to the bottom. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter] public bool FixedFooter { get; set; }

        public TableGroupDefinition<T> GroupBy { get; set; }

        /// <summary>
        /// If true, the results are displayed in a Virtualize component, allowing a boost in rendering speed.
        /// </summary>
        [Parameter] public bool Virtualize { get; set; }
        /// <summary>
        /// CSS class for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowClass { get; set; }
        /// <summary>
        /// CSS styles for the table rows. Note, many CSS settings are overridden by MudTd though
        /// </summary>
        [Parameter] public string RowStyle { get; set; }
        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<T, int, string> RowClassFunc { get; set; }
        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter] public Func<T, int, string> RowStyleFunc { get; set; }
        /// <summary>
        /// Set to true to enable selection of multiple rows with check boxes. 
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }
        /// <summary>
        /// Defines how a table row looks like in edit mode (for selected row). Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter] public RenderFragment<T> RowEditingTemplate { get; set; }
        [Parameter] public DataGridEditMode? EditMode { get; set; }

        private IEnumerable<T> _items;
        
        /// <summary>
        /// The data to display in the table. MudTable will render one row per item
        /// </summary>
        /// 
        [Parameter]
        public IEnumerable<T> Items
        {
            get => _items;
            set
            {
                if (_items == value)
                    return;
                _items = value;
                if (PagerStateHasChanged != null)
                    InvokeAsync(PagerStateHasChanged);
            }
        }

        public Action PagerStateHasChanged { get; set; }

        /// <summary>
        /// Show a loading animation, if true.
        /// </summary>
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool CanCancelEdit { get; set; } = true;

        /// <summary>
        /// The color of the loading progress if used. It supports the theme colors.
        /// </summary>
        [Parameter] public Color LoadingProgressColor { get; set; } = Color.Info;

        /// <summary>
        /// Optional. Add any kind of toolbar to this render fragment.
        /// </summary>
        [Parameter] public RenderFragment ToolBarContent { get; set; }

        /// <summary>
        /// Defines if the table has a horizontal scrollbar.
        /// </summary>
        [Parameter] public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// Add a class to the thead tag
        /// </summary>
        [Parameter] public string HeaderClass { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the table. If not set, it will try to grow in height. You can set this to any CSS value that the
        /// attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// Add a class to the tfoot tag
        /// </summary>
        [Parameter] public string FooterClass { get; set; }

        /// <summary>
        /// A function that returns whether or not an item should be displayed in the table. You can use this to implement your own search function.
        /// </summary>
        [Parameter] public Func<T, bool> QuickFilter { get; set; } = null;

        [Parameter] public RenderFragment Header { get; set; }
        [Parameter] public RenderFragment Columns { get; set; }
        [Parameter] public RenderFragment Footer { get; set; }

        /// <summary>
        /// Row Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment<T> ChildRowContent { get; set; }

        /// <summary>
        /// Defines the table body content when there are no matching records found
        /// </summary>
        [Parameter] public RenderFragment NoRecordsContent { get; set; }

        /// <summary>
        /// Defines the table body content  the table has no rows and is loading
        /// </summary>
        [Parameter] public RenderFragment LoadingContent { get; set; }

        /// <summary>
        /// Add MudTablePager here to enable breaking the rows in to multiple pages.
        /// </summary>
        [Parameter] public RenderFragment PagerContent { get; set; }

        /// <summary>
        /// Supply an async function which (re)loads filtered, paginated and sorted data from server.
        /// Table will await this func and update based on the returned TableData.
        /// Used only with ServerData
        /// </summary>
        [Parameter] public Func<GridState<T>, Task<GridData<T>>> ServerData { get; set; }

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

                if (_isFirstRendered)
                    InvokeAsync(InvokeServerLoadFunc);
            }
        }

        /// <summary>
        /// Locks Inline Edit mode, if true.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; } = true;

        /// <summary>
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// If MultiSelection is true, this returns the currently selected items. You can bind this property and the initial content of the HashSet you bind it to will cause these rows to be selected initially.
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems
        {
            get
            {
                if (!MultiSelection)
                    if (_selectedItem is null)
                        return new HashSet<T>(Array.Empty<T>());
                    else
                        return new HashSet<T>(new T[] { _selectedItem });
                else
                    return Selection;
            }
            set
            {
                if (value == Selection)
                    return;
                if (value == null)
                {
                    if (Selection.Count == 0)
                        return;
                    Selection = new HashSet<T>();
                }
                else
                    Selection = value;
                SelectedItemsChanged.InvokeAsync(Selection);
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Returns the item which was last clicked on in single selection mode (that is, if MultiSelection is false)
        /// </summary>
        [Parameter]
        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (EqualityComparer<T>.Default.Equals(SelectedItem, value))
                    return;
                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(value);
            }
        }
        private T _selectedItem;

        /// <summary>
        /// Callback is called when a row has been clicked and returns the selected item.
        /// </summary>
        [Parameter] public EventCallback<T> SelectedItemChanged { get; set; }

        public HashSet<T> Selection { get; set; } = new HashSet<T>();

        public bool HasPager { get; set; }

        GridData<T> _server_data = new GridData<T>() { TotalItems = 0, Items = Array.Empty<T>() };

        //internal bool IsEditable { get => (RowEditingTemplate != null); }

        internal async Task InvokeServerLoadFunc()
        {
            if (ServerData == null)
                return;

            Loading = true;
            StateHasChanged();
            //var label = CurrentSortLabel;

            var state = new GridState<T>
            {
                Page = CurrentPage,
                PageSize = RowsPerPage,
                SortBy = sortBy,
                SortDirection = direction
            };

            _server_data = await ServerData(state);

            if (CurrentPage * RowsPerPage > _server_data.TotalItems)
                CurrentPage = 0;

            Loading = false;
            StateHasChanged();
            PagerStateHasChanged?.Invoke();
        }

        protected string Classname =>
            new CssBuilder("mud-table")
               .AddClass("mud-data-grid")
               .AddClass("mud-xs-table", Breakpoint == Breakpoint.Xs)
               .AddClass("mud-sm-table", Breakpoint == Breakpoint.Sm)
               .AddClass("mud-md-table", Breakpoint == Breakpoint.Md)
               .AddClass("mud-lg-table", Breakpoint == Breakpoint.Lg || Breakpoint == Breakpoint.Always)
               .AddClass("mud-xl-table", Breakpoint == Breakpoint.Xl || Breakpoint == Breakpoint.Always)
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

        protected string TableStyle => 
            new StyleBuilder()
            .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
            .Build();

        internal string GetHorizontalScrollbarStyle() => HorizontalScrollbar ? ";display: block; overflow-x: auto;" : string.Empty;

        protected string HeadClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();

        protected string FootClassname => new CssBuilder("mud-table-foot")
            .AddClass(FooterClass).Build();

        protected int NumPages
        {
            get
            {
                if (ServerData != null)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        public IEnumerable<T> FilteredItems
        {
            get
            {
                if (ServerData != null)
                    return _server_data.Items;

                var items = Items;

                // Quick filtering
                if (QuickFilter != null)
                {
                    items = items.Where(QuickFilter);
                }

                foreach (var f in _filterDefinitions)
                {
                    var filterFunc = f.GenerateFilterFunction();
                    items = items.Where(filterFunc);
                }

                return Sort(items);
            }
        }

        protected IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (@PagerContent == null)
                    return FilteredItems; // we have no pagination
                if (ServerData == null)
                {
                    var filteredItemCount = GetFilteredItemsCount();
                    int lastPageNo;
                    if (filteredItemCount == 0)
                        lastPageNo = 0;
                    else
                        lastPageNo = (filteredItemCount / RowsPerPage) - (filteredItemCount % RowsPerPage == 0 ? 1 : 0);
                    CurrentPage = lastPageNo < CurrentPage ? lastPageNo : CurrentPage;
                }

                return GetItemsOfPage(CurrentPage, RowsPerPage);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isFirstRendered = true;
                await InvokeServerLoadFunc();
            }
            else
            {
                PagerStateHasChanged?.Invoke();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        internal void AddColumn(Column<T> column)
        {
            _columns.Add(column);
        }

        /// <summary>
        /// Called by the DataGrid when the "Add Filter" button is pressed.
        /// </summary>
        internal void AddFilter()
        {
            _filterDefinitions.Add(new FilterDefinition<T>
            {
                Id = Guid.NewGuid(),
                Field = _columns?.FirstOrDefault().Field,
            });
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void AddFilter(Guid id, string field)
        {
            _filterDefinitions.Add(new FilterDefinition<T>
            {
                Id = id,
                Field = field,
            });
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void RemoveFilter(Guid id)
        {
            _filterDefinitions.RemoveAll(x => x.Id == id);
            StateHasChanged();
        }

        private SortDirection direction = SortDirection.None;
        private Func<T, object> sortBy = null;

        internal void SetSelectedItem(bool value, T item)
        {
            if (value)
                Selection.Add(item);
            else
                Selection.Remove(item);

            OnSelectedItemsChanged.Invoke(SelectedItems);
            SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        internal void SetSelectAll(bool value)
        {
            if (value)
                Selection = new HashSet<T>(Items);
            else
                Selection.Clear();

            OnSelectedItemsChanged.Invoke(SelectedItems);
            SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        public Interfaces.IForm Validator { get; set; } = new DataGridRowValidator();

        public int GetFilteredItemsCount()
        {
            if (ServerData != null)
                return _server_data.TotalItems;
            return FilteredItems.Count();
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

        public void SetRowsPerPage(int size)
        {
            if (_rowsPerPage == size)
                return;
            _rowsPerPage = size;
            CurrentPage = 0;
            StateHasChanged();
            if (_isFirstRendered)
                InvokeAsync(InvokeServerLoadFunc);
        }

        protected IEnumerable<T> GetItemsOfPage(int n, int pageSize)
        {
            if (n < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (ServerData != null)
                return _server_data.Items;

            return FilteredItems.Skip(n * pageSize).Take(pageSize);
        }

        public async Task SetSortAsync(SortDirection _direction, Func<T, object> _sortBy, string field)
        {
            direction = _direction;
            sortBy = _sortBy;
            OnSortChanged?.Invoke(field);
            await InvokeAsync(InvokeServerLoadFunc);
            StateHasChanged();
        }

        public IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (Items == null)
                return items;

            if (sortBy == null || direction == SortDirection.None)
                return items;

            if (direction == SortDirection.Ascending)
                return items.OrderBy(item => sortBy(item));
            else
                return items.OrderByDescending(item => sortBy(item));
        }

        public void SetSelectedItem(T item)
        {
            if (MultiSelection)
            {
                if (Selection.Contains(item))
                {
                    Selection.Remove(item);
                }
                else
                {
                    Selection.Add(item);
                }

                OnSelectedItemsChanged.Invoke(SelectedItems);
                SelectedItemsChanged.InvokeAsync(SelectedItems);
            }

            SelectedItem = item;
        }

        public void SetEditingItem(T item)
        {
            if (!ReferenceEquals(_editingItem, item))
            {
                EditingCancelled.Invoke();
                _previousEditingItem = _editingItem;
                _editingItem = item;
                StartedEditingItem.Invoke();
            }
        }

        public void ClearEditingItem()
        {
            _editingItem = default(T);
            StateHasChanged();
            //EditingCancelled?.Invoke();
        }

        public void CancelEditingItem()
        {
            EditingCancelled?.Invoke();
            ClearEditingItem();
        }

        public void CommitItemChanges(T item)
        {
            StartedCommittingItemChanges?.Invoke(item);
            // Here, we need to validate at the cellular level...
            ClearEditingItem();
        }

        public void OnRowClicked(MouseEventArgs args, T item, int rowIndex)
        {
            // Manage any previous edited row
            //ManagePreviousEditedRow(this);

            //if (IsHeader || !(Context?.Table.Validator.IsValid ?? true))
            //    return;

            //Context?.Table.SetSelectedItem(Item);

            //// Manage edition the first time the row is clicked and if the table is editable
            //if (!hasBeenClikedFirstTime && IsEditable)
            //{
            //    // Sets hasBeenClikedFirstTime to true
            //    hasBeenClikedFirstTime = true;

            //    // Set to false that the item has been committed
            //    // Set to false that the item has been cancelled
            //    hasBeenCanceled = false;
            //    hasBeenCommitted = false;

            //    // Trigger the preview event
            //    Context?.Table.OnPreviewEditHandler(Item);

            //    // Trigger the row edit preview event
            //    Context.Table.RowEditPreview?.Invoke(Item);
            //}

            //Context?.Table.SetEditingItem(Item);

            //if (Context?.Table.MultiSelection == true && !IsHeader)
            //{
            //    IsChecked = !IsChecked;
            //}
            //Context?.Table.FireRowClickEvent(args, this, Item);

            SetEditingItem(item);
            SetSelectedItem(item);
        }

        public void ToggleFiltersMenu()
        {
            _filtersMenuVisible = !_filtersMenuVisible;
            StateHasChanged();
        }

        internal void ChildStateHasChanged()
        {
            StateHasChanged();
        }

        /// <summary>
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
        public Task ReloadServerData()
        {
            return InvokeServerLoadFunc();
        }

    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDataGrid<T> : MudComponentBase
    {
        private int _currentPage = 0;
        private int? _rowsPerPage;
        private bool _isFirstRendered = false;
        private bool _filtersMenuVisible = false;
        private bool _columnsPanelVisible = false;
        private IEnumerable<T> _items;
        private T _selectedItem;
        private SortDirection _direction = SortDirection.None;
        private Func<T, object> _sortBy = null;
        private HashSet<object> _groupExpansions = new HashSet<object>();
        private List<GroupDefinition<T>> _groups = new List<GroupDefinition<T>>();
        private PropertyInfo[] _properties = typeof(T).GetProperties();

        protected string _classname =>
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
        protected string _tableStyle =>
            new StyleBuilder()
            .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
            .Build();
        protected string _headClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();
        protected string _footClassname => new CssBuilder("mud-table-foot")
            .AddClass(FooterClass).Build();
        protected int numPages
        {
            get
            {
                if (ServerData != null)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                Console.WriteLine("numPages");
                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        internal readonly List<Column<T>> _columns = new List<Column<T>>();
        internal T _editingItem;
        internal int editingItemHash;
        internal T _previousEditingItem;
        internal bool isEditFormOpen;
        internal string GetHorizontalScrollbarStyle() => HorizontalScrollbar ? ";display: block; overflow-x: auto;" : string.Empty;

        // converters
        Converter<bool, bool?> _oppositeBoolConverter = new Converter<bool, bool?>
        {
            SetFunc = value => value ? false : true,
            GetFunc = value => value.HasValue ? !value.Value : true,
        };

        #region Notify Children Delegates

        internal Action<string> SortChangedEvent { get; set; }
        internal Action<HashSet<T>> SelectedItemsChangedEvent { get; set; }
        internal Action<bool> SelectedAllItemsChangedEvent { get; set; }
        internal Action StartedEditingItemEvent { get; set; }
        internal Action EditingCancelledEvent { get; set; }
        public Action PagerStateHasChangedEvent { get; set; }

        #endregion

        #region EventCallbacks

        /// <summary>
        /// Callback is called when a row has been clicked and returns the selected item.
        /// </summary>
        [Parameter] public EventCallback<T> SelectedItemChanged { get; set; }

        /// <summary>
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Callback is called whenever a row is clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowClickEventArgs<T>> RowClick { get; set; }

        /// <summary>
        /// Callback is called when an item has begun to be edited. Returns the item being edited.
        /// </summary>
        [Parameter] public EventCallback<T> StartedEditingItem { get; set; }

        /// <summary>
        /// Callback is called when the process of editing an item has been cancelled. Returns the item which was previously in edit mode.
        /// </summary>
        [Parameter] public EventCallback<T> CancelledEditingItem { get; set; }

        /// <summary>
        /// Callback is called when the changes to an item are committed. Returns the item whose changes were committed.
        /// </summary>
        [Parameter] public EventCallback<T> CommittedItemChanges { get; set; }

        #endregion

        #region Parameters

        /// <summary>
        /// Controls whether data in the DataGrid can be sorted. This is overridable by each column.
        /// </summary>
        [Parameter] public bool Sortable { get; set; } = false;

        /// <summary>
        /// Controls whether data in the DataGrid can be filtered. This is overridable by each column.
        /// </summary>
        [Parameter] public bool Filterable { get; set; } = false;

        /// <summary>
        /// Controls whether columns in the DataGrid can be hidden. This is overridable by each column.
        /// </summary>
        [Parameter] public bool Hideable { get; set; } = false;

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

        /// <summary>
        /// The list of FilterDefinitions that have been added to the data grid. FilterDefinitions are managed by the data
        /// grid automatically when using the built in filter UI. You can also programmatically manage these definitions 
        /// through this collection.
        /// </summary>
        [Parameter] public List<FilterDefinition<T>> FilterDefinitions { get; set; } = new List<FilterDefinition<T>>();

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
        /// Set to true to enable selection of multiple rows. 
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        /// <summary>
        /// When the grid is not read only, you can specify what type of editing mode to use.
        /// </summary>
        [Parameter] public DataGridEditMode? EditMode { get; set; }

        /// <summary>
        /// Allows you to specify the action that will trigger an edit when the EditMode is Form.
        /// </summary>
        [Parameter] public DataGridEditTrigger? EditTrigger { get; set; } = DataGridEditTrigger.Manual;

        /// <summary>
        /// Fine tune the edit dialog.
        /// </summary>
        [Parameter] public DialogOptions EditDialogOptions { get; set; }

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
                if (PagerStateHasChangedEvent != null)
                    InvokeAsync(PagerStateHasChangedEvent);
            }
        }

        /// <summary>
        /// Show a loading animation, if true.
        /// </summary>
        [Parameter] public bool Loading { get; set; }

        /// <summary>
        /// Define if Cancel button is present or not for inline editing.
        /// </summary>
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

        /// <summary>
        /// Allows adding a custom header beyond that specified in the Column component. Add HeaderCell 
        /// components to add a custom header.
        /// </summary>
        [Parameter] public RenderFragment Header { get; set; }

        /// <summary>
        /// The Columns that make up the data grid. Add Column components to this RenderFragment.
        /// </summary>
        [Parameter] public RenderFragment Columns { get; set; }

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
                    InvokeAsync(() => SetRowsPerPageAsync(value));
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
                SelectedItemsChangedEvent?.Invoke(Selection);
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

        /// <summary>
        /// Determines whether grouping of columns is allowed in the data grid.
        /// </summary>
        [Parameter]
        public bool Groupable
        {
            get { return _groupable; }
            set
            {
                if (_groupable != value)
                {
                    _groupable = value;

                    if (!_groupable)
                    {
                        _groups.Clear();
                        _groupExpansions.Clear();

                        foreach (var column in _columns)
                            column.RemoveGrouping();
                    }
                }
            }
        }
        private bool _groupable = false;
        /// <summary>
        /// If set, a grouped column will be expanded by default.
        /// </summary>
        [Parameter] public bool GroupExpanded { get; set; }
        /// <summary>
        /// CSS class for the groups.
        /// </summary>
        [Parameter] public string GroupClass { get; set; }
        /// <summary>
        /// CSS styles for the groups.
        /// </summary>
        [Parameter] public string GroupStyle { get; set; }

        #endregion

        #region Properties

        internal IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (@PagerContent == null)
                {
                    return FilteredItems; // we have no pagination
                }
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
        public HashSet<T> Selection { get; set; } = new HashSet<T>();
        public bool HasPager { get; set; }
        GridData<T> _server_data = new GridData<T>() { TotalItems = 0, Items = Array.Empty<T>() };
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

                foreach (var f in FilterDefinitions)
                {
                    var filterFunc = f.GenerateFilterFunction();
                    items = items.Where(filterFunc);
                }

                return Sort(items);
            }
        }
        public Interfaces.IForm Validator { get; set; } = new DataGridRowValidator();
        internal Column<T> GroupedColumn
        {
            get
            {
                return _columns.FirstOrDefault(x => x.grouping);
            }
        }

        #endregion

        #region Computed Properties

        bool hasFooter
        {
            get
            {
                return _columns.Any(x => !x.Hidden && (x.FooterTemplate != null || x.AggregateDefinition != null));
            }
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isFirstRendered = true;
                GroupItems();
                await InvokeServerLoadFunc();
            }
            else
            {
                PagerStateHasChangedEvent?.Invoke();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #region Methods

        protected IEnumerable<T> GetItemsOfPage(int n, int pageSize)
        {
            if (n < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (ServerData != null)
                return _server_data.Items;

            return FilteredItems.Skip(n * pageSize).Take(pageSize);
        }

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
                SortBy = _sortBy,
                SortDirection = _direction
            };

            _server_data = await ServerData(state);

            if (CurrentPage * RowsPerPage > _server_data.TotalItems)
                CurrentPage = 0;

            Loading = false;
            StateHasChanged();
            PagerStateHasChangedEvent?.Invoke();
        }

        internal void AddColumn(Column<T> column)
        {
            if (column.Tag?.ToString() == "select-column")
                _columns.Insert(0, column);
            else
                _columns.Add(column);
        }

        /// <summary>
        /// Called by the DataGrid when the "Add Filter" button is pressed.
        /// </summary>
        internal void AddFilter()
        {
            FilterDefinitions.Add(new FilterDefinition<T>
            {
                Id = Guid.NewGuid(),
                Field = _columns?.FirstOrDefault().Field,
            });
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void AddFilter(Guid id, string field)
        {
            FilterDefinitions.Add(new FilterDefinition<T>
            {
                Id = id,
                Field = field,
            });
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void RemoveFilter(Guid id)
        {
            FilterDefinitions.RemoveAll(x => x.Id == id);
            GroupItems();
        }

        internal async Task SetSelectedItemAsync(bool value, T item)
        {
            if (value)
                Selection.Add(item);
            else
                Selection.Remove(item);

            SelectedItemsChangedEvent.Invoke(SelectedItems);
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            StateHasChanged();
        }

        internal async Task SetSelectAllAsync(bool value)
        {
            if (value)
                Selection = new HashSet<T>(Items);
            else
                Selection.Clear();

            SelectedItemsChangedEvent?.Invoke(SelectedItems);
            SelectedAllItemsChangedEvent?.Invoke(value);
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            StateHasChanged();
        }

        internal IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (Items == null)
                return items;

            if (_sortBy == null || _direction == SortDirection.None)
                return items;
            if (_direction == SortDirection.Ascending)
                return items.OrderBy(item => _sortBy(item));
            else
                return items.OrderByDescending(item => _sortBy(item));
        }

        internal void ClearEditingItem()
        {
            _editingItem = default(T);
        }

        /// <summary>
        /// This method notifies the consumer that changes to the data have been committed
        /// and what those changes are. This variation of the method is only used by the Cell
        /// when the EditMode is set to cell.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal async Task CommitItemChangesAsync(T item)
        {
            // Here, we need to validate at the cellular level...
            await CommittedItemChanges.InvokeAsync(item);
        }

        /// <summary>
        /// This method notifies the consumer that changes to the data have been committed
        /// and what those changes are. This variation of the method is used when the EditMode 
        /// is anything but Cell since the _editingItem is used.
        /// </summary>
        /// <returns></returns>
        internal async Task CommitItemChangesAsync()
        {
            // Here, we need to validate at the cellular level...
            var found = CurrentPageItems.FirstOrDefault(x => x.GetHashCode() == editingItemHash);

            if (found != null)
            {
                foreach (var property in _properties)
                {
                    property.SetValue(found, property.GetValue(_editingItem));
                }

                Console.WriteLine(JsonSerializer.Serialize(found));

                await CommittedItemChanges.InvokeAsync(found);
                ClearEditingItem();
                isEditFormOpen = false;
            }
        }

        internal async Task OnRowClickedAsync(MouseEventArgs args, T item, int rowIndex)
        {
            await RowClick.InvokeAsync(new DataGridRowClickEventArgs<T>
            {
                MouseEventArgs = args,
                Item = item,
                RowIndex = rowIndex
            });

            if (EditMode != DataGridEditMode.Cell && EditTrigger == DataGridEditTrigger.OnRowClick)
                await SetEditingItemAsync(item);

            await SetSelectedItemAsync(item);
        }

        /// <summary>
        /// Gets the total count of filtered items in the data grid.
        /// </summary>
        /// <returns></returns>
        public int GetFilteredItemsCount()
        {
            if (ServerData != null)
                return _server_data.TotalItems;
            return FilteredItems.Count();
        }

        /// <summary>
        /// Navigates to a specific page when the data grid has an attached data pager.
        /// </summary>
        /// <param name="page"></param>
        public void NavigateTo(Page page)
        {
            switch (page)
            {
                case Page.First:
                    CurrentPage = 0;
                    break;
                case Page.Last:
                    CurrentPage = Math.Max(0, numPages - 1);
                    break;
                case Page.Next:
                    CurrentPage = Math.Min(numPages - 1, CurrentPage + 1);
                    break;
                case Page.Previous:
                    CurrentPage = Math.Max(0, CurrentPage - 1);
                    break;
            }

            GroupItems();
        }

        /// <summary>
        /// Sets the rows displayed per page when the data grid has an attached data pager.
        /// </summary>
        /// <param name="size"></param>
        public async Task SetRowsPerPageAsync(int size)
        {
            if (_rowsPerPage == size)
                return;

            _rowsPerPage = size;
            CurrentPage = 0;
            StateHasChanged();

            if (_isFirstRendered)
                await InvokeAsync(InvokeServerLoadFunc);
        }

        /// <summary>
        /// Sets the sort on the data grid.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="sortBy"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task SetSortAsync(SortDirection direction, Func<T, object> sortBy, string field)
        {
            _direction = direction;
            _sortBy = sortBy;
            SortChangedEvent?.Invoke(field);
            await InvokeServerLoadFunc();
            StateHasChanged();
        }

        /// <summary>
        /// Set the currently selected item in the data grid.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SetSelectedItemAsync(T item)
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

                SelectedItemsChangedEvent?.Invoke(SelectedItems);
                await SelectedItemsChanged.InvokeAsync(SelectedItems);
            }

            SelectedItem = item;
        }

        /// <summary>
        /// Set an item to be edited.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SetEditingItemAsync(T item)
        {
            if (ReadOnly) return;

            editingItemHash = item.GetHashCode();
            EditingCancelledEvent?.Invoke();
            _previousEditingItem = _editingItem;
            _editingItem = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(item));
            StartedEditingItemEvent?.Invoke();
            await StartedEditingItem.InvokeAsync(_editingItem);
            isEditFormOpen = true;
        }

        /// <summary>
        /// Cancel editing an item.
        /// </summary>
        public async Task CancelEditingItemAsync()
        {
            EditingCancelledEvent?.Invoke();
            await CancelledEditingItem.InvokeAsync(_editingItem);
            ClearEditingItem();
            isEditFormOpen = false;
        }

        /// <summary>
        /// Opens or closes the filter panel.
        /// </summary>
        public void ToggleFiltersMenu()
        {
            _filtersMenuVisible = !_filtersMenuVisible;
            StateHasChanged();
        }

        /// <summary>
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
        public Task ReloadServerData()
        {
            return InvokeServerLoadFunc();
        }

        /// <summary>
        /// Opens the filter panel.
        /// </summary>
        public void OpenFilters()
        {
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void HideAllColumns()
        {
            foreach (var column in _columns)
            {
                if (column.Hideable ?? false)
                    column.Hide();
            }

            StateHasChanged();
        }

        internal void ShowAllColumns()
        {
            foreach (var column in _columns)
            {
                if (column.Hideable ?? false)
                    column.Show();
            }

            StateHasChanged();
        }

        public void ShowColumnsPanel()
        {
            _columnsPanelVisible = true;
            StateHasChanged();
        }

        public void HideColumnsPanel()
        {
            _columnsPanelVisible = false;
            StateHasChanged();
        }

        internal void ExternalStateHasChanged()
        {
            StateHasChanged();
        }

        public void GroupItems()
        {
            if (GroupedColumn == null)
            {
                _groups = new List<GroupDefinition<T>>();
                StateHasChanged();
                return;
            }
            
            var groupings = CurrentPageItems.GroupBy(GroupedColumn.groupBy);

            if (_groupExpansions.Count == 0)
            {
                if (GroupExpanded)
                {
                    // We need to initially expand all groups.
                    foreach (var group in groupings)
                    {
                        _groupExpansions.Add(group.Key);
                    }
                }

                _groupExpansions.Add("__initial__");
            }

            // construct the groups
            _groups = groupings.Select(x => new GroupDefinition<T>(x,
                _groupExpansions.Contains(x.Key))).ToList();

            StateHasChanged();
        }

        internal void ChangedGrouping(Column<T> column)
        {
            foreach (var c in _columns)
            {
                if (c.Field != column.Field)
                    c.RemoveGrouping();
            }

            GroupItems();       
        }

        internal void ToggleGroupExpansion(GroupDefinition<T> g)
        {
            if (_groupExpansions.Contains(g.Grouping.Key))
            {
                _groupExpansions.Remove(g.Grouping.Key);
            }
            else
            {
                _groupExpansions.Add(g.Grouping.Key);
            }

            GroupItems();
        }

        public void ExpandAllGroups()
        {
            foreach (var group in _groups)
            {
                group.IsExpanded = true;
            }
        }

        public void CollapseAllGroups()
        {
            foreach (var group in _groups)
            {
                group.IsExpanded = false;
            }
        }

        #endregion

    }
}

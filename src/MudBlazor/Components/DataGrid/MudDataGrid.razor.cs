// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    [CascadingTypeParameter(nameof(T))]
    public partial class MudDataGrid<T> : MudComponentBase
    {
        private int _currentPage = 0;
        internal int? _rowsPerPage;
        private bool _isFirstRendered = false;
        private bool _filtersMenuVisible = false;
        private bool _columnsPanelVisible = false;
        private IEnumerable<T> _items;
        private T _selectedItem;
        internal HashSet<object> _groupExpansions = new HashSet<object>();
        private List<GroupDefinition<T>> _groups = new List<GroupDefinition<T>>();
        internal HashSet<T> _openHierarchies = new HashSet<T>();
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

        protected string _style =>
            new StyleBuilder()
                .AddStyle("overflow-x", "auto", when: HorizontalScrollbar || ColumnResizeMode == ResizeMode.Container)
                .AddStyle("position", "relative", when: hasStickyColumns)
                .AddStyle(Style)
            .Build();

        protected string _tableStyle =>
            new StyleBuilder()
                .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
                .AddStyle("width", "max-content", when: (HorizontalScrollbar || ColumnResizeMode == ResizeMode.Container) && !hasStickyColumns)
                .AddStyle("display", "block", when: HorizontalScrollbar)
            .Build();

        protected string _tableClass =>
            new CssBuilder("mud-table-container")
                .AddClass("cursor-col-resize", when: IsResizing)
            .Build();

        protected string _headClassname => new CssBuilder("mud-table-head")
            .AddClass(HeaderClass).Build();

        protected string _footClassname => new CssBuilder("mud-table-foot")
            .AddClass(FooterClass).Build();

        internal SortDirection GetColumnSortDirection(string columnName)
        {
            if (columnName == null)
            {
                return SortDirection.None;
            }
            else
            {
                SortDefinition<T> sortDefinition = null;
                var ok = SortDefinitions.TryGetValue(columnName, out sortDefinition);

                if (ok)
                {
                    return sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending;
                }
                else
                {
                    return SortDirection.None;
                }
            }
        }

        protected int numPages
        {
            get
            {
                if (ServerData != null)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        public readonly List<Column<T>> RenderedColumns = new List<Column<T>>();
        internal T _editingItem;

        //internal int editingItemHash;
        internal T editingSourceItem;

        internal T _previousEditingItem;
        internal bool isEditFormOpen;

        // converters
        private Converter<bool, bool?> _oppositeBoolConverter = new Converter<bool, bool?>
        {
            SetFunc = value => value ? false : true,
            GetFunc = value => value.HasValue ? !value.Value : true,
        };

        #region Notify Children Delegates

        internal Action<Dictionary<string, SortDefinition<T>>, HashSet<string>> SortChangedEvent { get; set; }
        internal Action<HashSet<T>> SelectedItemsChangedEvent { get; set; }
        internal Action<bool> SelectedAllItemsChangedEvent { get; set; }
        internal Action StartedEditingItemEvent { get; set; }
        internal Action EditingCanceledEvent { get; set; }
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
        /// Callback is called when the process of editing an item has been canceled. Returns the item which was previously in edit mode.
        /// </summary>
        [Parameter] public EventCallback<T> CanceledEditingItem { get; set; }

        /// <summary>
        /// Callback is called when the process of editing an item has been canceled. Returns the item which was previously in edit mode.
        /// NOTE: Obsolete, use CanceledEditingItem instead
        /// </summary>
        [Obsolete("Use CanceledEditingItem instead", false)]
        [Parameter] public EventCallback<T> CancelledEditingItem { get => CanceledEditingItem; set => CanceledEditingItem = value; }

        /// <summary>
        /// Callback is called when the changes to an item are committed. Returns the item whose changes were committed.
        /// </summary>
        [Parameter] public EventCallback<T> CommittedItemChanges { get; set; }

        /// <summary>
        /// Callback is called when a field changes in the dialog MudForm. Only works in EditMode.Form
        /// </summary>
        [Parameter] public EventCallback<FormFieldChangedEventArgs> FormFieldChanged { get; set; }

        #endregion

        #region Parameters

        /// <summary>
        /// Controls whether data in the DataGrid can be sorted. This is overridable by each column.
        /// </summary>
        [Parameter] public SortMode SortMode { get; set; } = SortMode.Multiple;

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

        [Parameter] public bool ShowFilterIcons { get; set; } = true;

        [Parameter] public DataGridFilterMode FilterMode { get; set; }

        [Parameter] public DataGridFilterCaseSensitivity FilterCaseSensitivity { get; set; }

        [Parameter] public RenderFragment<MudDataGrid<T>> FilterTemplate { get; set; }

        /// <summary>
        /// The list of FilterDefinitions that have been added to the data grid. FilterDefinitions are managed by the data
        /// grid automatically when using the built in filter UI. You can also programmatically manage these definitions
        /// through this collection.
        /// </summary>
        [Parameter] public List<FilterDefinition<T>> FilterDefinitions { get; set; } = new List<FilterDefinition<T>>();

        /// <summary>
        /// The list of SortDefinitions that have been added to the data grid. SortDefinitions are managed by the data
        /// grid automatically when using the built in filter UI. You can also programmatically manage these definitions
        /// through this collection.
        /// </summary>
        [Parameter] public Dictionary<string, SortDefinition<T>> SortDefinitions { get; set; } = new Dictionary<string, SortDefinition<T>>();

        /// <summary>
        /// If true, the results are displayed in a Virtualize component, allowing a boost in rendering speed.
        /// </summary>
        [Parameter] public bool Virtualize { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how many additional items will be rendered
        /// before and after the visible region. This help to reduce the frequency of rendering
        /// during scrolling. However, higher values mean that more elements will be present
        /// in the page.
        /// Only used for virtualization.
        /// </summary>
        [Parameter] public int OverscanCount { get; set; } = 3;

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

                // set initial grouping
                if (Groupable)
                {
                    GroupItems();
                }

                // Setup ObservableCollection functionality.
                if (_items is INotifyCollectionChanged)
                {
                    (_items as INotifyCollectionChanged).CollectionChanged += (s, e) =>
                    {
                        if (Groupable)
                            GroupItems();
                    };
                }
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
        /// Defines if columns of the grid can be resized.
        /// </summary>
        [Parameter] public ResizeMode ColumnResizeMode { get; set; }

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
        /// The culture used to represent numeric columns and his filtering input fields.
        /// Each column can override this DataGrid Culture.
        /// </summary>
        [Parameter]
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Row Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment<CellContext<T>> ChildRowContent { get; set; }

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

                        foreach (var column in RenderedColumns)
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

        /// <summary>
        /// Returns the class that will get joined with GroupClass.
        /// </summary>
        [Parameter] public Func<GroupDefinition<T>, string> GroupClassFunc { get; set; }

        /// <summary>
        /// Returns the class that will get joined with GroupStyle.
        /// </summary>
        [Parameter] public Func<GroupDefinition<T>, string> GroupStyleFunc { get; set; }

        /// <summary>
        /// When true, displays the built-in menu icon in the header of the data grid.
        /// </summary>
        [Parameter] public bool ShowMenuIcon { get; set; } = false;

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
        public IEnumerable<T> ServerItems => _server_data.Items;
        private GridData<T> _server_data = new GridData<T>() { TotalItems = 0, Items = Array.Empty<T>() };

        // TODO: When adding one FilterDefinition, this is called once for each RenderedColumn...
        public IEnumerable<T> FilteredItems
        {
            get
            {
                var items = ServerData != null
                    ? _server_data.Items
                    : Items;

                // Quick filtering
                if (QuickFilter != null)
                {
                    items = items.Where(QuickFilter);
                }

                foreach (var f in FilterDefinitions)
                {
                    f.DataGrid = this;
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
                return RenderedColumns.FirstOrDefault(x => x.grouping);
            }
        }

        #endregion

        #region Computed Properties

        private bool hasFooter
        {
            get
            {
                return RenderedColumns.Any(x => !x.Hidden && (x.FooterTemplate != null || x.AggregateDefinition != null));
            }
        }

        private bool hasStickyColumns
        {
            get
            {
                return RenderedColumns.Any(x => x.StickyLeft || x.StickyRight);
            }
        }

        private bool hasHierarchyColumn
        {
            get
            {
                return RenderedColumns.Any(x => x.Tag?.ToString() == "hierarchy-column");
            }
        }

        #endregion

        [UnconditionalSuppressMessage("Trimming", "IL2046: 'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Suppressing because we annotating the whole component with RequiresUnreferencedCodeAttribute for information that generic type must be preserved.")]
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeServerLoadFunc();
                GroupItems();
                if (ServerData == null)
                    StateHasChanged();
                _isFirstRendered = true;
            }
            else
            {
                PagerStateHasChangedEvent?.Invoke();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2046: 'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Suppressing because we annotating the whole component with RequiresUnreferencedCodeAttribute for information that generic type must be preserved.")]
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var sortModeBefore = SortMode;
            await base.SetParametersAsync(parameters);

            if (parameters.TryGetValue(nameof(SortMode), out SortMode sortMode) && sortMode != sortModeBefore)
                await ClearCurrentSortings();
        }

        #region Methods

        protected IEnumerable<T> GetItemsOfPage(int page, int pageSize)
        {
            if (page < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (ServerData != null)
            {
                return QuickFilter != null
                    ? _server_data.Items.Where(QuickFilter)
                    : _server_data.Items;
            }

            return FilteredItems.Skip(page * pageSize).Take(pageSize);
        }

        internal async Task InvokeServerLoadFunc()
        {
            if (ServerData == null)
                return;

            Loading = true;
            StateHasChanged();

            var state = new GridState<T>
            {
                Page = CurrentPage,
                PageSize = RowsPerPage,
                SortDefinitions = SortDefinitions.Values.OrderBy(sd => sd.Index).ToList(),
                // Additional ToList() here to decouple clients from internal list avoiding runtime issues
                FilterDefinitions = FilterDefinitions.ToList()
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
            if (column.Tag?.ToString() == "hierarchy-column")
            {
                RenderedColumns.Insert(0, column);
            }
            else if (column.Tag?.ToString() == "select-column")
            {
                // Position SelectColumn after HierarchyColumn if present
                if (RenderedColumns.Select(x => x.Tag).Contains("hierarchy-column"))
                {
                    RenderedColumns.Insert(1, column);
                }
                else
                {
                    RenderedColumns.Insert(0, column);
                }
            }
            else
            {
                RenderedColumns.Add(column);
            }
        }

        /// <summary>
        /// Called by the DataGrid when the "Add Filter" button is pressed.
        /// </summary>
        public void AddFilter()
        {
            var column = RenderedColumns.FirstOrDefault(x => x.filterable);
            FilterDefinitions.Add(new FilterDefinition<T>
            {
                Id = Guid.NewGuid(),
                DataGrid = this,
                //Field = column?.PropertyName,
                Title = column?.Title,
                //FieldType = column?.PropertyType,
                PropertyExpression = column?.PropertyExpression,
            });
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal void ApplyFilters()
        {
            _filtersMenuVisible = false;
            InvokeServerLoadFunc().AndForget();
        }

        public void ClearFilters()
        {
            FilterDefinitions.Clear();
        }

        public void AddFilter(FilterDefinition<T> definition)
        {
            FilterDefinitions.Add(definition);
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

            await InvokeAsync(() => SelectedItemsChangedEvent.Invoke(SelectedItems));
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            await InvokeAsync(StateHasChanged);
        }

        internal async Task SetSelectAllAsync(bool value)
        {
            var items = ServerData != null
                    ? ServerItems
                    : Items;
                    
            if (value)
                Selection = new HashSet<T>(items);
            else
                Selection.Clear();

            SelectedItemsChangedEvent?.Invoke(SelectedItems);
            SelectedAllItemsChangedEvent?.Invoke(value);
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            StateHasChanged();
        }

        internal IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (null == items || !items.Any())
                return items;

            if (null == SortDefinitions || 0 == SortDefinitions.Count)
                return items;

            IOrderedEnumerable<T> orderedEnumerable = null;

            foreach (var sortDefinition in SortDefinitions.Values.Where(sd => null != sd.SortFunc).OrderBy(sd => sd.Index))
            {
                if (null == orderedEnumerable)
                    orderedEnumerable = sortDefinition.Descending ? items.OrderByDescending(item => sortDefinition.SortFunc(item))
                        : items.OrderBy(item => sortDefinition.SortFunc(item));
                else
                    orderedEnumerable = sortDefinition.Descending ? orderedEnumerable.ThenByDescending(item => sortDefinition.SortFunc(item))
                        : orderedEnumerable.ThenBy(item => sortDefinition.SortFunc(item));
            }

            return orderedEnumerable ?? items;
        }

        internal void ClearEditingItem()
        {
            _editingItem = default;
            editingSourceItem = default;
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

            if (editingSourceItem != null)
            {
                foreach (var property in _properties)
                {
                    if (property.CanWrite)
                        property.SetValue(editingSourceItem, property.GetValue(_editingItem));
                }

                await CommittedItemChanges.InvokeAsync(editingSourceItem);
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
        /// <param name="field">The field.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="sortFunc">The sort function.</param>
        public async Task SetSortAsync(string field, SortDirection direction, Func<T, object> sortFunc)
        {
            var removedSortDefinitions = new HashSet<string>(SortDefinitions.Keys);
            SortDefinitions.Clear();

            var newDefinition = new SortDefinition<T>(field, direction == SortDirection.Descending, 0, sortFunc);
            SortDefinitions[field] = newDefinition;

            // In case sort is just updated make sure to not mark the field as removed
            removedSortDefinitions.Remove(field);

            await InvokeSortUpdates(SortDefinitions, removedSortDefinitions);
        }

        public async Task ExtendSortAsync(string field, SortDirection direction, Func<T, object> sortFunc)
        {
            // If SortMode is not multiple, use the default set approach and don't extend.
            if (SortMode != SortMode.Multiple)
            {
                await SetSortAsync(field, direction, sortFunc);
                return;
            }

            // in case it already exists, just update the current entry
            if (SortDefinitions.TryGetValue(field, out var sortDefinition))
                SortDefinitions[field] = sortDefinition with { Descending = direction == SortDirection.Descending, SortFunc = sortFunc };
            else
            {
                var newDefinition = new SortDefinition<T>(field, direction == SortDirection.Descending, SortDefinitions.Count, sortFunc);
                SortDefinitions[field] = newDefinition;
            }

            await InvokeSortUpdates(SortDefinitions, null);
        }

        public async Task RemoveSortAsync(string field)
        {
            if (!string.IsNullOrWhiteSpace(field) && SortDefinitions.TryGetValue(field, out var definition))
            {
                SortDefinitions.Remove(field);
                foreach (var defToUpdate in SortDefinitions.Where(kvp => kvp.Value.Index > definition.Index).ToList())
                    SortDefinitions[defToUpdate.Key] = defToUpdate.Value with { Index = defToUpdate.Value.Index - 1 };

                await InvokeSortUpdates(SortDefinitions, new HashSet<string>() { field });
            }
        }

        private async Task ClearCurrentSortings()
        {
            var removedSortDefinitions = new HashSet<string>(SortDefinitions.Keys);
            SortDefinitions.Clear();
            await InvokeSortUpdates(SortDefinitions, removedSortDefinitions);
        }

        private async Task InvokeSortUpdates(Dictionary<string, SortDefinition<T>> activeSortDefinitions, HashSet<string> removedSortDefinitions)
        {
            SortChangedEvent?.Invoke(activeSortDefinitions, removedSortDefinitions);

            if (_isFirstRendered)
            {
                await InvokeServerLoadFunc();
                if (ServerData == null)
                    StateHasChanged();
            }
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
        [UnconditionalSuppressMessage("Trimming", "IL2026: Using member 'System.Text.Json.JsonSerializer.Deserialize<T>(string, System.Text.Json.JsonSerializerOptions?)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.", Justification = "Suppressing because T is a type supplied by the user and it is unlikely that it is not referenced by their code.")]
        public async Task SetEditingItemAsync(T item)
        {
            if (ReadOnly) return;

            editingSourceItem = item;
            EditingCanceledEvent?.Invoke();
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
            EditingCanceledEvent?.Invoke();
            await CanceledEditingItem.InvokeAsync(_editingItem);
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

        internal async Task HideAllColumnsAsync()
        {
            foreach (var column in RenderedColumns)
            {
                if (column.Hideable ?? false)
                    await column.HideAsync();
            }

            StateHasChanged();
        }

        internal async Task ShowAllColumnsAsync()
        {
            foreach (var column in RenderedColumns)
            {
                if (column.Hideable ?? false)
                    await column.ShowAsync();
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
                if (_isFirstRendered)
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

            if (_isFirstRendered || ServerData != null)
                StateHasChanged();
        }

        internal void ChangedGrouping(Column<T> column)
        {
            foreach (var c in RenderedColumns)
            {
                if (c.PropertyName != column.PropertyName)
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
                _groupExpansions.Add(group.Grouping.Key);
            }
        }

        public void CollapseAllGroups()
        {
            _groupExpansions.Clear();

            foreach (var group in _groups)
                group.IsExpanded = false;
        }

        #endregion

        internal async Task ToggleHierarchyVisibilityAsync(T item)
        {
            if (_openHierarchies.Contains(item))
            {
                _openHierarchies.Remove(item);
            }
            else
            {
                _openHierarchies.Add(item);
            }

            await InvokeAsync(StateHasChanged);
        }

        #region Resize feature

        [Inject] private IEventListener EventListener { get; set; }
        internal bool IsResizing { get; set; }

        private ElementReference _gridElement;
        private DataGridColumnResizeService<T> _resizeService;

        internal DataGridColumnResizeService<T> ResizeService
        {
            get
            {
                if (null == _resizeService)
                {
                    _resizeService = new DataGridColumnResizeService<T>(this, EventListener);
                }

                return _resizeService;
            }
        }

        internal async Task<bool> StartResizeColumn(HeaderCell<T> headerCell, double clientX)
            => await ResizeService.StartResizeColumn(headerCell, clientX, RenderedColumns, ColumnResizeMode);

        internal async Task<double> GetActualHeight()
        {
            var gridRect = await _gridElement.MudGetBoundingClientRectAsync();
            var gridHeight = gridRect.Height;
            return gridHeight;
        }

        #endregion
    }
}

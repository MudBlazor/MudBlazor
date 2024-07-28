// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Clone;

namespace MudBlazor
{
    /// <summary>
    /// Represents a sortable, filterable data grid with multiselection and pagination.
    /// </summary>
    /// <typeparam name="T">The type of data represented by each row in this grid.</typeparam>
    [CascadingTypeParameter(nameof(T))]
    public partial class MudDataGrid<T> : MudComponentBase, IDisposable
    {
        private Func<IFilterDefinition<T>> _defaultFilterDefinitionFactory = () => new FilterDefinition<T>();
        private int _currentPage = 0;
        internal int? _rowsPerPage;
        private bool _isFirstRendered = false;
        private bool _filtersMenuVisible = false;
        private bool _columnsPanelVisible = false;
        private string _columnsPanelSearch = string.Empty;
        private IEnumerable<T> _items;
        private T _selectedItem;
        private MudForm _editForm;
        private MudVirtualize<T> _mudVirtualize;
        internal Dictionary<object, bool> _groupExpansionsDict = new Dictionary<object, bool>();
        private List<GroupDefinition<T>> _currentPageGroups = new List<GroupDefinition<T>>();
        private List<GroupDefinition<T>> _allGroups = new List<GroupDefinition<T>>();
        internal HashSet<T> _openHierarchies = new HashSet<T>();
        private PropertyInfo[] _properties = typeof(T).GetProperties();
        private MudDropContainer<Column<T>> _dropContainer;
        private MudDropContainer<Column<T>> _columnsPanelDropContainer;
        private CancellationTokenSource _serverDataCancellationTokenSource;
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
                .AddStyle("width", "max-content", when: HorizontalScrollbar || ColumnResizeMode == ResizeMode.Container)
                .AddStyle("overflow", "clip", when: (HorizontalScrollbar || ColumnResizeMode == ResizeMode.Container) && hasStickyColumns)
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
        protected string _headerFooterStyle =>
            new StyleBuilder()
                .AddStyle("position", "sticky", when: hasStickyColumns)
                .AddStyle("left", "0px", when: hasStickyColumns)
            .Build();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Items != null)
            {
                if (ServerData != null)
                {
                    throw new InvalidOperationException(
                        $"{GetType()} can only accept one item source from its parameters. " +
                        $"Do not supply both '{nameof(Items)}' and '{nameof(ServerData)}'.");
                }
                if (VirtualizeServerData != null)
                {
                    throw new InvalidOperationException(
                        $"{GetType()} can only accept one item source from its parameters. " +
                        $"Do not supply both '{nameof(Items)}' and '{nameof(VirtualizeServerData)}'.");
                }
                return;
            }

            if (VirtualizeServerData != null)
            {
                if (ServerData != null)
                {
                    throw new InvalidOperationException(
                        $"{GetType()} can only accept one item source from its parameters. " +
                        $"Do not supply both '{nameof(VirtualizeServerData)}' and '{nameof(ServerData)}'.");
                }
                if (QuickFilter != null)
                {
                    throw new InvalidOperationException(
                        $"Do not supply both '{nameof(VirtualizeServerData)}' and '{nameof(QuickFilter)}'.");
                }
                return;
            }

            if (ServerData != null && QuickFilter != null)
            {
                throw new InvalidOperationException(
                    $"Do not supply both '{nameof(ServerData)}' and '{nameof(QuickFilter)}'.");
            }
        }

        internal SortDirection GetColumnSortDirection(string columnName)
        {
            if (columnName == null)
            {
                return SortDirection.None;
            }
            else
            {
                var ok = SortDefinitions.TryGetValue(columnName, out var sortDefinition);

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
                if (HasServerData)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        internal static bool RenderedColumnsItemsSelector(Column<T> item, string dropZone) => item?.PropertyName == dropZone;

        private static void Swap<TItem>(List<TItem> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private Task ItemUpdatedAsync(MudItemDropInfo<Column<T>> dropItem)
        {
            dropItem.Item.Identifier = dropItem.DropzoneIdentifier;

            var dragAndDropSource = RenderedColumns.SingleOrDefault(rc => rc.PropertyName == dropItem.Item.PropertyName);
            var dragAndDropDestination = RenderedColumns.SingleOrDefault(rc => rc.PropertyName == dropItem.DropzoneIdentifier);
            if (dragAndDropSource != null && dragAndDropDestination != null)
            {
                var dragAndDropSourceIndex = RenderedColumns.IndexOf(dragAndDropSource);
                var dragAndDropDestinationIndex = RenderedColumns.IndexOf(dragAndDropDestination);

                Swap<Column<T>>(RenderedColumns, dragAndDropSourceIndex, dragAndDropDestinationIndex);

                // swap source / destination
                var dest = dragAndDropDestination.HeaderCell.Width;
                var src = dragAndDropSource.HeaderCell.Width;

                dragAndDropSource.HeaderCell.Width = dest;
                dragAndDropDestination.HeaderCell.Width = src;

                StateHasChanged();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// The columns currently being displayed.
        /// </summary>
        public readonly List<Column<T>> RenderedColumns = new List<Column<T>>();

        internal T _editingItem;

        //internal int editingItemHash;
        internal T editingSourceItem;

        internal T _previousEditingItem;
        internal bool isEditFormOpen;

        // converters
        private Converter<bool, bool?> _oppositeBoolConverter = new Converter<bool, bool?>
        {
            SetFunc = value => !value,
            GetFunc = value => !value ?? true,
        };

        #region Notify Children Delegates

        internal Action<Dictionary<string, SortDefinition<T>>, HashSet<string>> SortChangedEvent { get; set; }
        internal Action<HashSet<T>> SelectedItemsChangedEvent { get; set; }
        internal Action<bool> SelectedAllItemsChangedEvent { get; set; }
        internal Action StartedEditingItemEvent { get; set; }
        internal Action EditingCanceledEvent { get; set; }

        /// <summary>
        /// Occurs when the pager state has changed.
        /// </summary>
        public Action PagerStateHasChangedEvent { get; set; }

        #endregion

        #region EventCallbacks

        /// <summary>
        /// Occurs when the <see cref="SelectedItem"/> has changed.
        /// </summary>
        /// <remarks>
        /// This typically occurs when a row has been clicked.
        /// </remarks>
        [Parameter]
        public EventCallback<T> SelectedItemChanged { get; set; }

        /// <summary>
        /// Occurs when the <see cref="SelectedItems"/> have changed.
        /// </summary>
        /// <remarks>
        /// This typically occurs when one or more rows have been clicked when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Occurs when a row has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<DataGridRowClickEventArgs<T>> RowClick { get; set; }

        /// <summary>
        /// Occurs when a row has been right-clicked.
        /// </summary>
        [Parameter]
        public EventCallback<DataGridRowClickEventArgs<T>> RowContextMenuClick { get; set; }

        /// <summary>
        /// Occurs when edit mode begins for an item.
        /// </summary>
        /// <remarks>
        /// If changes are committed, the <see cref="CommittedItemChanges"/> event occurs.  If editing is canceled, the <see cref="CanceledEditingItem"/> occurs.
        /// </remarks>
        [Parameter]
        public EventCallback<T> StartedEditingItem { get; set; }

        /// <summary>
        /// Occurs when editing of an item has been canceled.
        /// </summary>
        [Parameter]
        public EventCallback<T> CanceledEditingItem { get; set; }

        /// <summary>
        /// (Obsolete) Occurs when editing of an item has been canceled.
        /// </summary>
        /// <remarks>
        /// This has been deprecated.  Use <see cref="CanceledEditingItem"/> instead.
        /// </remarks>
        [Obsolete("Use CanceledEditingItem instead", false)]
        [Parameter]
        public EventCallback<T> CancelledEditingItem { get => CanceledEditingItem; set => CanceledEditingItem = value; }

        /// <summary>
        /// Occurs when the user saved changes to an item.
        /// </summary>
        [Parameter]
        public EventCallback<T> CommittedItemChanges { get; set; }

        /// <summary>
        /// Occurs when a field changes in the edit dialog.
        /// </summary>
        /// <remarks>
        /// This event only occurs when <see cref="EditMode"/> is <see cref="DataGridEditMode.Form"/>.
        /// </remarks>
        [Parameter]
        public EventCallback<FormFieldChangedEventArgs> FormFieldChanged { get; set; }

        #endregion

        #region Parameters

        /// <summary>
        /// Allows columns to be reordered via the columns panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool ColumnsPanelReordering { get; set; } = false;

        [CascadingParameter(Name = "RightToLeft")]
        private bool RightToLeft { get; set; }

        /// <summary>
        /// Allows columns to be be reordered via drag-and-drop.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Can be overridden for individual columns via <see cref="Column{T}.DragAndDropEnabled"/>.
        /// </remarks>
        [Parameter]
        public bool DragDropColumnReordering { get; set; } = false;

        /// <summary>
        /// The icon displayed when hovering over a draggable column.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.DragIndicator"/>.  Use the <see cref="DragIndicatorSize"/> property to control this icon's size.
        /// </remarks>
        [Parameter]
        public string DragIndicatorIcon { get; set; } = Icons.Material.Filled.DragIndicator;

        /// <summary>
        /// The size of the icon displayed when hovering over a draggable column.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Small"/>.  Use the <see cref="DragIndicatorIcon"/> property to control which icon is displayed.
        /// </remarks>
        [Parameter]
        public Size DragIndicatorSize { get; set; } = Size.Small;

        /// <summary>
        /// The CSS class applied to columns where a dragged column can be dropped.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>drop-allowed</c>.
        /// </remarks>
        [Parameter]
        public string DropAllowedClass { get; set; } = "drop-allowed";

        /// <summary>
        /// The CSS class applied to columns where a dragged column cannot be dropped.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>drop-not-allowed</c>.
        /// </remarks>
        [Parameter]
        public string DropNotAllowedClass { get; set; } = "drop-not-allowed";

        /// <summary>
        /// Shows drop locations for columns even when not currently dragging a column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool ApplyDropClassesOnDragStarted { get; set; } = false;

        /// <summary>
        /// Sorts data in the grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortMode.Multiple"/>.  Can be overridden for individual columns via <see cref="Column{T}.Sortable"/>.
        /// </remarks>
        [Parameter]
        public SortMode SortMode { get; set; } = SortMode.Multiple;

        /// <summary>
        /// Allows filtering of data in this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Can be overridden for individual columns via <see cref="Column{T}.Filterable"/>.
        /// </remarks>
        [Parameter]
        public bool Filterable { get; set; } = false;

        /// <summary>
        /// Allows columns to be hidden.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Can be overridden for individual columns via <see cref="Column{T}.Hideable"/>.
        /// </remarks>
        [Parameter]
        public bool Hideable { get; set; } = false;

        /// <summary>
        /// Shows options for columns.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Can be overridden for individual columns via <see cref="Column{T}.ShowColumnOptions"/>.
        /// </remarks>
        [Parameter]
        public bool ShowColumnOptions { get; set; } = true;

        /// <summary>
        /// The breakpoint at which the grid switches to mobile layout.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Breakpoint.Xs"/>.  Supported values are <c>None</c>, <c>Xs</c>, <c>Sm</c>, <c>Md</c>, <c>Lg</c> and <c>Xl</c>.
        /// </remarks>
        [Parameter]
        public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Disables rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Square { get; set; }

        /// <summary>
        /// Shows an outline around this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Outlined { get; set; }

        /// <summary>
        /// Shows left and right borders for each column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Bordered { get; set; }

        /// <summary>
        /// The content for any column groupings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property specifies a group of one or more columns in a table for formatting.  For example:
        /// </para>
        /// <para>
        /// table
        ///     colgroup
        ///        col span="2" style="background-color:red"
        ///        col style="background-color:yellow"
        ///      colgroup
        ///      header
        ///      body
        /// table
        /// </para>
        /// </remarks>
        [Parameter]
        public RenderFragment ColGroup { get; set; }

        /// <summary>
        /// Uses compact padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Dense { get; set; }

        /// <summary>
        /// Highlights rows when hovering over them.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Hover { get; set; }

        /// <summary>
        /// Shows alternating row styles.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Striped { get; set; }

        /// <summary>
        /// Fixes the header in place even as the grid is scrolled.
        /// </summary>
        /// <remarks>
        /// Set the <see cref="Height"/> property to make this grid scrollable.
        /// </remarks>
        [Parameter]
        public bool FixedHeader { get; set; }

        /// <summary>
        /// Fixes the footer in place even as the grid is scrolled.
        /// </summary>
        /// <remarks>
        /// Set the <see cref="Height"/> property to make this grid scrollable.
        /// </remarks>
        [Parameter]
        public bool FixedFooter { get; set; }

        /// <summary>
        /// Shows icons for each column filter.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Can be overridden for individual columns via <see cref="Column{T}.ShowFilterIcon"/>.
        /// </remarks>
        [Parameter]
        public bool ShowFilterIcons { get; set; } = true;

        /// <summary>
        /// The way that this grid filters data.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DataGridFilterMode.Simple"/>.
        /// </remarks>
        [Parameter]
        public DataGridFilterMode FilterMode { get; set; }

        /// <summary>
        /// The case sensitivity setting for columns with <c>string</c> values.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DataGridFilterCaseSensitivity.Default"/>.
        /// </remarks>
        [Parameter]
        public DataGridFilterCaseSensitivity FilterCaseSensitivity { get; set; }

        /// <summary>
        /// The template used to display each filter.
        /// </summary>
        [Parameter]
        public RenderFragment<MudDataGrid<T>> FilterTemplate { get; set; }

        /// <summary>
        /// The filter definitions for all columns.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="FilterMode"/> of <see cref="DataGridFilterMode.Simple"/>, this property is managed automatically.
        /// </remarks>
        [Parameter]
        public List<IFilterDefinition<T>> FilterDefinitions { get; set; } = new List<IFilterDefinition<T>>();

        /// <summary>
        /// The sort definitions for all columns.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="FilterMode"/> of <see cref="DataGridFilterMode.Simple"/>, this property is managed automatically.
        /// </remarks>
        [Parameter]
        public Dictionary<string, SortDefinition<T>> SortDefinitions { get; set; } = new Dictionary<string, SortDefinition<T>>();

        /// <summary>
        /// Renders only visible items instead of all items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Only works when <see cref="Height"/> is set.  This feature can improve performance for large data sets.
        /// </remarks>
        [Parameter]
        public bool Virtualize { get; set; }

        /// <summary>
        /// A RenderFragment that will be used as a placeholder when the Virtualize component is asynchronously loading data.
        /// This placeholder is displayed for each item in the data source that is yet to be loaded. Useful for presenting a loading indicator 
        /// in a data grid row while the actual data is being fetched from the server.
        /// </summary>
        [Parameter]
        public RenderFragment RowLoadingContent { get; set; }

        /// <summary>
        /// The number of additional items rendered outside of the visible region when <see cref="Virtualize"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>3</c>.  This value can reduce the amount of rendering during scrolling, but higher values can affect performance.
        /// </remarks>
        [Parameter]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// The height of each row, in pixels, when <see cref="Virtualize"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        public float ItemSize { get; set; } = 50f;

        /// <summary>
        /// The CSS class applied to each row.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.  Note that some CSS settings are overridden by other styles, such as those from <see cref="MudTd"/>.
        /// </remarks>
        [Parameter]
        public string RowClass { get; set; }

        /// <summary>
        /// The CSS styles applied to each row.
        /// </summary>
        /// <remarks>
        /// Some CSS settings are overridden by other styles, such as those from <see cref="MudTd"/>.
        /// </remarks>
        [Parameter]
        public string RowStyle { get; set; }

        /// <summary>
        /// The function which calculates CSS classes for each row.
        /// </summary>
        /// <remarks>
        /// The function passes the current item and row index as parameters.
        /// </remarks>
        [Parameter]
        public Func<T, int, string> RowClassFunc { get; set; }

        /// <summary>
        /// The function which calculates CSS styles for each row.
        /// </summary>
        /// <remarks>
        /// The function passes the current item and row index as parameters.
        /// </remarks>
        [Parameter] public Func<T, int, string> RowStyleFunc { get; set; }

        /// <summary>
        /// Allows selection of more than one row.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// Toggles the row checkbox when the row is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool SelectOnRowClick { get; set; } = true;

        /// <summary>
        /// Controls how cell values are edited.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DataGridEditMode.Cell"/>.  Only works when <see cref="ReadOnly"/> is <c>false</c>.
        /// </remarks>
        [Parameter]
        public DataGridEditMode? EditMode { get; set; }

        /// <summary>
        /// The behavior which begins editing a cell when <see cref="EditMode"/> is <see cref="DataGridEditMode.Form"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DataGridEditTrigger.Manual"/>.
        /// </remarks>
        [Parameter]
        public DataGridEditTrigger? EditTrigger { get; set; } = DataGridEditTrigger.Manual;

        /// <summary>
        /// Any options applied to the edit dialog when <see cref="EditMode"/> is <see cref="DataGridEditMode.Form"/>.
        /// </summary>
        [Parameter]
        public DialogOptions EditDialogOptions { get; set; }

        /// <summary>
        /// The technique used to copy items for editing.
        /// </summary>
        /// <remarks>
        /// During edit mode, a copy of the item is edited, in order to allow an edit to be canceled.  This property controls how that copy is made.
        /// </remarks>
        [Parameter]
        public ICloneStrategy<T> CloneStrategy { get; set; } = SystemTextJsonDeepCloneStrategy<T>.Instance;

        /// <summary>
        /// The data for this grid when <see cref="ServerData"/> is not set.
        /// </summary>
        /// <remarks>
        /// One row will be displayed per item.  Use the <see cref="ServerData"/> function instead of this property to get data on demand.
        /// </remarks>
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
                if (_items is INotifyCollectionChanged changed)
                {
                    changed.CollectionChanged += (s, e) =>
                    {
                        _currentRenderFilteredItemsCache = null;
                        if (Groupable)
                            GroupItems();
                    };
                }
            }
        }

        /// <summary>
        /// Shows a loading animation while querying data.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This property is <c>true</c> while the <see cref="ServerData"/> function is executing.
        /// </remarks>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// Shows a cancel button during inline editing when <see cref="EditMode"/> is <see cref="DataGridEditMode.Cell"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool CanCancelEdit { get; set; } = true;

        /// <summary>
        /// The color of the loading progress indicator while <see cref="Loading" /> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Info"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        public Color LoadingProgressColor { get; set; } = Color.Info;

        /// <summary>
        /// Any custom content to show in this grid's toolbar.
        /// </summary>
        [Parameter]
        public RenderFragment ToolBarContent { get; set; }

        /// <summary>
        /// Shows a horizontal scrollbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// The column resizing behavior for this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ResizeMode.None"/>.  Other values include <see cref="ResizeMode.Column"/> and <see cref="ResizeMode.Container"/>.
        /// </remarks>
        [Parameter]
        public ResizeMode ColumnResizeMode { get; set; }

        /// <summary>
        /// The CSS classes applied to the grid header.
        /// </summary>
        /// <remarks>
        /// These classes are applied to the <c>thead</c> tag of the grid.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string HeaderClass { get; set; }

        /// <summary>
        /// The height of this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values such as <c>30%</c> and <c>500px</c> are allowed.  When <c>null</c>, the grid will try to grow in height.  Must be set when <see cref="Virtualize"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public string Height { get; set; }

        /// <summary>
        /// The CSS classes applied to the grid footer.
        /// </summary>
        /// <remarks>
        /// These classes are applied to the <c>tfoot</c> tag of the grid.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string FooterClass { get; set; }

        /// <summary>
        /// The function which determines visibility of each item in this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This function is typically used to implement a custom search.
        /// </remarks>
        [Parameter]
        public Func<T, bool> QuickFilter { get; set; } = null;

        /// <summary>
        /// Any custom content for this grid's header.
        /// </summary>
        [Parameter]
        public RenderFragment Header { get; set; }

        /// <summary>
        /// Any custom content for this grid's columns.
        /// </summary>
        [Parameter]
        public RenderFragment Columns { get; set; }

        /// <summary>
        /// The culture used to format numeric and date values.  Can be overridden by <see cref="Column{T}.Culture"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        [Parameter]
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// The content shown for each cell.
        /// </summary>
        [Parameter]
        public RenderFragment<CellContext<T>> ChildRowContent { get; set; }

        /// <summary>
        /// The content shown when there are no rows to display.
        /// </summary>
        [Parameter]
        public RenderFragment NoRecordsContent { get; set; }

        /// <summary>
        /// The content shown while <see cref="Loading"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        public RenderFragment LoadingContent { get; set; }

        /// <summary>
        /// The content shown for pagination.
        /// </summary>
        /// <remarks>
        /// A <see cref="MudTablePager"/> is typically added here to break up rows into multiple pages.
        /// </remarks>
        [Parameter]
        public RenderFragment PagerContent { get; set; }

        /// <summary>
        /// The function which gets data for this grid.
        /// </summary>
        /// <remarks>
        /// The function accepts a <see cref="GridState{T}"/> with current sorting, filtering, and pagination parameters.  Then, return a <see cref="GridData{T}"/> with a page of values, and the total (unpaginated) items set in <see cref="GridData{T}.TotalItems"/>.  When set, the <see cref="Items"/> property cannot be set.
        /// </remarks>
        [Parameter]
        public Func<GridState<T>, Task<GridData<T>>> ServerData { get; set; }

        /// <summary>
        /// The function which gets data for this grid.
        /// </summary>
        /// <remarks>
        /// The function accepts a <see cref="GridStateVirtualize{T}"/> with current sorting, filtering, and pagination parameters.
        /// Then, return a <see cref="GridData{T}"/> with a list of values, and the total (unpaginated) items count in <see cref="GridData{T}.TotalItems"/>.
        /// This property is used when you need to display a list without a paginator, 
        /// but with loading data from the server as the scroll position changes.
        /// </remarks>
        [Parameter]
        public Func<GridStateVirtualize<T>, CancellationToken, Task<GridData<T>>> VirtualizeServerData { get; set; }

        /// <summary>
        /// The number of rows displayed for each page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>.  Applies when the <see cref="PagerContent"/> section contains a <see cref="MudTablePager"/>.  When this property changes, the <see cref="RowsPerPageChanged"/> event occurs.
        /// </remarks>
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
        /// Occurs when the <see cref="RowsPerPage"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> RowsPerPageChanged { get; set; }

        /// <summary>
        /// The current page being displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  Applies when the <see cref="PagerContent"/> section contains a <see cref="MudTablePager"/>.
        /// </remarks>
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
        /// Prevents values from being edited.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, the edit behavior is controlled via <see cref="EditMode"/>.
        /// </remarks>
        [Parameter]
        public bool ReadOnly { get; set; } = true;

        /// <summary>
        /// The currently selected rows when <see cref="MultiSelection"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This property can be bound (<c>@bind-SelectedItems</c>) to initially select rows.  Use <see cref="SelectedItem"/> when <see cref="MultiSelection"/> is <c>false</c>.
        /// </remarks>
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
        /// The currently selected row when <see cref="MultiSelection"/> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This property can be bound (<c>@bind-SelectedItem</c>) to initially select a row.  Use <see cref="SelectedItems"/> when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
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
        /// Allows grouping of columns in this grid.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, columns can be used to group sets of items.  Can be overridden for individual columns via <see cref="Column{T}.Groupable"/>.
        /// </remarks>
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
                        _currentPageGroups.Clear();
                        _allGroups.Clear();
                        _groupExpansionsDict.Clear();

                        foreach (var column in RenderedColumns)
                            column.RemoveGrouping().CatchAndLog();
                    }
                }
            }
        }

        private bool _groupable = false;

        /// <summary>
        /// Expands grouped columns by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool GroupExpanded { get; set; }

        /// <summary>
        /// The CSS classes applied to column groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string GroupClass { get; set; }

        /// <summary>
        /// The CSS styles applied to column groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public string GroupStyle { get; set; }

        /// <summary>
        /// The function which determines CSS classes for column groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public Func<GroupDefinition<T>, string> GroupClassFunc { get; set; }

        /// <summary>
        /// The function which determines CSS styles for column groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public Func<GroupDefinition<T>, string> GroupStyleFunc { get; set; }

        /// <summary>
        /// Shows the settings icon in the grid header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an icon will be displayed to control column visibility, collapse all columns, or expand all columns.
        /// </remarks>
        [Parameter]
        public bool ShowMenuIcon { get; set; } = false;

        #endregion

        #region Properties

        internal IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (PagerContent == null)
                {
                    return FilteredItems; // we have no pagination
                }

                if (!HasServerData)
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

        /// <summary>
        /// The currently selected items.
        /// </summary>
        public HashSet<T> Selection { get; set; } = new HashSet<T>();

        /// <summary>
        /// Indicates if a <see cref="MudDataGridPager{T}"/> is present.
        /// </summary>
        public bool HasPager { get; set; }

        /// <summary>
        /// The items returned by the <see cref="ServerData"/> function.
        /// </summary>
        public IEnumerable<T> ServerItems => _server_data.Items;

        private GridData<T> _server_data = new GridData<T>() { TotalItems = 0, Items = Array.Empty<T>() };
        private IEnumerable<T> _currentRenderFilteredItemsCache = null;

        /// <summary>
        /// Defines the ItemsProviderDelegate property, which is necessary for implementing the ServerData methodology with Virtualization.
        /// This property is used to populate items virtually from the server.
        /// </summary>
        internal ItemsProviderDelegate<T> VirtualItemsProvider { get; set; }

        /// <summary>
        /// For unit testing the filtering cache mechanism.
        /// </summary>
        internal uint FilteringRunCount { get; private set; }

        /// <summary>
        /// The items which remain after applying filters.
        /// </summary>
        public IEnumerable<T> FilteredItems
        {
            // TODO: When adding one FilterDefinition, this is called once for each RenderedColumn...
            get
            {
                if (_currentRenderFilteredItemsCache != null) return _currentRenderFilteredItemsCache;
                var items = HasServerData
                    ? _server_data.Items
                    : Items;

                // Quick filtering
                if (QuickFilter != null)
                {
                    items = items.Where(QuickFilter);
                }

                if (!HasServerData)
                {
                    foreach (var filterDefinition in FilterDefinitions)
                    {
                        var filterFunc = filterDefinition.GenerateFilterFunction(new FilterOptions
                        {
                            FilterCaseSensitivity = FilterCaseSensitivity
                        });
                        items = items.Where(filterFunc);
                    }
                }

                _currentRenderFilteredItemsCache = Sort(items).ToList(); // To list to ensure evaluation only once per render
                unchecked { FilteringRunCount++; }
                GroupItems(noStateChange: true);
                return _currentRenderFilteredItemsCache;
            }
        }

        /// <summary>
        /// The validator which validates values in each row.
        /// </summary>
        public Interfaces.IForm Validator { get; set; } = new DataGridRowValidator();

        internal Column<T> GroupedColumn
        {
            get
            {
                return RenderedColumns.FirstOrDefault(x => x.GroupingState.Value);
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

        /// <summary>
        /// This property is determined by checking if the <see cref="ServerData"/> or <see cref="VirtualizeServerData"/> property is not null.
        /// </summary>
        internal bool HasServerData => ServerData != null || VirtualizeServerData != null;

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeServerLoadFunc();
                if (HasServerData)
                    StateHasChanged();
                _isFirstRendered = true;
            }
            else
            {
                PagerStateHasChangedEvent?.Invoke();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var sortModeBefore = SortMode;
            await base.SetParametersAsync(parameters);

            VirtualItemsProviderInitialize();
            if (parameters.TryGetValue(nameof(SortMode), out SortMode sortMode) && sortMode != sortModeBefore)
                await ClearCurrentSortings();
        }

        #region Methods

        protected IEnumerable<T> GetItemsOfPage(int page, int pageSize)
        {
            if (page < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (HasServerData)
            {
                return _server_data.Items;
            }

            return FilteredItems.Skip(page * pageSize).Take(pageSize);
        }

        internal async Task InvokeServerLoadFunc()
        {
            if (!HasServerData)
                return;

            if (VirtualizeServerData != null)
            {
                if (_mudVirtualize != null)
                {
                    // Cancel any prior request
                    CancelServerDataToken();
                    await _mudVirtualize.RefreshDataAsync();
                    StateHasChanged();
                }
                else
                {
                    Loading = true;
                    StateHasChanged();

                    var state = new GridStateVirtualize<T>
                    {
                        StartIndex = 0,
                        Count = 1,
                        SortDefinitions = SortDefinitions.Values.OrderBy(sd => sd.Index).ToList(),
                        // Additional ToList() here to decouple clients from internal list avoiding runtime issues
                        FilterDefinitions = FilterDefinitions.ToList()
                    };

                    // Cancel any prior request
                    CancelServerDataToken();

                    _server_data = await VirtualizeServerData(state, _serverDataCancellationTokenSource.Token);
                    _currentRenderFilteredItemsCache = null;

                    Loading = false;
                    StateHasChanged();
                }
            }
            else
            {
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
                _currentRenderFilteredItemsCache = null;

                if (CurrentPage * RowsPerPage > _server_data.TotalItems)
                    CurrentPage = 0;

                Loading = false;
                StateHasChanged();
                PagerStateHasChangedEvent?.Invoke();
            }
            GroupItems();
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

        internal void CancelServerDataToken()
        {
            try
            {
                _serverDataCancellationTokenSource?.Cancel();
            }
            catch { /*ignored*/ }
            finally
            {
                _serverDataCancellationTokenSource = new CancellationTokenSource();
            }
        }

        internal void RemoveColumn(Column<T> column)
        {
            RenderedColumns.Remove(column);
        }

        internal IFilterDefinition<T> CreateFilterDefinitionInstance()
        {
            return _defaultFilterDefinitionFactory();
        }

        /// <summary>
        /// Specifies the default <see cref="IFilterDefinition{T}"/> to be used by <see cref="AddFilter"/> and <see cref="Column{T}.FilterContext"/>.
        /// </summary>
        public void SetDefaultFilterDefinition<TFilterDefinition>() where TFilterDefinition : IFilterDefinition<T>, new()
        {
            SetDefaultFilterDefinition(() => new TFilterDefinition());
        }

        /// <summary>
        /// Specifies the default <see cref="IFilterDefinition{T}"/> to be used by <see cref="AddFilter"/> and <see cref="Column{T}.FilterContext"/>.
        /// </summary>
        /// <param name="factory">The factory function to create the default filter definition.</param>
        public void SetDefaultFilterDefinition(Func<IFilterDefinition<T>> factory)
        {
            _defaultFilterDefinitionFactory = factory;
        }

        /// <summary>
        /// Occurs when the "Add Filter" button is pressed.
        /// </summary>
        public void AddFilter()
        {
            var column = RenderedColumns.FirstOrDefault(x => x.filterable);
            var filterDefinition = CreateFilterDefinitionInstance();
            filterDefinition.Id = Guid.NewGuid();
            filterDefinition.Title = column?.Title;
            filterDefinition.Column = column;
            FilterDefinitions.Add(filterDefinition);
            _filtersMenuVisible = true;
            StateHasChanged();
        }

        internal Task ApplyFiltersAsync()
        {
            _filtersMenuVisible = false;
            return InvokeServerLoadFunc();
        }

        /// <summary>
        /// Removes all filters from all columns.
        /// </summary>
        public Task ClearFiltersAsync()
        {
            FilterDefinitions.Clear();
            return InvokeServerLoadFunc();
        }

        /// <summary>
        /// Adds the specified filter to the list of filters.
        /// </summary>
        /// <param name="definition">The filter to add.</param>
        public async Task AddFilterAsync(IFilterDefinition<T> definition)
        {
            FilterDefinitions.Add(definition);
            _filtersMenuVisible = true;
            await InvokeServerLoadFunc();
            if (!HasServerData) StateHasChanged();
        }

        internal async Task RemoveFilterAsync(Guid id)
        {
            FilterDefinitions.RemoveAll(x => x.Id == id);
            await InvokeServerLoadFunc();
            GroupItems();
        }

        internal async Task SetSelectedItemAsync(bool value, T item)
        {
            if (value)
            {
                Selection.Add(item);
                SelectedItem = item;
            }
            else
            {
                Selection.Remove(item);
                if (item.Equals(SelectedItem))
                {
                    SelectedItem = default;
                }
            }

            await InvokeAsync(() => SelectedItemsChangedEvent.Invoke(SelectedItems));
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            await InvokeAsync(StateHasChanged);
        }

        internal async Task SetSelectAllAsync(bool value)
        {
            var items = HasServerData
                    ? ServerItems
                    : FilteredItems;

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

            foreach (var sortDefinition in SortDefinitions.Values.Where(sd => sd.SortFunc != null).OrderBy(sd => sd.Index))
            {
                if (null == orderedEnumerable)
                    orderedEnumerable = sortDefinition.Descending ? items.OrderByDescending(item => sortDefinition.SortFunc(item), sortDefinition.Comparer)
                        : items.OrderBy(item => sortDefinition.SortFunc(item), sortDefinition.Comparer);
                else
                    orderedEnumerable = sortDefinition.Descending ? orderedEnumerable.ThenByDescending(item => sortDefinition.SortFunc(item), sortDefinition.Comparer)
                        : orderedEnumerable.ThenBy(item => sortDefinition.SortFunc(item), sortDefinition.Comparer);
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
            await _editForm.Validate();
            if (!_editForm.IsValid)
            {
                return;
            }

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
            await RowClick.InvokeAsync(new DataGridRowClickEventArgs<T>(args, item, rowIndex));

            if (EditMode != DataGridEditMode.Cell && EditTrigger == DataGridEditTrigger.OnRowClick)
                await SetEditingItemAsync(item);

            await SetSelectedItemAsync(item);
        }

        internal async Task OnContextMenuClickedAsync(MouseEventArgs args, T item, int rowIndex)
        {
            await RowContextMenuClick.InvokeAsync(new DataGridRowClickEventArgs<T>(args, item, rowIndex));
        }

        /// <summary>
        /// Gets the total count of filtered items in the data grid.
        /// </summary>
        /// <returns>
        /// The number of items remaining after applying filters.  When <see cref="ServerData"/> is in use, the <see cref="GridData{T}.TotalItems"/> value is returned.
        /// </returns>
        public int GetFilteredItemsCount()
        {
            if (HasServerData)
                return _server_data.TotalItems;
            return FilteredItems.Count();
        }

        /// <summary>
        /// Navigates to a page when this grid has a <see cref="MudDataGridPager{T}"/>.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
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
        /// Sets the <see cref="RowsPerPage"/> when this grid contains a <see cref="MudDataGridPager{T}"/>.
        /// </summary>
        /// <param name="size">The new page size.</param>
        public Task SetRowsPerPageAsync(int size) => SetRowsPerPageAsync(size, true);

        /// <summary>
        /// Sets the <see cref="RowsPerPage"/> when this grid contains a <see cref="MudDataGridPager{T}"/>.
        /// </summary>
        /// <param name="size">The new page size.</param>
        /// <param name="resetPage">When <c>true</c>, resets <see cref="CurrentPage"/> to 0.</param>
        public async Task SetRowsPerPageAsync(int size, bool resetPage)
        {
            if (_rowsPerPage == size)
                return;

            _rowsPerPage = size;

            if (resetPage)
                _currentPage = 0;

            await RowsPerPageChanged.InvokeAsync(_rowsPerPage.Value);

            StateHasChanged();

            if (_isFirstRendered)
                await InvokeAsync(InvokeServerLoadFunc);
        }

        /// <summary>
        /// Replaces the sorting behavior for a field.
        /// </summary>
        /// <param name="field">The field to sort.</param>
        /// <param name="direction">The direction to sort results.</param>
        /// <param name="sortFunc">The function which sorts results.</param>
        /// <param name="comparer">The comparer used for custom comparisons.</param>
        public async Task SetSortAsync(string field, SortDirection direction, Func<T, object> sortFunc, IComparer<object> comparer = null)
        {
            var removedSortDefinitions = new HashSet<string>(SortDefinitions.Keys);
            SortDefinitions.Clear();

            var newDefinition = new SortDefinition<T>(field, direction == SortDirection.Descending, 0, sortFunc, comparer);
            SortDefinitions[field] = newDefinition;

            // In case sort is just updated make sure to not mark the field as removed
            removedSortDefinitions.Remove(field);

            await InvokeSortUpdates(SortDefinitions, removedSortDefinitions);
        }

        /// <summary>
        /// Adds or replaces a sort behavior depending on the <see cref="SortMode"/>.
        /// </summary>
        /// <param name="field">The field to sort.</param>
        /// <param name="direction">The direction to sort results.</param>
        /// <param name="sortFunc">The function which sorts results.</param>
        /// <param name="comparer">The comparer used for custom comparisons.</param>
        /// <remarks>
        /// When the <see cref="SortMode"/> is <see cref="SortMode.Single"/>, this method replaces the sort column.  Otherwise, this sort is appended to any existing sort column.
        /// </remarks>
        public async Task ExtendSortAsync(string field, SortDirection direction, Func<T, object> sortFunc, IComparer<object> comparer = null)
        {
            // If SortMode is not multiple, use the default set approach and don't extend.
            if (SortMode != SortMode.Multiple)
            {
                await SetSortAsync(field, direction, sortFunc, comparer);
                return;
            }

            // in case it already exists, just update the current entry
            if (SortDefinitions.TryGetValue(field, out var sortDefinition))
                SortDefinitions[field] = sortDefinition with { Descending = direction == SortDirection.Descending, SortFunc = sortFunc, Comparer = comparer };
            else
            {
                var newDefinition = new SortDefinition<T>(field, direction == SortDirection.Descending, SortDefinitions.Count, sortFunc, comparer);
                SortDefinitions[field] = newDefinition;
            }

            await InvokeSortUpdates(SortDefinitions, null);
        }

        /// <summary>
        /// Removes a sort behavior from the list of sort behaviors.
        /// </summary>
        /// <param name="field">The name of the field to remove.</param>
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

        /// <summary>
        /// Clears all current sort definitions.
        /// </summary>
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
                if (!HasServerData)
                    StateHasChanged();
            }
        }

        private void VirtualItemsProviderInitialize()
        {
            if (VirtualItemsProvider != null || VirtualizeServerData == null)
            {
                return;
            }

            VirtualItemsProvider = async request =>
            {
                var stateFunc = (int startIndex, int count) => new GridStateVirtualize<T>
                {
                    StartIndex = startIndex,
                    Count = count,
                    SortDefinitions = SortDefinitions.Values.OrderBy(sd => sd.Index).ToList(),
                    // Additional ToList() here to decouple clients from internal list avoiding runtime issues
                    FilterDefinitions = FilterDefinitions.ToList()
                };

                _server_data = await VirtualizeServerData(
                    stateFunc(request.StartIndex, request.Count),
                    request.CancellationToken
                );

                if (request.StartIndex > 0 && _server_data.TotalItems < request.StartIndex + request.Count)
                {
                    _server_data = await VirtualizeServerData(
                        stateFunc(0, request.Count),
                        request.CancellationToken
                    );
                }

                _currentRenderFilteredItemsCache = null;

                return new ItemsProviderResult<T>(
                    _server_data.Items,
                    _server_data.TotalItems);
            };
        }

        /// <summary>
        /// Set the currently selected item in the data grid.
        /// </summary>
        /// <param name="item">The item to select.</param>
        /// <remarks>
        /// When <see cref="MultiSelection"/> is <c>true</c> and <see cref="SelectOnRowClick"/> is <c>true</c>, the <see cref="SelectedItems"/> are updated.  The <see cref="SelectedItem"/> is also updated.
        /// </remarks>
        public async Task SetSelectedItemAsync(T item)
        {
            if (MultiSelection && SelectOnRowClick)
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
        /// Starts editing for the specified item.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        public async Task SetEditingItemAsync(T item)
        {
            if (ReadOnly) return;

            editingSourceItem = item;
            EditingCanceledEvent?.Invoke();
            _previousEditingItem = _editingItem;
            _editingItem = CloneStrategy.CloneObject(item);
            StartedEditingItemEvent?.Invoke();
            await StartedEditingItem.InvokeAsync(_editingItem);
            isEditFormOpen = true;
        }

        /// <summary>
        /// Cancels the current editing of an item.
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
        /// Reloads grid data by calling the <see cref="ServerData"/> function.
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
                if (column.hideable)
                    await column.HideAsync();
            }
            DropContainerHasChanged();
            StateHasChanged();
        }

        internal async Task ShowAllColumnsAsync()
        {
            foreach (var column in RenderedColumns)
            {
                if (column.hideable)
                    await column.ShowAsync();
            }
            DropContainerHasChanged();
            StateHasChanged();
        }

        /// <summary>
        /// Shows a panel that lets you show, hide, filter, group, sort and re-arrange columns.
        /// </summary>
        public void ShowColumnsPanel()
        {
            _columnsPanelVisible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Hides the columns panel.
        /// </summary>
        public void HideColumnsPanel()
        {
            _columnsPanelVisible = false;
            StateHasChanged();
        }

        private Task ColumnOrderUpdated(MudItemDropInfo<Column<T>> dropItem)
        {
            RenderedColumns.Remove(dropItem.Item);
            RenderedColumns.Insert(dropItem.IndexInZone, dropItem.Item);
            DropContainerHasChanged();

            return Task.CompletedTask;
        }

        private void ColumnUp(Column<T> column)
        {
            var index = RenderedColumns.IndexOf(column);
            if (index > 0)
            {
                RenderedColumns.RemoveAt(index);
                RenderedColumns.Insert(index - 1, column);
            }
            DropContainerHasChanged();
        }

        private void ColumnDown(Column<T> column)
        {
            var index = RenderedColumns.IndexOf(column);
            if (index < RenderedColumns.Count - 1)
            {
                RenderedColumns.RemoveAt(index);
                RenderedColumns.Insert(index + 1, column);
            }
            DropContainerHasChanged();
        }

        internal void DropContainerHasChanged()
        {
            _dropContainer?.Refresh();
            _columnsPanelDropContainer?.Refresh();
        }

        /// <summary>
        /// Performs grouping of the current items.
        /// </summary>
        /// <param name="noStateChange">Defaults to <c>false</c>.  When <c>true</c>, calls to <c>StateHasChanged</c> will not occur.</param>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        public void GroupItems(bool noStateChange = false)
        {
            if (!noStateChange)
                DropContainerHasChanged();

            if (GroupedColumn?.groupBy == null)
            {
                _currentPageGroups = new List<GroupDefinition<T>>();
                _allGroups = new List<GroupDefinition<T>>();
                if (_isFirstRendered && !noStateChange)
                    StateHasChanged();
                return;
            }

            var currentPageGroupings = CurrentPageItems.GroupBy(GroupedColumn.groupBy);

            // Maybe group Items to keep groups expanded after clearing a filter?
            var allGroupings = FilteredItems.GroupBy(GroupedColumn.groupBy).ToArray();

            if (GetFilteredItemsCount() > 0)
            {
                foreach (var group in allGroupings)
                {
                    _groupExpansionsDict.TryAdd(group.Key, GroupExpanded);
                }
            }

            // construct the groups
            _currentPageGroups = currentPageGroupings.Select(x => new GroupDefinition<T>(x,
                _groupExpansionsDict[x.Key])).ToList();

            _allGroups = allGroupings.Select(x => new GroupDefinition<T>(x,
                _groupExpansionsDict[x.Key])).ToList();

            if ((_isFirstRendered || HasServerData) && !noStateChange)
                StateHasChanged();
        }

        internal async Task ChangedGrouping(Column<T> column)
        {
            foreach (var c in RenderedColumns)
            {
                if (c.PropertyName != column.PropertyName)
                    await c.RemoveGrouping();
            }

            GroupItems();
        }

        internal void ToggleGroupExpansion(GroupDefinition<T> g)
        {
            if (_groupExpansionsDict.TryGetValue(g.Grouping.Key, out var value))
            {
                _groupExpansionsDict[g.Grouping.Key] = !value;
            }

            GroupItems();
        }

        /// <summary>
        /// Expands all groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        public void ExpandAllGroups()
        {
            foreach (var group in _allGroups)
            {
                group.Expanded = true;
                _groupExpansionsDict[group.Grouping.Key] = true;
            }
            GroupItems();
        }

        /// <summary>
        /// Collapses all groups.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="Groupable"/> is <c>true</c>.
        /// </remarks>
        public void CollapseAllGroups()
        {
            foreach (var group in _allGroups)
            {
                group.Expanded = false;
                _groupExpansionsDict[group.Grouping.Key] = false;
            }
            GroupItems();
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
                return _resizeService ??= new DataGridColumnResizeService<T>(this, EventListener);
            }
        }

        internal async Task<bool> StartResizeColumn(HeaderCell<T> headerCell, double clientX)
            => await ResizeService.StartResizeColumn(headerCell, clientX, RenderedColumns, ColumnResizeMode, RightToLeft);

        internal async Task<double> GetActualHeight()
        {
            var gridRect = await _gridElement.MudGetBoundingClientRectAsync();
            var gridHeight = gridRect.Height;
            return gridHeight;
        }

        #endregion

        /// <summary>
        /// Releases resources used by this data grid.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _serverDataCancellationTokenSource?.Dispose();
        }
    }
}

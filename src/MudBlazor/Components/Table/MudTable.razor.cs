using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor
{
#nullable enable
    // note: the MudTable code is split. Everything depending on the type parameter T of MudTable<T> is here in MudTable<T>

    /// <summary>
    /// A sortable, filterable table with multiselection and pagination.
    /// </summary>
    /// <typeparam name="T">The type of item displayed in this table.</typeparam>
    public partial class MudTable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudTableBase, IDisposable
    {
        private T? _selectedItem;
        private IEnumerable<T>? _items;
        private IEnumerable<T>? _preEditSort;
        private IEqualityComparer<T>? _comparer;
        private TableGroupDefinition<T>? _groupBy;
        private bool _currentRenderFilteredItemsCached;
        private CancellationTokenSource? _cancellationTokenSrc;
        private TableData<T> _serverData = new() { TotalItems = 0, Items = Array.Empty<T>() };

        [MemberNotNullWhen(true, nameof(_preEditSort))]
        private bool HasPreEditSort => _preEditSort is not null;

        [MemberNotNullWhen(true, nameof(ServerData))]
        internal override bool HasServerData => ServerData is not null;

        /// <summary>
        /// The columns for each row in this table.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudTd"/> to define columns, and <c>context</c> to access item properties for each column.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T>? RowTemplate { get; set; }

        /// <summary>
        /// The optional nested content underneath each row.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudTr"/> and <see cref="MudTd"/> to define the child content, and <c>context</c> to access item properties.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T>? ChildRowContent { get; set; }

        /// <summary>
        /// The columns for each row when a row is being edited.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudTd"/> to define columns, and <c>context</c> to access item properties for each column.  Typically looks similar to rows in <see cref="RowTemplate"/> but with edit components.
        /// </remarks>        
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public RenderFragment<T>? RowEditingTemplate { get; set; }

        /// <summary>
        /// The function which determines if a row can be edited.
        /// </summary>
        /// <remarks>
        /// Make the function return <c>true</c> to allow editing, and <c>false</c> to prevent it.  When no value is set, all rows are considered editable.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public Func<T, bool>? RowEditableFunc { get; set; }

        private bool IsItemEditable(T item)
        {
            if (!Editable)
            {
                return false;
            }

            if (RowEditableFunc == null)
            {
                return true;
            }

            return RowEditableFunc(item);
        }

        #region Code for column based approach

        /// <summary>
        /// The columns for each row in this table.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment<T>? Columns { get; set; }

        // Workaround because "where T : new()" didn't work with Blazor components
        // T must have a default constructor, otherwise we cannot show headers when Items collection
        // is empty
        protected T? Def
        {
            get
            {
                T? t1 = default;
                if (t1 == null)
                {
                    return Activator.CreateInstance<T>();
                }

                return default;
            }
        }

        protected override void OnInitialized()
        {
            if (HasServerData)
            {
                Loading = true;
            }
        }

        #endregion

        /// <summary>
        /// The content shown when there are no rows to display.
        /// </summary>
        /// <remarks>
        /// No <see cref="MudTr"/> or <see cref="MudTd"/> is necessary.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment? NoRecordsContent { get; set; }

        /// <summary>
        /// The content shown while table data is loading and the table has no rows.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment? LoadingContent { get; set; }

        /// <summary>
        /// Shows a horizontal scroll bar if the content exceeds the maximum width.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Typically <c>true</c> for tables with more columns than can fit on the screen.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public bool HorizontalScrollbar { get; set; }

        internal string GetHorizontalScrollbarStyle() => HorizontalScrollbar ? ";display: block; overflow-x: auto;" : string.Empty;

        /// <summary>
        /// The data to display.
        /// </summary>
        /// <remarks>
        /// When set, <see cref="ServerData"/> should not be set.  Use that property to get data from a back end.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public IEnumerable<T>? Items
        {
            get => _items;
            set
            {
                if (_items == value)
                    return;
                _items = value;
                if (Context?.PagerStateHasChanged != null)
                    InvokeAsync(Context.PagerStateHasChanged);
            }
        }

        /// <summary>
        /// The function which determines whether an item should be displayed.
        /// </summary>
        /// <remarks>
        /// Typically used to implement custom filtering.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Filtering)]
        public Func<T, bool>? Filter { get; set; } = null;

        /// <summary>
        /// Occurs when a row has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<TableRowClickEventArgs<T>> OnRowClick { get; set; }

        internal override async Task FireRowClickEventAsync(MouseEventArgs args, MudTr row, object? o)
        {
            var item = default(T);
            try
            {
                item = (T?)o;
            }
            catch (Exception) { /*ignore*/}
            await OnRowClick.InvokeAsync(new TableRowClickEventArgs<T>(args, row, item));
        }

        /// <summary>
        /// Occurs when the pointer hovers over a row.
        /// </summary>
        [Parameter]
        public EventCallback<TableRowHoverEventArgs<T>> OnRowMouseEnter { get; set; }

        internal override bool HasRowMouseEnterEventHandler => OnRowMouseEnter.HasDelegate;

        internal override async Task FireRowMouseEnterEventAsync(PointerEventArgs args, MudTr row, object? o)
        {
            var item = default(T);
            try
            {
                item = (T?)o;
            }
            catch (Exception) { /*ignore*/}
            await OnRowMouseEnter.InvokeAsync(new TableRowHoverEventArgs<T>(args, row, item));
        }

        /// <summary>
        /// Occurs when the pointer is no longer hovering over a row.
        /// </summary>
        [Parameter]
        public EventCallback<TableRowHoverEventArgs<T>> OnRowMouseLeave { get; set; }

        internal override bool HasRowMouseLeaveEventHandler => OnRowMouseLeave.HasDelegate;

        internal override async Task FireRowMouseLeaveEventAsync(PointerEventArgs args, MudTr row, object? o)
        {
            var item = default(T);
            try
            {
                item = (T?)o;
            }
            catch (Exception) { /*ignore*/}
            await OnRowMouseLeave.InvokeAsync(new TableRowHoverEventArgs<T>(args, row, item));
        }

        /// <summary>
        /// The function which returns CSS classes for a row.
        /// </summary>
        /// <remarks>
        /// The current item and row index are provided to the function.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string>? RowClassFunc { get; set; }

        /// <summary>
        /// The function which returns CSS styles for a row.
        /// </summary>
        /// <remarks>
        /// The current item and row index are provided to the function.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string>? RowStyleFunc { get; set; }

        /// <summary>
        /// The currently selected item when <c>MultiSelection</c> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// When the selected item changes, the <see cref="SelectedItemChanged"/> event occurs.  When <c>MultiSelection</c> is <c>true</c>, use the <see cref="SelectedItems"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public T? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_comparer != null && _comparer.Equals(SelectedItem, value))
                    return;
                if (EqualityComparer<T>.Default.Equals(SelectedItem, value))
                    return;
                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Occurs when <see cref="SelectedItem"/> has changed.
        /// </summary>
        /// <remarks>
        /// Occurs when <c>MultiSelection</c> is <c>false</c>, otherwise <see cref="SelectedItemsChanged"/> occurs.
        /// </remarks>
        [Parameter]
        public EventCallback<T> SelectedItemChanged { get; set; }

        /// <summary>
        /// The currently selected item when <c>MultiSelection</c> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// When the selected items change, the <see cref="SelectedItemsChanged"/> event occurs.  When <c>MultiSelection</c> is <c>false</c>, use the <see cref="SelectedItem"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public HashSet<T>? SelectedItems
        {
            get
            {
                if (!MultiSelection)
                    if (_selectedItem is null)
                        return new HashSet<T>(Array.Empty<T>(), _comparer);
                    else
                        return new HashSet<T>(new T[] { _selectedItem }, _comparer);

                return Context.Selection;
            }
            set
            {
                if (value == Context.Selection)
                    return;
                if (value == null)
                {
                    if (Context.Selection.Count == 0)
                        return;
                    Context.Selection = new HashSet<T>(_comparer);
                }
                else
                    Context.Selection = value;
                SelectedItemsChanged.InvokeAsync(Context.Selection);
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Checks if the row is selected.
        /// If there is set a Comparer, uses the comparer, otherwise uses a direct contains
        /// </summary>
        protected bool IsCheckedRow(T item)
            => _comparer is not null ? Context.Selection.Any(x => _comparer.Equals(x, item)) : Context.Selection.Contains(item);

        /// <summary>
        /// The comparer used to determine selected items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T>? Comparer
        {
            get => _comparer;
            set
            {
                if (value == _comparer) return;
                _comparer = value;
                // Apply comparer and (selected values are refreshed in the Context.Comparer setter)
                Context.Comparer = _comparer;
            }
        }

        /// <summary>
        /// Occurs when <see cref="SelectedItems"/> has changed.
        /// </summary>
        /// <remarks>
        /// Occurs when <c>MultiSelection</c> is <c>true</c>, otherwise <see cref="SelectedItemChanged"/> occurs.
        /// </remarks>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Defines how rows are grouped together.
        /// </summary>
        /// <remarks>
        /// Groups can be defined in multiple levels.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public TableGroupDefinition<T>? GroupBy
        {
            get => _groupBy;
            set
            {
                _groupBy = value;
                if (_groupBy != null)
                    _groupBy.Context = Context;
            }
        }

        /// <summary>
        /// The content for the header of each group when <see cref="GroupBy"/> is set.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudTd"/> to define the cells in the group header.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>>? GroupHeaderTemplate { get; set; }

        /// <summary>
        /// The custom CSS classes to apply to each group header row.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupHeaderClass { get; set; }

        /// <summary>
        /// The custom CSS styles to apply to each group header row.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupHeaderStyle { get; set; }

        /// <summary>
        /// The custom CSS classes to apply to each group footer row.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupFooterClass { get; set; }

        /// <summary>
        /// The custom CSS styles to apply to each group footer row.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupFooterStyle { get; set; }

        /// <summary>
        /// The content for the footer of each group when <see cref="GroupBy"/> is set.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudTd"/> to define the cells in the group footer.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>>? GroupFooterTemplate { get; set; }

        /// <summary>
        /// For unit testing the filtering cache mechanism.
        /// </summary>
        internal uint FilteringRunCount { get; private set; } = 0;

        /// <summary>
        /// The table items after filters are applied.
        /// </summary>
        /// <remarks>
        /// When <see cref="ServerData"/> is set, the latest items are returned.  When <see cref="Filter"/> is set, filtered items are returned.
        /// </remarks>
        public IEnumerable<T> FilteredItems
        {
            get
            {
                if (_currentRenderFilteredItemsCached)
                    return _preEditSort ?? Array.Empty<T>();
                if (Editing && HasPreEditSort)
                    return _preEditSort;
                if (HasServerData)
                    _preEditSort = _serverData.Items?.ToList();
                else if (Filter == null)
                    _preEditSort = Context.Sort(Items)?.ToList();
                else
                    _preEditSort = Context.Sort(Items?.Where(Filter))?.ToList();

                _currentRenderFilteredItemsCached = true;
                unchecked { FilteringRunCount++; }

                return _preEditSort ?? Array.Empty<T>();
            }
        }

        protected IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (@PagerContent == null)
                    return FilteredItems; // we have no pagination
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

        protected IEnumerable<T> GetItemsOfPage(int n, int pageSize)
        {
            if (n < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (HasServerData)
                return _serverData.Items ?? Array.Empty<T>();

            return FilteredItems.Skip(n * pageSize).Take(pageSize);
        }

        protected override int NumPages
        {
            get
            {
                if (HasServerData)
                    return (int)Math.Ceiling(_serverData.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        /// <summary>
        /// Gets the number of filtered items.
        /// </summary>
        /// <returns>When <see cref="ServerData"/> is set, the total number of items, otherwise the number of <see cref="FilteredItems"/>.</returns>
        public override int GetFilteredItemsCount()
        {
            if (HasServerData)
                return _serverData.TotalItems;
            return FilteredItems.Count();
        }

        /// <summary>
        /// Sets the <see cref="SelectedItem"/>.
        /// </summary>
        /// <param name="item">The new value to set.</param>
        public override void SetSelectedItem(object? item)
        {
            SelectedItem = item.As<T>();
        }

        /// <summary>
        /// Sets the item to edit.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        public override void SetEditingItem(object? item)
        {
            if (!ReferenceEquals(_editingItem, item))
                _editingItem = item;
        }

        /// <summary>
        /// Gets whether <see cref="Items"/> contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>When <c>true</c>, the item was found, otherwise <c>false</c>.</returns>
        public override bool ContainsItem(object? item)
        {
            var t = item.As<T>();
            if (t is null)
                return false;
            return Items?.Contains(t) ?? false;
        }

        /// <summary>
        /// Raises the <see cref="SelectedItemsChanged"/> event.
        /// </summary>
        public override void UpdateSelection() => SelectedItemsChanged.InvokeAsync(SelectedItems);

        /// <summary>
        /// Gets the current state of the table.
        /// </summary>
        public override TableContext TableContext
        {
            get
            {
                Context.Table = this;
                Context.TableStateHasChanged = StateHasChanged;
                return Context;
            }
        }

        /// <summary>
        /// Gets the current state of the table.
        /// </summary>
        public TableContext<T> Context { get; } = new();

        private void OnRowCheckboxChanged(bool checkedState, T item)
        {
            if (checkedState)
                Context.Selection.Add(item);
            else
                Context.Selection.Remove(item);

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        internal override void OnHeaderCheckboxClicked(bool checkedState)
        {
            if (checkedState)
            {
                foreach (var item in FilteredItems)
                    Context.Selection.Add(item);
            }
            else
                Context.Selection.Clear();

            Context.UpdateRowCheckBoxes();

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        /// <summary>
        /// Gets the sorted and paginated data for the table.
        /// </summary>
        /// <remarks>
        /// Use the provided <see cref="TableState"/> to request items for a specific page index, page size, sort column, and sort order.<br />  
        /// Return a <see cref="TableData{T}"/> which contains the requested page of items and the total number of items (excluding pagination).<br />
        /// Forward the <see cref="CancellationToken"/> to methods which support it such as <c>HttpClient</c> and <c>DbContext</c> to cancel ongoing requests.<br />  
        /// When this parameter is set, <see cref="Items"/> and <see cref="Filter"/> should not be set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public Func<TableState, CancellationToken, Task<TableData<T>>>? ServerData { get; set; }

        private void CancelToken()
        {
            try
            {
                _cancellationTokenSrc?.Cancel();
            }
            catch { /*ignored*/ }
            finally
            {
                _cancellationTokenSrc = new CancellationTokenSource();
            }
        }

        internal override async Task InvokeServerLoadFunc()
        {
            if (!HasServerData)
                return;

            Loading = true;
            await InvokeAsync(StateHasChanged);
            var label = Context.CurrentSortLabel;

            var state = new TableState
            {
                Page = CurrentPage,
                PageSize = RowsPerPage,
                SortDirection = Context.SortDirection,
                SortLabel = label?.SortLabel
            };

            // Cancel any prior request
            CancelToken();

            // Get data via the ServerData function
            _serverData = await ServerData(state, _cancellationTokenSrc!.Token);

            if (CurrentPage * RowsPerPage > _serverData.TotalItems)
                CurrentPage = 0;

            Loading = false;
            await InvokeAsync(StateHasChanged);
            Context?.PagerStateHasChanged?.Invoke();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender)
                Context?.PagerStateHasChanged?.Invoke();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
                await InvokeServerLoadFunc();
            TableContext.UpdateRowCheckBoxes(updateGroups: false);
        }

        /// <summary>
        /// Reloads this table's data via the <see cref="ServerData"/> function.
        /// </summary>
        /// <remarks>
        /// Use this method to reload this table's results when <see cref="ServerData"/> is set.
        /// </remarks>
        public Task ReloadServerData()
        {
            return InvokeServerLoadFunc();
        }

        internal override bool Editable { get => (RowEditingTemplate != null) || (Columns != null); }

        //GROUPING:
        private IEnumerable<IGrouping<object, T>> GroupItemsPage
        {
            get
            {
                return GetItemsOfGroup(GroupBy, CurrentPageItems);
            }
        }

        internal IEnumerable<IGrouping<object, T>> GetItemsOfGroup(TableGroupDefinition<T>? parent, IEnumerable<T>? sourceList)
        {
            if (parent is null || sourceList is null)
            {
                return new List<IGrouping<object, T>>();
            }

            if (parent.Selector is not null)
            {
                return sourceList.GroupBy(parent.Selector).ToList();
            }

            return new List<IGrouping<object, T>>();
        }

        internal void OnGroupHeaderCheckboxClicked(bool checkedState, IEnumerable<T> items)
        {
            if (checkedState)
            {
                foreach (var item in items)
                    Context.Selection.Add(item);
            }
            else
            {
                foreach (var item in items)
                    Context.Selection.Remove(item);
            }

            Context.UpdateRowCheckBoxes();

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        /// <summary>
        /// Expands all groups within this table.
        /// </summary>
        public void ExpandAllGroups()
        {
            ToggleExpandGroups(expand: true);
        }

        /// <summary>
        /// Collapses all groups within this table.
        /// </summary>
        public void CollapseAllGroups()
        {
            ToggleExpandGroups(expand: false);
        }

        private void ToggleExpandGroups(bool expand)
        {
            if (_groupBy is not null)
            {
                _groupBy.IsInitiallyExpanded = expand;
                Context?.GroupRows.Where(gr => gr.GroupDefinition == _groupBy).ToList().ForEach(gr => gr.Expanded = _groupBy.IsInitiallyExpanded);
            }
        }

        private string ClearFilterCache()
        {
            _currentRenderFilteredItemsCached = false;
            return "";
        }

        /// <summary>
        /// Releases resources used by this table.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                _cancellationTokenSrc?.Cancel();
            }
            catch { /*ignored*/ }
            _cancellationTokenSrc?.Dispose();
        }
    }
}

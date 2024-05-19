﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor
{
#nullable enable
    // note: the MudTable code is split. Everything depending on the type parameter T of MudTable<T> is here in MudTable<T>
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
        /// Defines how a table row looks like. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T>? RowTemplate { get; set; }

        /// <summary>
        /// Row Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T>? ChildRowContent { get; set; }

        /// <summary>
        /// Defines how a table row looks like in edit mode (for selected row). Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public RenderFragment<T>? RowEditingTemplate { get; set; }

        /// <summary>
        /// A function that returns whether or not an item should be editable. Use to remove editing for certain rows.
        /// </summary>
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
        /// Defines how a table column looks like. Columns components should inherit from MudBaseColumn
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
        /// <summary>
        /// Creates a default Column renderfragment if there is no templates defined
        /// </summary>
        protected override void OnInitialized()
        {
            if (HasServerData)
            {
                Loading = true;
            }
        }

        #endregion
        /// <summary>
        /// Defines the table body content when there are no matching records found
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment? NoRecordsContent { get; set; }

        /// <summary>
        /// Defines the table body content  the table has no rows and is loading
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment? LoadingContent { get; set; }

        /// <summary>
        /// Defines if the table has a horizontal scrollbar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public bool HorizontalScrollbar { get; set; }

        internal string GetHorizontalScrollbarStyle() => HorizontalScrollbar ? ";display: block; overflow-x: auto;" : string.Empty;

        /// <summary>
        /// The data to display in the table. MudTable will render one row per item
        /// </summary>
        /// 
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
        /// A function that returns whether or not an item should be displayed in the table. You can use this to implement your own search function.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Filtering)]
        public Func<T, bool>? Filter { get; set; } = null;

        /// <summary>
        /// Row click event.
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
        /// Row hover start event.
        /// </summary>
        [Parameter]
        public EventCallback<TableRowHoverEventArgs<T>> OnRowMouseEnter { get; set; }

        internal override bool HasRowPointerEnterEventHandler => OnRowMouseEnter.HasDelegate;

        internal override async Task FireRowPointerEnterEventAsync(PointerEventArgs args, MudTr row, object? o)
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
        /// Row hover stop event.
        /// </summary>
        [Parameter]
        public EventCallback<TableRowHoverEventArgs<T>> OnRowMouseLeave { get; set; }

        internal override bool HasRowPointerLeaveEventHandler => OnRowMouseLeave.HasDelegate;

        internal override async Task FireRowPointerLeaveEventAsync(PointerEventArgs args, MudTr row, object? o)
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
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string>? RowClassFunc { get; set; }

        /// <summary>
        /// Returns the style that will get joined with RowStyle. Takes the current item and row index.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string>? RowStyleFunc { get; set; }

        /// <summary>
        /// Returns the item which was last clicked on in single selection mode (that is, if MultiSelection is false)
        /// </summary>
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
        /// Callback is called when a row has been clicked and returns the selected item.
        /// </summary>
        [Parameter]
        public EventCallback<T> SelectedItemChanged { get; set; }

        /// <summary>
        /// If MultiSelection is true, this returns the currently selected items. You can bind this property and the initial content of the HashSet you bind it to will cause these rows to be selected initially.
        /// </summary>
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
        /// The Comparer to use for comparing selected items internally.
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
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Defines data grouping parameters. It can has N hierarchical levels
        /// </summary>
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
        /// Defines how a table grouping row header looks like. It works only when GroupBy is not null. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>>? GroupHeaderTemplate { get; set; }

        /// <summary>
        /// Defines custom CSS classes for using on Group Header's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupHeaderClass { get; set; }

        /// <summary>
        /// Defines custom styles for using on Group Header's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupHeaderStyle { get; set; }

        /// <summary>
        /// Defines custom CSS classes for using on Group Footer's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupFooterClass { get; set; }

        /// <summary>
        /// Defines custom styles for using on Group Footer's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string? GroupFooterStyle { get; set; }

        /// <summary>
        /// Defines how a table grouping row footer looks like. It works only when GroupBy is not null. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>>? GroupFooterTemplate { get; set; }

        /// <summary>
        /// For unit testing the filtering cache mechanism.
        /// </summary>
        internal uint FilteringRunCount { get; private set; } = 0;

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

        public override int GetFilteredItemsCount()
        {
            if (HasServerData)
                return _serverData.TotalItems;
            return FilteredItems.Count();
        }

        public override void SetSelectedItem(object? item)
        {
            SelectedItem = item.As<T>();
        }

        public override void SetEditingItem(object? item)
        {
            if (!ReferenceEquals(_editingItem, item))
                _editingItem = item;
        }

        public override bool ContainsItem(object? item)
        {
            var t = item.As<T>();
            if (t is null)
                return false;
            return Items?.Contains(t) ?? false;
        }

        public override void UpdateSelection() => SelectedItemsChanged.InvokeAsync(SelectedItems);

        public override TableContext TableContext
        {
            get
            {
                Context.Table = this;
                Context.TableStateHasChanged = StateHasChanged;
                return Context;
            }
        }

        // TableContext provides shared functionality between all table sub-components
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
        /// Supply an async function which (re)loads filtered, paginated and sorted data from server.
        /// Table will await this func and update based on the returned TableData.
        /// Used only with ServerData
        /// </summary>
        /// <remarks>
        /// MudTable will automatically control loading animation visibility if ServerData is set.
        /// See <see cref="MudTableBase.Loading"/>.  Forward the provided cancellation token to
        /// methods which support it.
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
            StateHasChanged();
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
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
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

        public void ExpandAllGroups()
        {
            ToggleExpandGroups(expand: true);
        }

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
            _cancellationTokenSrc?.Dispose();
        }
    }
}

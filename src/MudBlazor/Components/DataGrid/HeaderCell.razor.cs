// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a cell displayed at the top of a <see cref="MudDataGrid{T}"/> column.
    /// </summary>
    /// <typeparam name="T">The kind of item managed by the grid.</typeparam>
    public partial class HeaderCell<T> : MudComponentBase, IDisposable
    {
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// The <see cref="MudDataGrid{T}"/> which contains this header cell.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T> DataGrid { get; set; }

        /// <summary>
        /// Shows this cell only in the header area.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the header cell display in the header area and will not display cells with data like a normal column.  This property is set automatically when adding a header to the grid manually.
        /// </remarks>
        [CascadingParameter(Name = "IsOnlyHeader")]
        public bool IsOnlyHeader { get; set; } = false;

        /// <summary>
        /// The column associated with this header cell.
        /// </summary>
        [Parameter]
        public Column<T> Column { get; set; }

        /// <summary>
        /// The content within this header cell.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private SortDirection _initialDirection;
        private bool _selected;

        /// <summary>
        /// The direction to sort values in this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.
        /// </remarks>
        [Parameter]
        public SortDirection SortDirection
        {
            get => _initialDirection;
            set
            {
                _initialDirection = value;
            }
        }

        private string _classname =>
            new CssBuilder(Column?.HeaderClass)
                .AddClass(Column?.HeaderClassFunc?.Invoke(DataGrid?.CurrentPageItems ?? Enumerable.Empty<T>()))
                .AddClass(Column?.headerClassname)
                .AddClass(Class)
            .Build();

        private string _style =>
            new StyleBuilder()
                .AddStyle(Column?.HeaderStyleFunc?.Invoke(DataGrid?.CurrentPageItems ?? Enumerable.Empty<T>()))
                .AddStyle(Column?.HeaderStyle)
                .AddStyle("width", Width?.ToPx(), when: Width.HasValue)
                .AddStyle(Style)
            .Build();

        private string _resizerStyle =>
            new StyleBuilder()
                .AddStyle("height", _resizerHeight?.ToPx() ?? "100%")
                .AddStyle(Style)
            .Build();

        private string _resizerClass =>
            new CssBuilder()
                .AddClass("mud-resizing", when: _isResizing)
                .AddClass("mud-resizer")
            .Build();

        private string _sortHeaderClass =>
            new CssBuilder()
                .AddClass("sortable-column-header")
                .AddClass("cursor-pointer", when: !_isResizing)
            .Build();

        private string _optionsClass =>
            new CssBuilder()
                .AddClass("column-options")
                .AddClass("cursor-pointer", when: !_isResizing)
            .Build();

        private ElementReference _headerElement;

        /// <summary>
        /// The width for this header cell, in pixels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public double? Width { get; internal set; }

        private double? _resizerHeight;
        private bool _isResizing;
        private bool _filtersMenuVisible;

        #region Computed Properties and Functions

        private string computedTitle
        {
            get
            {
                return Column.Title;
            }
        }

        private bool sortable
        {
            get
            {
                return Column?.Sortable ?? DataGrid?.SortMode != SortMode.None;
            }
        }

        private bool resizable
        {
            get
            {
                return Column?.Resizable ?? DataGrid.ColumnResizeMode != ResizeMode.None;
            }
        }

        private bool filterable
        {
            get
            {
                return Column?.Filterable ?? DataGrid?.Filterable ?? true;
            }
        }

        private bool showFilterIcon
        {
            get
            {
                if (!filterable)
                    return false;

                return Column?.ShowFilterIcon ?? DataGrid?.ShowFilterIcons ?? true;
            }
        }

        private bool hideable
        {
            get
            {
                return Column?.Hideable ?? DataGrid?.Hideable ?? false;
            }
        }

        private bool groupable
        {
            get
            {
                return Column?.Groupable ?? DataGrid?.Groupable ?? false;
            }
        }

        private bool showColumnOptions
        {
            get
            {
                if (!sortable && !filterable && !groupable)
                    return false;
                if (!sortable && DataGrid.FilterMode == DataGridFilterMode.ColumnFilterRow)
                    return false;

                return Column?.ShowColumnOptions ?? DataGrid?.ShowColumnOptions ?? true;
            }
        }

        internal string sortIconClass
        {
            get
            {
                return _initialDirection switch
                {
                    SortDirection.Descending => "sort-direction-icon mud-direction-desc",
                    SortDirection.Ascending => "sort-direction-icon mud-direction-asc",
                    _ => "sort-direction-icon"
                };
            }
        }

        internal bool hasFilter
        {
            get
            {
                if (DataGrid == null)
                    return false;

                return DataGrid.FilterDefinitions.Any(x => x.Column?.PropertyName == Column?.PropertyName && x.Operator != null);
            }
        }

        #endregion
        protected override async Task OnParametersSetAsync()
        {
            if (Column != null)
            {
                Column.HeaderCell = this;

                if (Column.filterable)
                {
                    Column.FilterContext.HeaderCell = this;
                }

            }
            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _initialDirection = Column?.InitialDirection ?? SortDirection.None;

            if (_initialDirection != SortDirection.None)
            {
                // set initial sort
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.PropertyName, _initialDirection, Column.GetLocalSortFunc()));
            }

            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent += OnGridSortChanged;
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }
        }

        #region Events

        /// <summary>
        /// This is triggered by the DataGrid when a sort is applied
        /// e.g. from another HeaderCell.
        /// </summary>
        /// <param name="activeSorts">The active sorts.</param>
        /// <param name="removedSorts">The removed sorts.</param>
        private void OnGridSortChanged(Dictionary<string, SortDefinition<T>> activeSorts, HashSet<string> removedSorts)
        {
            if (Column == null || (Column.Sortable.HasValue && !Column.Sortable.Value) || string.IsNullOrWhiteSpace(Column.PropertyName))
                return;

            if (null != removedSorts && removedSorts.Contains(Column.PropertyName))
            {
                MarkAsUnsorted();
            }
            else if (activeSorts.TryGetValue(Column.PropertyName, out var sortDefinition))
            {
                Column.SortIndex = sortDefinition.Index;
            }
        }

        private void OnSelectedAllItemsChanged(bool value)
        {
            _selected = value;
            StateHasChanged();
        }

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            _selected = items.Count == DataGrid.GetFilteredItemsCount();
            StateHasChanged();
        }

        private async Task OnResizerPointerDown(PointerEventArgs args)
        {
            if (!resizable)
                return;

            if (args.Detail > 1) // Double click clears the width, hence setting it to minimum size.
            {
                Width = null;
                return;
            }

            _isResizing = await DataGrid.StartResizeColumn(this, args.ClientX);
        }

        private async Task OnResizerPointerOver()
        {
            if (!_isResizing)
                _resizerHeight = await DataGrid?.GetActualHeight();
        }

        private void OnResizerPointerLeave()
        {
            if (!_isResizing)
                _resizerHeight = null;
        }

        internal async Task<double> UpdateColumnWidth(double targetWidth, double gridHeight, bool finishResize)
        {
            if (targetWidth > 0)
            {
                _resizerHeight = gridHeight;
                Width = targetWidth;
                await InvokeAsync(StateHasChanged);
            }

            if (finishResize)
            {
                _isResizing = false;
                await InvokeAsync(StateHasChanged);
            }

            return await GetCurrentCellWidth();
        }

        internal async Task<double> GetCurrentCellWidth()
        {
            var boundingRect = await _headerElement.MudGetBoundingClientRectAsync();
            return boundingRect.Width;
        }

        internal async Task SortChangedAsync(MouseEventArgs args)
        {
            if (args.AltKey)
            {
                if (_initialDirection != SortDirection.None)
                    await RemoveSortAsync();

                return;
            }

            _initialDirection = _initialDirection switch
            {
                SortDirection.Ascending => SortDirection.Descending,
                _ => SortDirection.Ascending
            };

            DataGrid.DropContainerHasChanged();

            if (args.CtrlKey && DataGrid.SortMode == SortMode.Multiple)
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.PropertyName, _initialDirection, Column.GetLocalSortFunc(), Column.Comparer));
            else
                await InvokeAsync(() => DataGrid.SetSortAsync(Column.PropertyName, _initialDirection, Column.GetLocalSortFunc(), Column.Comparer));
        }

        internal async Task RemoveSortAsync()
        {
            await InvokeAsync(() => DataGrid.RemoveSortAsync(Column.PropertyName));
            MarkAsUnsorted();
            DataGrid.DropContainerHasChanged();
        }

        internal void AddFilter()
        {
            var filterDefinition = Column?.FilterContext.FilterDefinition;
            if (DataGrid.FilterMode == DataGridFilterMode.Simple && filterDefinition != null)
            {
                if (DataGrid.FilterDefinitions.All(x => x.Title != filterDefinition.Title))
                {
                    DataGrid.FilterDefinitions.Add(filterDefinition.Clone());
                }
                DataGrid.OpenFilters();
            }
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
            {
                _filtersMenuVisible = true;
                DataGrid.DropContainerHasChanged();
            }
        }

        internal void OpenFilters()
        {
            if (DataGrid.FilterMode == DataGridFilterMode.Simple)
                DataGrid.OpenFilters();
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
            {
                _filtersMenuVisible = true;
                DataGrid.DropContainerHasChanged();
            }
        }

        internal async Task ApplyFilterAsync()
        {
            DataGrid.FilterDefinitions.Add(Column.FilterContext.FilterDefinition);
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ApplyFilterAsync(IFilterDefinition<T> filterDefinition)
        {
            DataGrid.FilterDefinitions.Add(filterDefinition);
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ApplyFiltersAsync(IEnumerable<IFilterDefinition<T>> filterDefinitions)
        {
            DataGrid.FilterDefinitions.AddRange(filterDefinitions);
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFilterAsync()
        {
            Column.FilterContext.FilterDefinition.Value = null;
            await DataGrid.RemoveFilterAsync(Column.FilterContext.FilterDefinition.Id);
            if (!DataGrid.HasServerData)
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFilterAsync(IFilterDefinition<T> filterDefinition)
        {
            await DataGrid.RemoveFilterAsync(filterDefinition.Id);
            if (!DataGrid.HasServerData)
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        internal async Task ClearFiltersAsync(IEnumerable<IFilterDefinition<T>> filterDefinitions)
        {
            DataGrid.FilterDefinitions.RemoveAll(x => filterDefinitions.Any(y => y.Id == x.Id));
            if (DataGrid.HasServerData)
            {
                await DataGrid.ReloadServerData();
            }
            else
            {
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
            _filtersMenuVisible = false;
            DataGrid.DropContainerHasChanged();
        }

        private async Task CheckedChangedAsync(bool value)
        {
            await DataGrid?.SetSelectAllAsync(value);
        }

        internal async Task HideColumnAsync()
        {
            if (Column != null)
            {
                await Column.HideAsync();
                ((IMudStateHasChanged)DataGrid).StateHasChanged();
            }
        }

        internal async Task GroupColumn()
        {
            await Column?.SetGrouping(true);
            DataGrid.DropContainerHasChanged();
        }

        internal async Task UngroupColumn()
        {
            await Column?.SetGrouping(false);
            DataGrid.DropContainerHasChanged();
        }

        private void MarkAsUnsorted()
        {
            _initialDirection = SortDirection.None;
            Column.SortIndex = -1;
        }

        #endregion

        /// <summary>
        /// Releases resources used by this header cell.
        /// </summary>
        public void Dispose()
        {
            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent -= OnGridSortChanged;
                DataGrid.SelectedAllItemsChangedEvent -= OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent -= OnSelectedItemsChanged;
            }
        }
    }
}

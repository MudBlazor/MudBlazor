﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class HeaderCell<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }
        [CascadingParameter(Name = "IsOnlyHeader")] public bool IsOnlyHeader { get; set; } = false;

        [Parameter] public Column<T> Column { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private SortDirection _initialDirection;
        private Type _dataType;
        private bool _isSelected;

        private string _classname =>
            new CssBuilder(Column?.HeaderClass)
                .AddClass(Column?.headerClassname)
                .AddClass(Class)
            .Build();

        private string _style =>
            new StyleBuilder()
                .AddStyle(Column?.HeaderStyle)
                .AddStyle("width", _width?.ToPx(), when: _width.HasValue)
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

        private double? _width;
        private double? _resizerHeight;
        private bool _isResizing;
        private bool _filtersMenuVisible;

        #region Computed Properties and Functions

        private string computedTitle
        {
            get
            {
                return Column.Title ?? Column.Field;
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

                return Column?.ShowColumnOptions ?? DataGrid?.ShowColumnOptions ?? true;
            }
        }

        private string sortIconClass
        {
            get
            {
                if (_initialDirection == SortDirection.Descending)
                {
                    return "sort-direction-icon mud-direction-desc";
                }
                else if (_initialDirection == SortDirection.Ascending)
                {
                    return "sort-direction-icon mud-direction-asc";
                }
                else
                {
                    return "sort-direction-icon";
                }
            }
        }

        private bool hasFilter
        {
            get
            {
                if (DataGrid == null)
                    return false;

                return DataGrid.FilterDefinitions.Any(x => x.Field == Column.Field && x.Operator != null && x.Value != null);
            }
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _initialDirection = Column?.InitialDirection ?? SortDirection.None;

            if (_initialDirection != SortDirection.None)
            {
                // set initial sort
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.Field, _initialDirection, Column.GetLocalSortFunc()));
            }

            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent += OnGridSortChanged;
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }

            if (null != Column)
            {
                Column.HeaderCell = this;

                if (Column.filterable)
                {
                    Column.filterContext._headerCell = this;
                }
            }
        }

        internal void GetDataType()
        {
            var p = typeof(T).GetProperty(Column?.Field);
            _dataType = p.GetType();
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
            if ((Column.Sortable.HasValue && !Column.Sortable.Value) || string.IsNullOrWhiteSpace(Column.Field))
                return;

            if (null != removedSorts && removedSorts.Contains(Column.Field))
                MarkAsUnsorted();
            else if (activeSorts.TryGetValue(Column.Field, out var sortDefinition))
                Column.SortIndex = sortDefinition.Index;
        }

        private void OnSelectedAllItemsChanged(bool value)
        {
            _isSelected = value;
            StateHasChanged();
        }

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            _isSelected = items.Count == DataGrid.GetFilteredItemsCount();
            StateHasChanged();
        }

        private async Task OnResizerMouseDown(MouseEventArgs args)
        {
            if (!resizable)
                return;

            if (args.Detail > 1) // Double click clears the width, hence setting it to minimum size.
            {
                _width = null;
                return;
            }

            _isResizing = await DataGrid.StartResizeColumn(this, args.ClientX);
        }

        private async Task OnResizerMouseOver()
        {
            if (!_isResizing)
                _resizerHeight = await DataGrid?.GetActualHeight();
        }

        private void OnResizerMouseLeave()
        {
            if (!_isResizing)
                _resizerHeight = null;
        }

        internal async Task<double> UpdateColumnWidth(double targetWidth, double gridHeight, bool finishResize)
        {
            if (targetWidth > 0)
            {
                _resizerHeight = gridHeight;
                _width = targetWidth;
                if (finishResize)
                {
                    _isResizing = false;
                }

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

            if (args.CtrlKey && DataGrid.SortMode == SortMode.Multiple)
                await InvokeAsync(() => DataGrid.ExtendSortAsync(Column.Field, _initialDirection, Column.GetLocalSortFunc()));
            else
                await InvokeAsync(() => DataGrid.SetSortAsync(Column.Field, _initialDirection, Column.GetLocalSortFunc()));
        }

        internal async Task RemoveSortAsync()
        {
            await InvokeAsync(() => DataGrid.RemoveSortAsync(Column.Field));
            MarkAsUnsorted();
        }

        internal void AddFilter()
        {
            if (DataGrid.FilterMode == DataGridFilterMode.Simple)
                DataGrid.AddFilter(Guid.NewGuid(), Column?.Field);
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
                _filtersMenuVisible = true;
        }

        internal void OpenFilters()
        {
            if (DataGrid.FilterMode == DataGridFilterMode.Simple)
                DataGrid.OpenFilters();
            else if (DataGrid.FilterMode == DataGridFilterMode.ColumnFilterMenu)
                _filtersMenuVisible = true;
        }

        internal void ApplyFilter()
        {
            DataGrid.FilterDefinitions.Add(Column.filterContext.FilterDefinition);
            DataGrid.ExternalStateHasChanged();
            _filtersMenuVisible = false;
        }

        internal void ApplyFilter(FilterDefinition<T> filterDefinition)
        {
            DataGrid.FilterDefinitions.Add(filterDefinition);
            DataGrid.ExternalStateHasChanged();
            _filtersMenuVisible = false;
        }

        internal void ApplyFilters(IEnumerable<FilterDefinition<T>> filterDefinitions)
        {
            DataGrid.FilterDefinitions.AddRange(filterDefinitions);
            DataGrid.ExternalStateHasChanged();
            _filtersMenuVisible = false;
        }

        internal void ClearFilter()
        {
            DataGrid.RemoveFilter(Column.filterContext.FilterDefinition.Id);
            Column.filterContext.FilterDefinition.Value = null;
            _filtersMenuVisible = false;
        }

        internal void ClearFilter(FilterDefinition<T> filterDefinition)
        {
            DataGrid.RemoveFilter(filterDefinition.Id);
            _filtersMenuVisible = false;
        }

        internal void ClearFilters(IEnumerable<FilterDefinition<T>> filterDefinitions)
        {
            DataGrid.FilterDefinitions.RemoveAll(x => filterDefinitions.Any(y => y.Id == x.Id));
            DataGrid.ExternalStateHasChanged();
            _filtersMenuVisible = false;
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
                DataGrid.ExternalStateHasChanged();
            }
        }

        internal void GroupColumn()
        {
            Column?.SetGrouping(true);
        }

        internal void UngroupColumn()
        {
            Column?.SetGrouping(false);
        }

        private void MarkAsUnsorted()
        {
            _initialDirection = SortDirection.None;
            Column.SortIndex = -1;
        }

        #endregion

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

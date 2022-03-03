// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
                .AddStyle(Style)
            .Build();

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
                return Column?.Sortable ?? DataGrid?.Sortable ?? true;
            }
        }
        private bool filterable
        {
            get
            {
                return Column?.Filterable ?? DataGrid?.Filterable ?? true;
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
                await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, Column.SortBy, Column.Field));
            }

            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent += ClearSort;
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }
        }

        internal void GetDataType()
        {
            var p = typeof(T).GetProperty(Column?.Field);
            _dataType = p.GetType();
        }

        #region Events

        /// <summary>
        /// Removes the sort icon from the HeaderCell. This is triggered by the DataGrid when a sort is applied
        /// from another HeaderCell to remove all other sorts.
        /// </summary>
        /// <param name="field">The field that is the currently set sort.</param>
        private void ClearSort(string field)
        {
            if (Column?.Field != field)
                _initialDirection = SortDirection.None;
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

        internal async Task SortChangedAsync()
        {
            if (_initialDirection == SortDirection.None)
                _initialDirection = SortDirection.Ascending;
            else if (_initialDirection == SortDirection.Ascending)
                _initialDirection = SortDirection.Descending;
            else if (_initialDirection == SortDirection.Descending)
                _initialDirection = SortDirection.None;

            await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, Column?.SortBy, Column?.Field));
        }

        internal async Task RemoveSortAsync()
        {
            _initialDirection = SortDirection.None;
            await InvokeAsync(() => DataGrid.SetSortAsync(SortDirection.None, Column?.SortBy, Column?.Field));
        }

        internal void AddFilter()
        {
            DataGrid.AddFilter(Guid.NewGuid(), Column?.Field);
        }

        internal void OpenFilters()
        {
            DataGrid.OpenFilters();
        }

        private async Task CheckedChangedAsync(bool value)
        {
            await DataGrid?.SetSelectAllAsync(value);
        }

        internal async Task HideColumnAsync()
        {
            if (Column != null)
            {
                Column.Hide();
                await Column.HiddenChanged.InvokeAsync(Column.Hidden);
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

        #endregion

        public void Dispose()
        {
            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent -= ClearSort;
                DataGrid.SelectedAllItemsChangedEvent -= OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent -= OnSelectedItemsChanged;
            }
        }
    }
}

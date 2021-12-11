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

        [Parameter] public string Title { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int ColSpan { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; } = ColumnType.Text;
        [Parameter] public Func<T, object> SortBy 
        { 
            get
            {
                CompileSortBy();
                return _sortBy;
            }
            set
            {
                _sortBy = value;
            }
        }
        [Parameter] public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;
        [Parameter] public SortDirection InitialDirection { get; set; } = SortDirection.None;
        [Parameter] public bool? Sortable { get; set; }
        [Parameter] public bool? Filterable { get; set; }
        [Parameter] public bool? ShowColumnOptions { get; set; }
        [Parameter] public string HeaderClass { get; set; }
        [Parameter] public string HeaderStyle { get; set; }

        private SortDirection _initialDirection;
        private Func<T, object> _sortBy;
        private Type _dataType;
        private bool _isSelected;
        private string _classname =>
            new CssBuilder(HeaderClass)
                .AddClass(Class)
            .Build();
        private string _style =>
            new StyleBuilder()
                .AddStyle(HeaderStyle)
                .AddStyle(Style)
            .Build();

        #region Computed Properties and Functions

        private string computedTitle
        {
            get
            {
                return Title ?? Field;
            }
        }
        private bool sortable
        {
            get
            {
                return Sortable ?? DataGrid?.Sortable ?? true;
            }
        }
        private bool filterable
        {
            get
            {
                return Filterable ?? DataGrid?.Filterable ?? true;
            }
        }
        private bool showColumnOptions
        {
            get
            {
                if (!sortable && !filterable)
                    return false;

                return ShowColumnOptions ?? DataGrid?.ShowColumnOptions ?? true;
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

                return DataGrid.FilterDefinitions.Any(x => x.Field == Field && x.Operator != null && x.Value != null);
            }
        }

        #endregion

        /// <summary>
        /// The main content for the HeaderCell. We use a RenderFragment here so the code 
        /// does not have to be repeated in the razor file.
        /// </summary>
        private RenderFragment _content
        {
            get
            {
                return (builder =>
                {
                    if (IsOnlyHeader)
                    {
                        builder.AddContent(0, ChildContent);
                    }
                    else if (HeaderTemplate != null)
                    {
                        builder.AddContent(0, HeaderTemplate);
                    }
                    else
                    {
                        builder.AddContent(0, computedTitle);
                    }
                });
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _initialDirection = InitialDirection;

            if (_initialDirection != SortDirection.None)
            {
                // set initial sort
                await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, SortBy, Field));
            }

            if (DataGrid != null)
            {
                DataGrid.SortChangedEvent += ClearSort;
                DataGrid.SelectedAllItemsChangedEvent += OnSelectedAllItemsChanged;
                DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
            }
        }

        internal void CompileSortBy()
        {
            if (_sortBy == null)
            {
                // set the default SortBy
                var parameter = Expression.Parameter(typeof(T), "x");
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(object));
                _sortBy = Expression.Lambda<Func<T, object>>(field, parameter).Compile();
            }
        }

        internal void GetDataType()
        {
            var p = typeof(T).GetProperty(Field);
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
            if (Field != field)
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

            await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, SortBy, Field));
        }

        internal async Task RemoveSortAsync()
        {
            _initialDirection = SortDirection.None;
            await InvokeAsync(() => DataGrid.SetSortAsync(SortDirection.None, SortBy, Field));
        }

        internal void AddFilter()
        {
            DataGrid.AddFilter(Guid.NewGuid(), Field);
        }

        internal void OpenFilters()
        {
            DataGrid.OpenFilters();
        }

        private async Task CheckedChangedAsync(bool value)
        {
            await DataGrid?.SetSelectAllAsync(value);
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

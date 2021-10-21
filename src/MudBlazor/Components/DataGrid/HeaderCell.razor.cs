// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using MudBlazor.Components.DataGrid;

namespace MudBlazor
{
    public partial class HeaderCell<T> : MudComponentBase
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }
        [CascadingParameter(Name = "IsOnlyHeader")] public bool IsOnlyHeader { get; set; } = false;

        [Parameter] public string Title { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public int ColSpan { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; } = ColumnType.Text;
        [Parameter] public Func<T, object> SortBy { get; set; } = null;
        [Parameter] public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;
        [Parameter] public SortDirection InitialDirection { get; set; } = SortDirection.None;
        [Parameter] public bool? Sortable { get; set; }
        [Parameter] public bool? Filterable { get; set; }
        [Parameter] public bool? ShowColumnOptions { get; set; }

        private SortDirection _initialDirection;
        private Type _dataType;

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
                    if (HeaderTemplate != null)
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
                CompileSortBy();
                // set initial sort
                await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, SortBy, Field));
            }

            if (DataGrid != null)
                DataGrid.OnSortChanged += ClearSort;
        }

        private void CompileSortBy()
        {
            if (SortBy == null)
            {
                // set the default SortBy
                var parameter = Expression.Parameter(typeof(T), "x");
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(object));
                SortBy = Expression.Lambda<Func<T, object>>(field, parameter).Compile();
            }
        }

        private void GetDataType()
        {
            var p = typeof(T).GetProperty(Field);
            _dataType = p.GetType();
        }

        #region Events

        private async Task SortChangedAsync()
        {
            CompileSortBy();

            if (_initialDirection == SortDirection.None)
                _initialDirection = SortDirection.Ascending;
            else if (_initialDirection == SortDirection.Ascending)
                _initialDirection = SortDirection.Descending;
            else if (_initialDirection == SortDirection.Descending)
                _initialDirection = SortDirection.None;

            await InvokeAsync(() => DataGrid.SetSortAsync(_initialDirection, SortBy, Field));
        }

        private async Task RemoveSortAsync()
        {
            CompileSortBy();
            await InvokeAsync(() => DataGrid.SetSortAsync(SortDirection.None, SortBy, Field));
        }

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

        private void AddFilter()
        {
            DataGrid.AddFilter(Guid.NewGuid(), Field);
        }

        private void CheckedChanged(bool value)
        {
            DataGrid.SetSelectAll(value);
        }

        #endregion
    }
}

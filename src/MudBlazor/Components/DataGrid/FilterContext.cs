// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MudBlazor
{
    /// <summary>
    /// Represents the current state of a filter in a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item managed by the <see cref="MudDataGrid{T}"/>.</typeparam>
    /// <seealso cref="MudDataGrid{T}"/>
    public class FilterContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        internal HeaderCell<T>? HeaderCell { get; set; }

        /// <summary>
        /// The definition of this filter.
        /// </summary>
        public IFilterDefinition<T>? FilterDefinition { get; set; }

        /// <summary>
        /// The items to filter.
        /// </summary>
        public IEnumerable<T>? Items => _dataGrid.Items;

        /// <summary>
        /// Indicates whether this filter currently has an applied value in the data grid.
        /// </summary>
        public bool IsFiltered => _dataGrid.HasFilter(FilterDefinition?.Column);

        /// <summary>
        /// The icon which reflects whether this filter is currently applied.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="MudDataGrid{T}.FilterIconFilled"/> when <see cref="IsFiltered"/> is <c>true</c>;
        /// otherwise returns <see cref="MudDataGrid{T}.FilterIconEmpty"/>.
        /// </remarks>
        public string FilterIcon => IsFiltered ? _dataGrid.FilterIconFilled : _dataGrid.FilterIconEmpty;

        /// <summary>
        /// The definitions of all filters in the grid.
        /// </summary>
        public List<IFilterDefinition<T>> FilterDefinitions => _dataGrid.FilterDefinitions;

        /// <summary>
        /// The behaviors which occur when filters are applied or cleared.
        /// </summary>
        public FilterActions Actions { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The <see cref="MudDataGrid{T}"/> managing this filter.</param>
        public FilterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = CreateHeaderCellActions();
        }

        internal void SetActions(FilterActions actions)
        {
            Actions = actions;
        }

        internal FilterActions CreateHeaderCellActions()
        {
            return new FilterActions
            {
                ApplyFilterAsync = async x => await (HeaderCell?.ApplyFilterAsync(x) ?? Task.CompletedTask),
                ApplyFiltersAsync = async x => await (HeaderCell?.ApplyFiltersAsync(x) ?? Task.CompletedTask),
                ClearFilterAsync = async x => await (HeaderCell?.ClearFilterAsync(x) ?? Task.CompletedTask),
                ClearFiltersAsync = async x => await (HeaderCell?.ClearFiltersAsync(x) ?? Task.CompletedTask),
                CloseFilterAsync = async () => await (HeaderCell?.CloseFilterAsync() ?? Task.CompletedTask),
            };
        }

        /// <summary>
        /// Represents the apply and clear behaviors for a filter of a<see cref="MudDataGrid{T}"/>.
        /// </summary>
        public class FilterActions
        {
            /// <summary>
            /// The function which applies a single filter.
            /// </summary>
            public required Func<IFilterDefinition<T>, Task> ApplyFilterAsync { get; init; }

            /// <summary>
            /// The function which applies multiple filters.
            /// </summary>
            public required Func<IEnumerable<IFilterDefinition<T>>, Task> ApplyFiltersAsync { get; init; }

            /// <summary>
            /// The function which clears a single filter.
            /// </summary>
            public required Func<IFilterDefinition<T>, Task> ClearFilterAsync { get; init; }

            /// <summary>
            /// The function which clears multiple filters.
            /// </summary>
            public required Func<IEnumerable<IFilterDefinition<T>>, Task> ClearFiltersAsync { get; init; }

            /// <summary>
            /// The function which closes the filter UI associated with this context.
            /// </summary>
            /// <remarks>
            /// In <see cref="DataGridFilterMode.Simple"/>, this closes the data grid filter panel.
            /// In <see cref="DataGridFilterMode.ColumnFilterMenu"/>, this closes the column filter popover.
            /// </remarks>
            public required Func<Task> CloseFilterAsync { get; init; }
        }
    }
}

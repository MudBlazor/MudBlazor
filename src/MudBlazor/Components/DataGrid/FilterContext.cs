// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    public class FilterContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        internal FilterDefinition<T>? FilterDefinition { get; set; }

        internal HeaderCell<T>? HeaderCell { get; set; }

        public IEnumerable<T> Items
        {
            get
            {
                return _dataGrid.Items;
            }
        }

        public List<FilterDefinition<T>> FilterDefinitions
        {
            get
            {
                return _dataGrid.FilterDefinitions;
            }
        }

        public FilterActions Actions { get; }

        public FilterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FilterActions
            {
                ApplyFilterAsync = (HeaderCell is not null) ? async x => await HeaderCell.ApplyFilterAsync(x) : async _ => await Task.CompletedTask,
                ApplyFiltersAsync = (HeaderCell is not null) ? async x => await HeaderCell.ApplyFiltersAsync(x) : async _ => await Task.CompletedTask,
                ClearFilterAsync = (HeaderCell is not null) ? async x => await HeaderCell.ClearFilterAsync(x) : async _ => await Task.CompletedTask,
                ClearFiltersAsync = (HeaderCell is not null) ? async x => await HeaderCell.ClearFiltersAsync(x) : async _ => await Task.CompletedTask,
            };
        }

        public class FilterActions
        {
            public Func<FilterDefinition<T>, Task> ApplyFilterAsync { get; init; } = null!;
            public Func<IEnumerable<FilterDefinition<T>>, Task> ApplyFiltersAsync { get; init; } = null!;
            public Func<FilterDefinition<T>, Task> ClearFilterAsync { get; init; } = null!;
            public Func<IEnumerable<FilterDefinition<T>>, Task> ClearFiltersAsync { get; init; } = null!;
        }
    }
}

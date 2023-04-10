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
                ApplyFilterAsync = x =>
                {
                    //Use Task.CompletedTask but later ApplyFilter should return Task
                    HeaderCell?.ApplyFilter(x);
                    return Task.CompletedTask;
                },
                ApplyFiltersAsync = x =>
                {
                    HeaderCell?.ApplyFilters(x);
                    return Task.CompletedTask;
                },
                ClearFilterAsync = x =>
                {
                    HeaderCell?.ClearFilter(x);
                    return Task.CompletedTask;
                },
                ClearFiltersAsync = x =>
                {
                    HeaderCell?.ClearFilters(x);
                    return Task.CompletedTask;
                },
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

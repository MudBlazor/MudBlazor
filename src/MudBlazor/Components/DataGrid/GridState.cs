// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MudBlazor
{
#nullable enable
    public class GridState<T>
    {
        public GridState()
        {
        }

        public GridState(GridState<T> gridState)
        {
            Page = gridState.Page;
            PageSize = gridState.PageSize;
            SortDefinitions = gridState.SortDefinitions;
            FilterDefinitions = gridState.FilterDefinitions;
        }
        public int Page { get; set; }

        public int PageSize { get; set; }

        public ICollection<SortDefinition<T>> SortDefinitions { get; set; } = new List<SortDefinition<T>>();

        public ICollection<IFilterDefinition<T>> FilterDefinitions { get; set; } = new List<IFilterDefinition<T>>();
    }


    public class GridStateVirtualize<T> : GridState<T>
    {
        public GridStateVirtualize(GridState<T> gridState) : base(gridState)
        {

        }
        /// <summary>
        /// The zero-based index of the first item to be supplied.
        /// </summary>
        public int StartIndex { get; init; }

        /// <summary>
        /// If set, the maximum number of items to be supplied. If not set, the maximum number is unlimited.
        /// </summary>
        public int? Count { get; init; }

        public CancellationToken CancellationToken { get; init; }
    }

    public class GridData<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public int TotalItems { get; set; }
    }
}

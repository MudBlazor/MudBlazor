// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the current paging, sorting, and filtering for a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The kind of item managed by the grid.</typeparam>
    public class GridState<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        /// <summary>
        /// The current page being displayed. The page index is zero-based.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The maximum number of items displayed on each page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The current sorting applied to grid values.
        /// </summary>
        public ICollection<SortDefinition<T>> SortDefinitions { get; set; } = new List<SortDefinition<T>>();

        /// <summary>
        /// The current filters applied to grid values.
        /// </summary>
        public ICollection<IFilterDefinition<T>> FilterDefinitions { get; set; } = new List<IFilterDefinition<T>>();
    }

    public class GridStateVirtualize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        /// <summary>
        /// The zero-based index of the first item to be supplied.
        /// </summary>
        public int StartIndex { get; init; }

        /// <summary>
        /// The requested number of items to be provided. The actual number of provided items does not need to match
        /// this value.
        /// </summary>
        public int Count { get; init; }

        /// <summary>
        /// The current sorting applied to grid values.
        /// </summary>
        public IReadOnlyCollection<SortDefinition<T>> SortDefinitions { get; set; } = new List<SortDefinition<T>>();

        /// <summary>
        /// The current filters applied to grid values.
        /// </summary>
        public IReadOnlyCollection<IFilterDefinition<T>> FilterDefinitions { get; set; } = new List<IFilterDefinition<T>>();
    }

    /// <summary>
    /// Represents data to display in a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The kind of item managed by the grid.</typeparam>
    public class GridData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        /// <summary>
        /// The items to display in the grid.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The total number of items, excluding page size.
        /// </summary>
        /// <remarks>
        /// This property is used to determine the number of pages of data.
        /// </remarks>
        public int TotalItems { get; set; }
    }
}

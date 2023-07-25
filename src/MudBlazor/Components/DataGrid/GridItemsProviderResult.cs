// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.Components.DataGrid
{
    /// <summary>
    /// Holds data being supplied to a MudBlazor DataGrid/>.
    /// </summary>
    /// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
    public readonly struct GridItemsProviderResult<TGridItem>
    {
        /// <summary>
        /// The items being supplied.
        /// </summary>
        public ICollection<TGridItem> Items { get; init; }

        /// <summary>
        /// The total number of items that may be displayed in the grid. This normally means the total number of items in the
        /// underlying data source after applying any filtering that is in effect.
        ///
        /// If the grid is paginated, this should include all pages. If the grid is virtualized, this should include the entire scroll range.
        /// </summary>
        public int TotalItemCount { get; init; }
    }

}

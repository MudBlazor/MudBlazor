using System.Collections.Generic;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// The state of a <see cref="MudTable{T}"/> when requesting data via <see cref="MudTable{T}.ServerData"/>.
    /// </summary>
    public class TableState
    {
        /// <summary>
        /// The requested index of the page to display.
        /// </summary>
        /// <remarks>
        /// The index of the first page is <c>0</c>.  
        /// </remarks>
        public int Page { get; set; }

        /// <summary>
        /// The number of items requested.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The optional column to sort by.
        /// </summary>
        public string? SortLabel { get; set; }

        /// <summary>
        /// The direction to sort results.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.
        /// </remarks>
        public SortDirection SortDirection { get; set; }
    }

    /// <summary>
    /// The result of a call to <see cref="MudTable{T}.ServerData"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to display in the table.</typeparam>
    public class TableData<T>
    {
        /// <summary>
        /// The items to display in the table.
        /// </summary>
        /// <remarks>
        /// The number of items should match the number in <see cref="TableState.PageSize"/>.  
        /// </remarks>
        public IEnumerable<T>? Items { get; set; }

        /// <summary>
        /// The total number of items, excluding pagination.
        /// </summary>
        /// <remarks>
        /// This number is used to calculate the total number of pages in the table.
        /// </remarks>
        public int TotalItems { get; set; }
    }
}

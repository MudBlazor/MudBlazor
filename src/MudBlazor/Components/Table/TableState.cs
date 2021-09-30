using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public class TableState
    {
        [Obsolete("Please use Start for the exact number of items to skip, could be in the middle of a page")]
        public int Page { get; set; }

        [Obsolete("Please use Count for the exact number of items to retreive, could be only the items that fit on a page")]
        public int PageSize { get; set; }

        public int Start { get; set; }

        public int Count { get; set; }

        public string SortLabel { get; set; }

        public SortDirection SortDirection { get; set; }
    }

    public class TableData<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}

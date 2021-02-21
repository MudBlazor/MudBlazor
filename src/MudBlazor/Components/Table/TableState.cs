﻿using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public class TableState
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string SortLabel { get; set; }

        public SortDirection SortDirection { get; set; }
    }

    public class TableState<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
        public string SortLabel { get; set; }

        public Func<T, object> SortBy { get; set; } = null;

        public SortDirection SortDirection { get; set; }
    }

    public class TableData<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}

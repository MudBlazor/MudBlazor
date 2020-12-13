using System;

namespace MudBlazor
{
    public class MudTablePageEventArgs : EventArgs
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string SortLabel { get; set; }

        public SortDirection SortDirection { get; set; }
    }

}

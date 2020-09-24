using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MudBlazor
{
    public partial class MudTablePager : MudComponentBase
    {
        [CascadingParameter] public MudTableBase Table { get; set; }

        [Parameter] public bool DisableRowsPerPage { get; set; }

        [Parameter] public List<int> PageSizeOptions { get; set; } = new List<int>() { 10, 25, 50, 100 };

        [Parameter] public string InfoFormat { get; set; } = "{first_item}-{last_item} of {all_items}";

        private string Info => InfoFormat
            .Replace("{first_item}", $"{Table.CurrentPage * Table.RowsPerPage+1}")
            .Replace("{last_item}", $"{Math.Min((Table.CurrentPage+1) * Table.RowsPerPage, Table.GetFilteredItemsCount())}")
            .Replace("{all_items}", $"{Table.GetFilteredItemsCount()}");

        [Parameter] public string RowsPerPageString { get; set; } = "Rows per page:";

        private void SetRowsPerPage(string size)
        {
            Table.SetRowsPerPage(int.Parse(size));
        }
    }
}

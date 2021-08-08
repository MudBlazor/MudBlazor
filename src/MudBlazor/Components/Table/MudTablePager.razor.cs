﻿using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTablePager : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-table-pagination-toolbar")
            .AddClass(Class)
            .Build();

        [CascadingParameter] public TableContext Context { get; set; }

        /// <summary>
        /// Set true to hide the part of the pager which allows to change the page size.
        /// </summary>
        [Parameter] public bool HideRowsPerPage { get; set; }

        /// <summary>
        /// Set true to hide the part of the pager which allows to change the page size.
        /// </summary>
        [Obsolete("DisableRowsPerPage is obsolete. Use HideRowsPerPage instead!", false)]
        [Parameter] public bool DisableRowsPerPage { get; set; }

        /// <summary>
        /// Set true to hide the number of pages.
        /// </summary>
        [Parameter] public bool HidePageNumber { get; set; }

        /// <summary>
        /// Set true to hide the pagination.
        /// </summary>
        [Parameter] public bool HidePagination { get; set; }

        /// <summary>
        /// Set the horizontal alignment position.
        /// </summary>
        [Parameter] public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Right;

        /// <summary>
        /// Define a list of available page size options for the user to choose from
        /// </summary>
        [Parameter] public int[] PageSizeOptions { get; set; } = new int[] { 10, 25, 50, 100 };

        /// <summary>
        /// Format string for the display of the current page, which you can localize to your language. Available variables are:
        /// {first_item}, {last_item} and {all_items} which will replaced with the indices of the page's first and last item, as well as the total number of items.
        /// Default: "{first_item}-{last_item} of {all_items}" which is transformed into "0-25 of 77". 
        /// </summary>
        [Parameter] public string InfoFormat { get; set; } = "{first_item}-{last_item} of {all_items}";

        private string Info => Table == null ? "Table==null" : InfoFormat
            .Replace("{first_item}", $"{Table?.CurrentPage * Table.RowsPerPage + 1}")
            .Replace("{last_item}", $"{Math.Min((Table.CurrentPage + 1) * Table.RowsPerPage, Table.GetFilteredItemsCount())}")
            .Replace("{all_items}", $"{Table.GetFilteredItemsCount()}");

        /// <summary>
        /// The localizable "Rows per page:" text.
        /// </summary>
        [Parameter] public string RowsPerPageString { get; set; } = "Rows per page:";

        private void SetRowsPerPage(string size)
        {
            Table?.SetRowsPerPage(int.Parse(size));
        }

        private bool BackButtonsDisabled => Table == null ? false : Table.CurrentPage == 0;

        private bool ForwardButtonsDisabled => Table == null ? false : (Table.CurrentPage + 1) * Table.RowsPerPage >= Table.GetFilteredItemsCount();

        public MudTableBase Table => Context?.Table;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Context != null)
            {
                Context.HasPager = true;
                Context.PagerStateHasChanged = StateHasChanged;
            }
        }

    }
}

using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTablePager : MudComponentBase
    {
        protected string Classname =>
                    new CssBuilder("mud-table-pagination-toolbar")
                    .AddClass("mud-tablepager-left", !RightToLeft)
                    .AddClass("mud-tablepager-right", RightToLeft)
                    .AddClass(Class)
                    .Build();

        protected string PaginationClassname =>
            new CssBuilder("mud-table-pagination-display")
            .AddClass("mud-tablepager-left", !RightToLeft)
            .AddClass("mud-tablepager-right", RightToLeft)
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        [CascadingParameter] public TableContext Context { get; set; }

        /// <summary>
        /// Set true to hide the part of the pager which allows to change the page size.
        /// </summary>
        [Parameter] public bool HideRowsPerPage { get; set; }

        /// <summary>
        /// Set true to hide the part of the pager which allows to change the page size.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use HideRowsPerPage instead.", true)]
        [Parameter] public bool DisableRowsPerPage { get => HideRowsPerPage; set => HideRowsPerPage = value; }

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

        /// <summary>
        /// Defines the text shown in the items per page dropdown when a user provides int.MaxValue as an option
        /// </summary>
        [Parameter] public string AllItemsText { get; set; } = "All";

        private string Info
        {
            get
            {
                // fetch number of filtered items (once only)
                var filteredItemsCount = Table?.GetFilteredItemsCount() ?? 0;

                return Table == null
                    ? "Table==null"
                    : InfoFormat
                        .Replace("{first_item}", $"{(filteredItemsCount == 0 ? 0 : Table?.CurrentPage * Table.RowsPerPage + 1)}")
                        .Replace("{last_item}", $"{Math.Min((Table.CurrentPage + 1) * Table.RowsPerPage, filteredItemsCount)}")
                        .Replace("{all_items}", $"{filteredItemsCount}");
            }
        }

        /// <summary>
        /// The localizable "Rows per page:" text.
        /// </summary>
        [Parameter] public string RowsPerPageString { get; set; } = "Rows per page:";

        /// <summary>
        /// Custom first icon.
        /// </summary>
        [Parameter] public string FirstIcon { get; set; } = Icons.Material.Filled.FirstPage;

        /// <summary>
        /// Custom before icon.
        /// </summary>
        [Parameter] public string BeforeIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// Custom next icon.
        /// </summary>
        [Parameter] public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// Custom last icon.
        /// </summary>
        [Parameter] public string LastIcon { get; set; } = Icons.Material.Filled.LastPage;

        private void SetRowsPerPage(int size) => Table?.SetRowsPerPage(size);

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
                var size = Table._rowsPerPage ?? PageSizeOptions.FirstOrDefault();
                SetRowsPerPage(size);
            }
        }

    }
}

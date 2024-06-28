using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A component which changes pages and page size for a <see cref="MudTable{T}"/>.
    /// </summary>
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

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this pager.
        /// </summary>
        [CascadingParameter]
        public TableContext? Context { get; set; }

        /// <summary>
        /// Hides the list of page sizes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HideRowsPerPage { get; set; }

        /// <summary>
        /// Hides the current page number.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HidePageNumber { get; set; }

        /// <summary>
        /// Hides the list of page numbers.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HidePagination { get; set; }

        /// <summary>
        /// The location of this pager relative to the parent <see cref="MudTable{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="HorizontalAlignment.Right"/>.
        /// </remarks>
        [Parameter]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Right;

        /// <summary>
        /// The list of page sizes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>, <c>25</c>, <c>50</c>, and <c>100</c>.  Requires <see cref="HideRowsPerPage"/> to be <c>false</c>.
        /// </remarks>
        [Parameter]
        public int[] PageSizeOptions { get; set; } = new[] { 10, 25, 50, 100 };

        /// <summary>
        /// The format of the text label.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"{first_item}-{last_item} of {all_items}"</c> (e.g. <c>0-25 of 77</c>).  You can use any of the following values:
        /// <list type="bullet"> 
        /// <item><description><c>{first_item}</c>: The index of the first row being displayed.</description></item> 
        /// <item><description><c>{last_item}</c>: The index of the last row being displayed.</description></item> 
        /// <item><description><c>{all_items}</c>: The total number of rows in all pages.</description></item> 
        /// </list> 
        /// </remarks>
        [Parameter]
        public string InfoFormat { get; set; } = "{first_item}-{last_item} of {all_items}";

        /// <summary>
        /// The text displayed in the page-size dropdown when <see cref="PageSizeOptions"/> contains <see cref="int.MaxValue"/>.
        /// </summary>
        [Parameter]
        public string AllItemsText { get; set; } = "All";

        private string Info
        {
            get
            {
                // fetch number of filtered items (once only)
                var filteredItemsCount = Table?.GetFilteredItemsCount() ?? 0;

                return Table is null
                    ? "Table==null"
                    : InfoFormat
                        .Replace("{first_item}", $"{(filteredItemsCount == 0 ? 0 : (Table?.CurrentPage * Table?.RowsPerPage) + 1)}")
                        .Replace("{last_item}", $"{Math.Min((Table?.CurrentPage + 1) * Table?.RowsPerPage ?? 0, filteredItemsCount)}")
                        .Replace("{all_items}", $"{filteredItemsCount}");
            }
        }

        /// <summary>
        /// The text label for the current rows per page.
        /// </summary>
        [Parameter]
        public string RowsPerPageString { get; set; } = "Rows per page:";

        /// <summary>
        /// The icon for the "First Page" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.FirstPage"/>.
        /// </remarks>
        [Parameter]
        public string FirstIcon { get; set; } = Icons.Material.Filled.FirstPage;

        /// <summary>
        /// The icon for the "Previous Page" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateBefore"/>.
        /// </remarks>
        [Parameter]
        public string BeforeIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// The icon for the "Next Page" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateNext"/>.
        /// </remarks>
        [Parameter]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// The icon for the "Last Page" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.LastPage"/>.
        /// </remarks>
        [Parameter]
        public string LastIcon { get; set; } = Icons.Material.Filled.LastPage;

        private void SetRowsPerPage(int size) => Table?.SetRowsPerPage(size);

        private bool BackButtonsDisabled => Table is { CurrentPage: 0 };

        private bool ForwardButtonsDisabled => Table != null && (Table.CurrentPage + 1) * Table.RowsPerPage >= Table.GetFilteredItemsCount();

        /// <summary>
        /// The <see cref="MudTable{T}"/> linked to this pager.
        /// </summary>
        public MudTableBase? Table => Context?.Table;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Context != null)
            {
                Context.HasPager = true;
                Context.PagerStateHasChanged = StateHasChanged;
                var size = Table?._rowsPerPage ?? PageSizeOptions.FirstOrDefault();
                SetRowsPerPage(size);
            }
        }
    }
}

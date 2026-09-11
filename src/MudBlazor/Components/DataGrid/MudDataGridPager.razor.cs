// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Resources;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Pagination controls for navigating pages of a <see cref="MudDataGrid{T}"/>, with page-size selection and next and previous buttons.
    /// </summary>
    /// <typeparam name="T">The kind of data displayed in the grid.</typeparam>
    /// <seealso cref="MudDataGrid{T}" />
    /// <seealso cref="MudTablePager" />
    public partial class MudDataGridPager<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase, IDisposable
    {
        /// <summary>
        /// The grid which contains this pager.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T>? DataGrid { get; set; }

        /// <summary>
        /// Shows the page-size drop-down list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Use <see cref="PageSizeOptions"/> to control the allowed page sizes.
        /// </remarks>
        [Parameter]
        public bool PageSizeSelector { get; set; } = true;

        /// <summary>
        /// Disables the back button, forward button, and page-size drop-down list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// The allowed page sizes when <see cref="PageSizeSelector"/> is <c>true</c>.  Defaults to <c>10</c>, <c>25</c>, <c>50</c>, <c>100</c>.
        /// </summary>
        [Parameter]
        public int[] PageSizeOptions { get; set; } = new int[] { 10, 25, 50, 100 };

        /// <summary>
        /// The format for the first item, last item, and number of total items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>{first_item}-{last_item} of {all_items}</c> (e.g. <c>0-25 of 77</c>).  Available values are <c>{first_item}</c>, <c>{last_item}</c>, and <c>{all_items}</c>.
        /// </remarks>
        [Parameter]
        public string InfoFormat { get; set; } = string.Empty;

        /// <summary>
        /// The text to show for the "Rows per page:" label.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>Rows per page:</c>.  Can be localized to other languages.
        /// </remarks>
        [Parameter]
        public string RowsPerPageString { get; set; } = string.Empty;

        /// <summary>
        /// Shows the pagination buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool ShowNavigation { get; set; } = true;

        /// <summary>
        /// Shows the current page number.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool ShowPageNumber { get; set; } = true;

        /// <summary>
        /// Defines the text shown in the items per page dropdown when a user provides int.MaxValue as an option
        /// </summary>
        [Parameter]
        public string AllItemsText { get; set; } = string.Empty;

        /// <summary>
        /// Shows clickable page numbers between the navigation buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, a user can jump directly to a page by clicking its number.
        /// </remarks>
        [Parameter]
        public bool ShowPageNumbers { get; set; }

        /// <summary>
        /// The display variant of the pagination buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        public Variant PaginationVariant { get; set; } = Variant.Text;

        /// <summary>
        /// The size of the pagination buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        public Size PaginationSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the selected page button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        [Parameter]
        public Color PaginationColor { get; set; } = Color.Primary;

        private string Info
        {
            get
            {
                if (DataGrid == null)
                    return "DataGrid==null";
                Debug.Assert(DataGrid is not null);
                var firstItem = DataGrid.GetFilteredItemsCount() == 0 ? 0 : (DataGrid.CurrentPage * DataGrid.RowsPerPage) + 1;
                var lastItem = Math.Min((DataGrid.CurrentPage + 1) * DataGrid.RowsPerPage, DataGrid.GetFilteredItemsCount());
                var allItems = DataGrid.GetFilteredItemsCount();
                var culture = DataGrid.Culture ?? CultureInfo.InvariantCulture;

                if (string.IsNullOrEmpty(InfoFormat))
                {
                    return Localizer[LanguageResource.MudDataGridPager_InfoFormat, firstItem.ToString("N0", culture), lastItem.ToString("N0", culture), allItems.ToString("N0", culture)];
                }

                return InfoFormat
                    .Replace("{first_item}", firstItem.ToString("N0", culture))
                    .Replace("{last_item}", lastItem.ToString("N0", culture))
                    .Replace("{all_items}", allItems.ToString("N0", culture));
            }
        }

        // MudPagination is one-based, while the grid's CurrentPage is zero-based.
        private int CurrentPageNumber => (DataGrid?.CurrentPage ?? 0) + 1;

        private int PageCount => DataGrid is null
            ? 1
            : Math.Max(1, (int)Math.Ceiling(DataGrid.GetFilteredItemsCount() / (double)DataGrid.RowsPerPage));

        protected string Classname =>
            new CssBuilder("mud-table-pagination-toolbar")
                .AddClass(Class)
                .Build();

        private async Task SetRowsPerPageAsync(int size)
        {
            if (DataGrid != null)
            {
                await DataGrid.SetRowsPerPageAsync(size);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (string.IsNullOrEmpty(RowsPerPageString))
            {
                RowsPerPageString = Localizer[LanguageResource.MudDataGridPager_RowsPerPage];
            }

            if (string.IsNullOrEmpty(AllItemsText))
            {
                AllItemsText = Localizer[LanguageResource.MudDataGridPager_AllItems];
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (DataGrid != null)
            {
                DataGrid.HasPager = true;
                DataGrid.PagerStateHasChangedEvent += StateHasChanged;
                var size = DataGrid._rowsPerPage ?? PageSizeOptions.FirstOrDefault();
                await DataGrid.SetRowsPerPageAsync(size, false);
            }
        }

        /// <summary>
        /// Releases resources used by this pager.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by this pager.
        /// </summary>
        /// <param name="disposing">When <c>true</c>, managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (DataGrid != null)
            {
                DataGrid.PagerStateHasChangedEvent -= StateHasChanged;
            }
        }
    }
}

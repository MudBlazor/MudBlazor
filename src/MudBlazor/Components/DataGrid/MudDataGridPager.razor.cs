// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Resources;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a pager for navigating pages of a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The kind of data displayed in the grid.</typeparam>
    public partial class MudDataGridPager<T> : MudComponentBase, IDisposable
    {
        /// <summary>
        /// The grid which contains this pager.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T> DataGrid { get; set; }

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

        private string Info
        {
            get
            {
                if (DataGrid == null)
                    return "DataGrid==null";
                Debug.Assert(DataGrid != null);
                var firstItem = DataGrid?.GetFilteredItemsCount() == 0 ? 0 : DataGrid.CurrentPage * DataGrid.RowsPerPage + 1;
                var lastItem = Math.Min((DataGrid.CurrentPage + 1) * DataGrid.RowsPerPage, DataGrid.GetFilteredItemsCount());
                var allItems = DataGrid?.GetFilteredItemsCount() ?? 0;

                if (InfoFormat.Contains("{first_item}") || InfoFormat.Contains("{last_item}") || InfoFormat.Contains("{all_items}"))
                {
                    return InfoFormat
                        .Replace("{first_item}", $"{firstItem}")
                        .Replace("{last_item}", $"{lastItem}")
                        .Replace("{all_items}", $"{allItems}");
                }

                return Localizer[LanguageResource.MudDataGridPager_InfoFormat, firstItem, lastItem, allItems];
            }
        }

        private bool BackButtonsDisabled => Disabled || DataGrid is { CurrentPage: 0 };

        private bool ForwardButtonsDisabled => Disabled || (DataGrid != null && (DataGrid.CurrentPage + 1) * DataGrid.RowsPerPage >= DataGrid.GetFilteredItemsCount());

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
            if (DataGrid != null)
            {
                DataGrid.PagerStateHasChangedEvent -= StateHasChanged;
            }
        }
    }
}

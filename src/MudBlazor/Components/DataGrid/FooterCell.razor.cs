// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// The footer cell displayed at the bottom of a <see cref="MudDataGrid{T}"/> column, typically showing totals or aggregate values.
    /// </summary>
    /// <typeparam name="T">The kind of data managed by this footer.</typeparam>
    public partial class FooterCell<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
    {
        /// <summary>
        /// The <see cref="MudDataGrid{T}"/> which contains this footer cell.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T>? DataGrid { get; set; }

        /// <summary>
        /// The column related to this footer cell.
        /// </summary>
        [Parameter]
        public Column<T>? Column { get; set; }

        /// <summary>
        /// The content within this footer cell.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The current values related to this footer cell.
        /// </summary>
        [Parameter]
        public IEnumerable<T>? CurrentItems { get; set; }

        /// <summary>
        /// The rows of the group this footer cell belongs to.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>, which scopes the footer to the whole grid.  It is set for the footer rendered underneath a single group of rows.
        /// </remarks>
        [Parameter]
        public IEnumerable<T>? GroupItems { get; set; }

        // Set in OnInitialized() so it can't be null.
        private FooterContext<T> _footerContext = null!;

        protected override void OnInitialized()
        {
            Debug.Assert(DataGrid is not null);
            _footerContext = new FooterContext<T>(DataGrid) { GroupItemsFunc = () => GroupItems };
        }

        private string Classname =>
            new CssBuilder(Column?.FooterClassname)
                .AddClass(Column?.FooterClassFunc?.Invoke(items ?? Enumerable.Empty<T>()))
                .AddClass(Column?.FooterClass)
                .AddClass(Class)
                .Build();

        private string Stylename =>
            new StyleBuilder()
                .AddStyle(Column?.FooterStyleFunc?.Invoke(items ?? Enumerable.Empty<T>()))
                .AddStyle(Column?.FooterStyle)
                .AddStyle(Style)
                .AddStyle("font-weight", "600")
                .Build();

        internal IEnumerable<T> items
        {
            get
            {
                Debug.Assert(DataGrid is not null);
                return CurrentItems ?? DataGrid.CurrentPageItems;
            }
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a cell displayed at the bottom of a column.
    /// </summary>
    /// <typeparam name="T">The kind of data managed by this footer.</typeparam>
    public partial class FooterCell<T> : MudComponentBase
    {
        /// <summary>
        /// The <see cref="MudDataGrid{T}"/> which contains this footer cell.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T> DataGrid { get; set; }

        /// <summary>
        /// The column related to this footer cell.
        /// </summary>
        [Parameter]
        public Column<T> Column { get; set; }

        /// <summary>
        /// The content within this footer cell.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The current values related to this footer cell.
        /// </summary>
        [Parameter]
        public IEnumerable<T> CurrentItems { get; set; }

        private string _classname =>
            new CssBuilder("footer-cell")
                .AddClass(Column?.FooterClassFunc?.Invoke(items ?? Enumerable.Empty<T>()))
                .AddClass(Column?.FooterClass)
                .AddClass(Column?.footerClassname)
                .AddClass(Class)
            .Build();
        private string _style =>
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
                return CurrentItems ?? DataGrid?.CurrentPageItems;
            }
        }
    }
}

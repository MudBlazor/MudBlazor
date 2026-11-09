// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    /// <summary>
    /// Renders the data rows of a <see cref="MudDataGrid{T}"/>, using virtualization to keep only the visible rows in the DOM for large data sets.
    /// </summary>
    /// <typeparam name="T">The type of item displayed in the data grid.</typeparam>
    public partial class DataGridVirtualizeRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
    {
        /// <summary>
        /// The data grid which contains this virtualized row.
        /// </summary>
        [Parameter, EditorRequired]
        [Category(CategoryTypes.DataGrid.Data)]
        public MudDataGrid<T> DataGrid { get; set; } = null!;

        /// <summary>
        /// The grouped items rendered by this virtualized row.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DataGrid.Grouping)]
        public IGrouping<object, T>? GroupedItems { get; set; }

        /// <summary>
        /// Occurs when a row is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DataGrid.Selecting)]
        public EventCallback<(MouseEventArgs args, T item, int index)> RowClick { get; set; }

        /// <summary>
        /// Occurs when a row is right-clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DataGrid.Selecting)]
        public EventCallback<(MouseEventArgs args, T item, int index)> ContextRowClick { get; set; }
    }
}

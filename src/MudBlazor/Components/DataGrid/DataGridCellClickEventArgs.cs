// Copyright (c) MudBlazor 2026
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

/// <summary>
/// Represents the information related to a <see cref="MudDataGrid{T}.CellClick"/> event.
/// </summary>
/// <typeparam name="T">The type of data represented by each row in the <see cref="MudDataGrid{T}"/>.</typeparam>
public class DataGridCellClickEventArgs<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : EventArgs
{
    /// <summary>
    /// The coordinates of the pointer for this click.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; }

    /// <summary>
    /// The item whose row was clicked.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// The zero-based index of the row that was clicked.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// The zero-based index of the visible column that was clicked.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    /// The column that was clicked.
    /// </summary>
    public Column<T> Column { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="mouseEventArgs">The coordinates of the pointer for this click.</param>
    /// <param name="item">The item whose row was clicked.</param>
    /// <param name="rowIndex">The zero-based index of the row.</param>
    /// <param name="columnIndex">The zero-based index of the visible column.</param>
    /// <param name="column">The column that was clicked.</param>
    public DataGridCellClickEventArgs(MouseEventArgs mouseEventArgs, T item, int rowIndex, int columnIndex, Column<T> column)
    {
        MouseEventArgs = mouseEventArgs;
        Item = item;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Column = column;
    }
}

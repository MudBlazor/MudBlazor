// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents the information related to a <see cref="MudDataGrid{T}.RowClick"/> event.
/// </summary>
/// <typeparam name="T">The item managed by the <see cref="MudDataGrid{T}"/>.</typeparam>
public class DataGridRowClickEventArgs<T> : EventArgs
{
    /// <summary>
    /// The coordinates of the pointer for this click.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; }

    /// <summary>
    /// The item which was clicked.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// The index of the row.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="mouseEventArgs">The coordinates of the pointer for this click.</param>
    /// <param name="item">The item which was clicked.</param>
    /// <param name="rowIndex">The index of the row.</param>
    public DataGridRowClickEventArgs(MouseEventArgs mouseEventArgs, T item, int rowIndex)
    {
        MouseEventArgs = mouseEventArgs;
        Item = item;
        RowIndex = rowIndex;
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;

/// <summary>
/// Represents the information related to a <see cref="MudDataGrid{T}.ColumnOrderChanged"/> event.
/// </summary>
/// <typeparam name="T">The item managed by the <see cref="MudDataGrid{T}"/>.</typeparam>
public sealed class DataGridColumnOrderChangedEventArgs<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
{
    /// <summary>
    /// The column whose order change triggered the event.
    /// </summary>
    public Column<T> Column { get; }

    /// <summary>
    /// The previous rendered index of <see cref="Column"/>.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// The current rendered index of <see cref="Column"/>.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// The current rendered column order as a read-only snapshot.
    /// </summary>
    public IReadOnlyList<Column<T>> Columns { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridColumnOrderChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="column">The column whose order changed.</param>
    /// <param name="previousIndex">The previous rendered index of the column.</param>
    /// <param name="currentIndex">The current rendered index of the column.</param>
    /// <param name="columns">The current rendered column order.</param>
    public DataGridColumnOrderChangedEventArgs(Column<T> column, int previousIndex, int currentIndex, IReadOnlyList<Column<T>> columns)
    {
        Column = column;
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        Columns = columns;
    }
}

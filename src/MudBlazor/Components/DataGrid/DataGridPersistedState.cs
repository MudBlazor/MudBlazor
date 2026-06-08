// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Represents a serializable snapshot of a <see cref="MudDataGrid{T}"/> state.
/// </summary>
/// <remarks>
/// This type is intended for persistence scenarios such as browser storage, URLs, or databases.
/// Restore selection by providing item key selectors and resolvers to <see cref="MudDataGrid{T}.GetState(Func{T, string}?)"/>
/// and <see cref="MudDataGrid{T}.SetStateAsync(DataGridPersistedState, Func{string, T}?)"/>.
/// </remarks>
public sealed class DataGridPersistedState
{
    /// <summary>
    /// The zero-based page index.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The number of rows displayed on each page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// The active sort definitions.
    /// </summary>
    public List<DataGridSortState> Sorts { get; set; } = [];

    /// <summary>
    /// The active filter definitions.
    /// </summary>
    public List<DataGridFilterState> Filters { get; set; } = [];

    /// <summary>
    /// The active grouping definitions.
    /// </summary>
    public List<DataGridGroupState> Groups { get; set; } = [];

    /// <summary>
    /// The selected item key when <see cref="MudDataGrid{T}.MultiSelection"/> is <c>false</c>.
    /// </summary>
    public string? SelectedItemKey { get; set; }

    /// <summary>
    /// The selected item keys when <see cref="MudDataGrid{T}.MultiSelection"/> is <c>true</c>.
    /// </summary>
    public List<string> SelectedItemKeys { get; set; } = [];
}

/// <summary>
/// Represents a serializable sort definition for a <see cref="MudDataGrid{T}"/>.
/// </summary>
public sealed class DataGridSortState
{
    /// <summary>
    /// The column identifier used to resolve the sort column during restore.
    /// </summary>
    public string Column { get; set; } = string.Empty;

    /// <summary>
    /// When <c>true</c>, sorts in descending order.
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// The order of this sort relative to other sort definitions.
    /// </summary>
    public int Index { get; set; }
}

/// <summary>
/// Represents a serializable filter definition for a <see cref="MudDataGrid{T}"/>.
/// </summary>
public sealed class DataGridFilterState
{
    /// <summary>
    /// The column identifier used to resolve the filter column during restore.
    /// </summary>
    public string Column { get; set; } = string.Empty;

    /// <summary>
    /// The display title of the filter.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The filter operator.
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// The JSON-serialized filter value.
    /// </summary>
    public string? ValueJson { get; set; }

    /// <summary>
    /// The assembly-qualified type name of <see cref="ValueJson"/>.
    /// </summary>
    public string? ValueType { get; set; }
}

/// <summary>
/// Represents a serializable grouping definition for a <see cref="MudDataGrid{T}"/>.
/// </summary>
public sealed class DataGridGroupState
{
    /// <summary>
    /// The column identifier used to resolve the grouped column during restore.
    /// </summary>
    public string Column { get; set; } = string.Empty;

    /// <summary>
    /// The grouping order relative to other grouped columns.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Whether groups created from this column are expanded.
    /// </summary>
    public bool Expanded { get; set; }
}

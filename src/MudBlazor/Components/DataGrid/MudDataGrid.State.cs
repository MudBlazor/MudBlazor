// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MudBlazor;

public partial class MudDataGrid<T>
{
    private static readonly JsonSerializerOptions s_filterValueJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Captures the current grid state as a serializable snapshot.
    /// </summary>
    /// <param name="itemKeySelector">
    /// When provided, maps selected items to stable keys for persistence. Required to capture selection state.
    /// </param>
    /// <returns>The current grid state.</returns>
    public DataGridPersistedState GetState(Func<T, string>? itemKeySelector = null)
    {
        var state = new DataGridPersistedState
        {
            Page = CurrentPage,
            PageSize = RowsPerPage,
            Sorts = SortDefinitions.Values
                .OrderBy(sortDefinition => sortDefinition.Index)
                .Select(sortDefinition => new DataGridSortState
                {
                    Column = GetColumnIdentifierForSort(sortDefinition.SortBy),
                    Descending = sortDefinition.Descending,
                    Index = sortDefinition.Index,
                })
                .ToList(),
            Filters = FilterDefinitions
                .Select(CreateFilterState)
                .Where(filterState => !string.IsNullOrWhiteSpace(filterState.Column))
                .ToList(),
            Groups = RenderedColumns
                .Where(column => column.GroupingState.Value)
                .OrderBy(column => column._groupByOrderState.Value)
                .Select(column => new DataGridGroupState
                {
                    Column = column.GetStateIdentifier() ?? string.Empty,
                    Order = column._groupByOrderState.Value,
                    Expanded = column._groupExpandedState.Value,
                })
                .ToList(),
        };

        if (itemKeySelector is null)
        {
            return state;
        }

        if (MultiSelection)
        {
            state.SelectedItemKeys = Selection
                .Select(itemKeySelector)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .ToList();
        }
        else if (_selectedItemState.Value is not null)
        {
            state.SelectedItemKey = itemKeySelector(_selectedItemState.Value);
        }

        return state;
    }

    /// <summary>
    /// Restores grid state from a serializable snapshot.
    /// </summary>
    /// <param name="state">The state to restore.</param>
    /// <param name="itemResolver">
    /// When provided, resolves persisted item keys back to data items for selection restore.
    /// </param>
    /// <remarks>
    /// Call this after the grid has rendered and columns are registered.
    /// Unknown columns in the saved state are skipped.
    /// </remarks>
    public async Task SetStateAsync(DataGridPersistedState state, Func<string, T?>? itemResolver = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        var removedSortDefinitions = new HashSet<string>(SortDefinitions.Keys);

        SortDefinitions.Clear();
        FilterDefinitions.Clear();

        foreach (var column in RenderedColumns)
        {
            await column.RemoveGrouping();
        }

        foreach (var groupState in state.Groups.OrderBy(group => group.Order))
        {
            var column = GetColumnByStateIdentifier(groupState.Column);
            if (column is null)
            {
                continue;
            }

            await column.SetGroupingAsync(true);
            await column._groupByOrderState.SetValueAsync(groupState.Order);
            await column._groupExpandedState.SetValueAsync(groupState.Expanded);
        }

        foreach (var sortState in state.Sorts.OrderBy(sort => sort.Index))
        {
            var column = GetColumnByStateIdentifier(sortState.Column);
            if (column?.PropertyName is null)
            {
                continue;
            }

            SortDefinitions[column.PropertyName] = new SortDefinition<T>(
                column.PropertyName,
                sortState.Descending,
                sortState.Index,
                column.GetLocalSortFunc(),
                column.Comparer);

            removedSortDefinitions.Remove(column.PropertyName);
        }

        foreach (var filterState in state.Filters)
        {
            var column = GetColumnByStateIdentifier(filterState.Column);
            if (column is null)
            {
                continue;
            }

            var filterDefinition = CreateFilterDefinitionInstance();
            filterDefinition.Column = column;
            filterDefinition.Title = filterState.Title ?? column.Title;
            filterDefinition.Operator = filterState.Operator;
            filterDefinition.Value = DeserializeFilterValue(filterState.ValueJson, filterState.ValueType);
            FilterDefinitions.Add(filterDefinition);
        }

        var pageSize = state.PageSize > 0 ? state.PageSize : 10;
        if (_rowsPerPage != pageSize)
        {
            _rowsPerPage = pageSize;
            await RowsPerPageChanged.InvokeAsync(pageSize);
        }

        var page = Math.Max(0, state.Page);
        var currentPageChanged = _currentPage != page;
        _currentPage = page;
        if (currentPageChanged)
        {
            await CurrentPageChanged.InvokeAsync(_currentPage);
        }

        await ApplySelectionStateAsync(state, itemResolver);

        SortChangedEvent?.Invoke(SortDefinitions, removedSortDefinitions);
        if (_isFirstRendered)
        {
            await SortChanged.InvokeAsync(new Dictionary<string, SortDefinition<T>>(SortDefinitions));
        }

        await InvokeServerLoadFunc();
        GroupItems();
        await NotifyFilterChangedAsync();

        if (HasServerData && CurrentPage * RowsPerPage > _serverData.TotalItems)
        {
            CurrentPage = 0;
        }

        PagerStateHasChangedEvent?.Invoke();
        StateHasChanged();
    }

    private async Task ApplySelectionStateAsync(DataGridPersistedState state, Func<string, T?>? itemResolver)
    {
        if (itemResolver is null)
        {
            return;
        }

        if (MultiSelection)
        {
            var selectedItems = state.SelectedItemKeys
                .Select(itemResolver)
                .Where(item => item is not null)
                .Cast<T>()
                .ToHashSet(Comparer);

            Selection.Clear();
            Selection.UnionWith(selectedItems);
            await _selectedItemsState.SetValueAsync(new HashSet<T>(selectedItems, Comparer));
            return;
        }

        if (string.IsNullOrWhiteSpace(state.SelectedItemKey))
        {
            Selection.Clear();
            await _selectedItemState.SetValueAsync(default);
            return;
        }

        var selectedItem = itemResolver(state.SelectedItemKey);
        Selection.Clear();
        if (selectedItem is not null)
        {
            Selection.Add(selectedItem);
        }

        await _selectedItemState.SetValueAsync(selectedItem);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Filter values are supplied by the application and serialized for optional persistence.")]
    private DataGridFilterState CreateFilterState(IFilterDefinition<T> filterDefinition)
    {
        var columnIdentifier = filterDefinition.Column?.GetStateIdentifier();
        var value = filterDefinition.Value;
        return new DataGridFilterState
        {
            Column = columnIdentifier ?? string.Empty,
            Title = filterDefinition.Title,
            Operator = filterDefinition.Operator,
            ValueJson = value is null ? null : JsonSerializer.Serialize(value, value.GetType(), s_filterValueJsonOptions),
            ValueType = value?.GetType().AssemblyQualifiedName,
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Filter values are supplied by the application and deserialized during optional state restore.")]
    [UnconditionalSuppressMessage("Trimming", "IL2057:UnrecognizedValuePassedToParameter", Justification = "The persisted type name is supplied by the application during optional state restore.")]
    private static object? DeserializeFilterValue(string? valueJson, string? valueType)
    {
        if (string.IsNullOrEmpty(valueJson))
        {
            return null;
        }

        var type = ResolveValueType(valueType) ?? typeof(string);
        return JsonSerializer.Deserialize(valueJson, type, s_filterValueJsonOptions);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2057:UnrecognizedValuePassedToParameter", Justification = "The persisted type name is supplied by the application during optional state restore.")]
    private static Type? ResolveValueType(string? valueType)
    {
        if (string.IsNullOrWhiteSpace(valueType))
        {
            return null;
        }

        return Type.GetType(valueType, throwOnError: false);
    }

    private string GetColumnIdentifierForSort(string sortBy)
    {
        var column = RenderedColumns.FirstOrDefault(renderedColumn => renderedColumn.PropertyName == sortBy);
        return column?.GetStateIdentifier() ?? sortBy;
    }

    /// <summary>
    /// Gets the column with the specified state identifier.
    /// </summary>
    /// <param name="columnIdentifier">The column identifier or property name.</param>
    /// <returns>The matching column, if found.</returns>
    public Column<T>? GetColumnByStateIdentifier(string? columnIdentifier)
    {
        if (string.IsNullOrWhiteSpace(columnIdentifier))
        {
            return null;
        }

        return RenderedColumns.FirstOrDefault(column =>
            string.Equals(column.GetStateIdentifier(), columnIdentifier, StringComparison.Ordinal));
    }
}

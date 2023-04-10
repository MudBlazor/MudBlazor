// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor;

#nullable enable
public class FilterTypeMapper<T>
{
    private readonly List<IFilterType<T>> _filters = new()
    {
        new BooleanFilterType<T>(),
        new DateTimeFilterType<T>(),
        new EnumFilterType<T>(),
        new GuidFilterType<T>(),
        new NumberFilterType<T>(),
        new StringFilterType<T>()
    };

    public IReadOnlyList<IFilterType<T>> Filters => _filters;

    public void AddFilterType(IFilterType<T> filterType) => _filters.Add(filterType);

    public void RemoveFilterType(IFilterType<T> filterType) => _filters.Remove(filterType);

    public void RemoveAll() => _filters.Clear();

    public static FilterTypeMapper<T> Default = new();
}

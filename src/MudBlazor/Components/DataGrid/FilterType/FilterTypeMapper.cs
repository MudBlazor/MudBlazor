// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor;

#nullable enable
public class FilterTypeMapper
{
    private readonly List<IFilterType> _filters = new()
    {
        new BooleanFilterType(),
        new DateTimeFilterType(),
        new EnumFilterType(),
        new GuidFilterType(),
        new NumberFilterType(),
        new StringFilterType()
    };

    public IReadOnlyList<IFilterType> Filters => _filters;

    public void AddFilterType(IFilterType filterType) => _filters.Add(filterType);

    public void RemoveFilterType(IFilterType filterType) => _filters.Remove(filterType);

    public void RemoveAll() => _filters.Clear();

    public static FilterTypeMapper Default = new();
}

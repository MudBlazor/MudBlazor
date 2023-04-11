// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor;

#nullable enable
public class FilterTypeMapper
{
    private readonly List<Type> _filterTypes = new()
    {
        typeof(BooleanFilterType<>),
        typeof(DateTimeFilterType<>),
        typeof(EnumFilterType<>),
        typeof(GuidFilterType<>),
        typeof(NumberFilterType<>),
        typeof(StringFilterType<>),
    };

    //public IReadOnlyList<IFilterType> Filters => new List<IFilterType>()
    //{
    //    new BooleanFilterType(),
    //    new DateTimeFilterType(),
    //    new EnumFilterType(),
    //    new GuidFilterType(),
    //    new NumberFilterType(),
    //    new StringFilterType()
    //};

    public IEnumerable<Type> Create<T>()
    {
        foreach (var filterType in _filterTypes)
        {
            var genericType = filterType.MakeGenericType(typeof(T));
            yield return genericType;
        }
    }

    //public void AddFilterType(IFilterType filterType) => _filters.Add(filterType);

    //public void RemoveFilterType(IFilterType filterType) => _filters.Remove(filterType);

    //public void RemoveAll() => _filters.Clear();

    public static FilterTypeMapper Default = new();
}

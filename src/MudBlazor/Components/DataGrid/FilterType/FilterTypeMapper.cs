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
        typeof(StringFilterType<>)
    };

    public virtual IEnumerable<Type> Create<T>()
    {
        foreach (var filterType in _filterTypes)
        {
            var genericType = filterType.MakeGenericType(typeof(T));
            yield return genericType;
        }
    }

    public void AddFilterType<T>() where T : FilterTypeBase<T>
    {
        _filterTypes.Add(typeof(T).GetGenericTypeDefinition());
    }

    public void RemoveFilterType<T>() where T : FilterTypeBase<T>
    {
        _filterTypes.Remove(typeof(T).GetGenericTypeDefinition());
    }

    public void RemoveAll() => _filterTypes.Clear();

    public static FilterTypeMapper Default = new();
}

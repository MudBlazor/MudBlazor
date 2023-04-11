// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
public abstract class FilterTypeBase<T> : ComponentBase
{
    [CascadingParameter(Name = "Filter")] 
    private Filter<T> FilterInternal { get; set; } = null!;

    [CascadingParameter(Name = "FilterDefinition")]
    private FilterDefinition<T> FilterDefinitionInternal { get; set; } = null!;

    protected FilterTypeBase()
    {
        FilterLazy = new Lazy<Filter<T>>(GetFilter);
        FilterDefinitionLazy = new Lazy<FilterDefinition<T>>(GetFilterDefinition);
    }

    protected Lazy<Filter<T>> FilterLazy { get; }

    protected Lazy<FilterDefinition<T>> FilterDefinitionLazy { get; }

    public abstract bool CanBeMapped();

    private Filter<T> GetFilter()
    {
        //ArgumentNullException.ThrowIfNull(FilterInternal);
        return FilterInternal;
    }

    private FilterDefinition<T> GetFilterDefinition()
    {
        //ArgumentNullException.ThrowIfNull(FilterDefinitionInternal);
        return FilterDefinitionInternal;
    }
}

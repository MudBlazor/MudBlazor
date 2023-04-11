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

    protected Filter<T> Filter
    {
        get
        {
            ArgumentNullException.ThrowIfNull(FilterInternal);
            return FilterInternal;
        }
    }

    protected FilterDefinition<T> FilterDefinition
    {
        get
        {
            ArgumentNullException.ThrowIfNull(FilterDefinitionInternal);
            return FilterDefinitionInternal;
        }
    }

    public abstract bool CanBeMapped();
}

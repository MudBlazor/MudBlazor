// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    public interface IFilterDefinition<T>
    {
        Guid Id { get; set; }

        Column<T>? Column { get; set; }

        string? Title { get; set; }

        string? Operator { get; set; }

        object? Value { get; set; }

        FieldType FieldType => FieldType.Identify(Column?.PropertyType);

        Func<T, bool> GenerateFilterFunction(FilterOptions? filterOptions = null);

        IFilterDefinition<T> Clone();
    }
}

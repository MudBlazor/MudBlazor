// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.UnitTests.Components;

#nullable enable
public class CustomFilterDefinitionMock<TType> : IFilterDefinition<TType>
{
    public Guid Id { get; set; }

    public Column<TType>? Column { get; set; }

    public string? Title { get; set; }

    public string? Operator { get; set; }

    public object? Value { get; set; }

    public Func<TType, bool> GenerateFilterFunction(FilterOptions? filterOptions = null)
    {
        return _ => true;
    }

    public IFilterDefinition<TType> Clone()
    {
        return new CustomFilterDefinitionMock<TType>()
        {
            Id = Id,
            Column = Column,
            Title = Title,
            Operator = Operator,
            Value = Value
        };
    }
}

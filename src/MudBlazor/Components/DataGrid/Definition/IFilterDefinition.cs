// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Defines filter definition features for a column.
/// </summary>
/// <typeparam name="T">The type of object being filtered.</typeparam>
public interface IFilterDefinition<T>
{
    /// <summary>
    /// The unique ID of this filter.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// The column for which this filter applies.
    /// </summary>
    Column<T>? Column { get; set; }

    /// <summary>
    /// The name of this filter.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// The kind of equality comparison to perform on values.
    /// </summary>
    /// <remarks>
    /// Values typically come from <see cref="FilterOperator"/> depending on the data type:
    /// <list type="table">
    /// <item><term><see cref="FilterOperator.Boolean" /></term><description>Operators for <see cref="bool"/> column types.</description></item>
    /// <item><term><see cref="FilterOperator.DateTime" /></term><description>Operators for <see cref="DateTime"/> column types.</description></item>
    /// <item><term><see cref="FilterOperator.Enum" /></term><description>Operators for <see cref="Enum"/> column types.</description></item>
    /// <item><term><see cref="FilterOperator.Guid" /></term><description>Operators for <see cref="Guid"/> column types.</description></item>
    /// <item><term><see cref="FilterOperator.Number" /></term><description>Operators for numeric column types.</description></item>
    /// <item><term><see cref="FilterOperator.String" /></term><description>Operators for <see cref="string"/> column types.</description></item>
    /// </list>
    /// </remarks>
    string? Operator { get; set; }

    /// <summary>
    /// The value to filter.
    /// </summary>
    object? Value { get; set; }

    FieldType FieldType => FieldType.Identify(Column?.PropertyType);

    Func<T, bool> GenerateFilterFunction(FilterOptions? filterOptions = null);

    IFilterDefinition<T> Clone();
}

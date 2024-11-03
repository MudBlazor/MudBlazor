// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Linq.Expressions;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a service which generates C# functions from text-based filter operations.
/// </summary>
public static class FilterExpressionGenerator
{
    /// <summary>
    /// Creates a C# expression for the specified filter and options.
    /// </summary>
    /// <typeparam name="T">The kind of object to filter.</typeparam>
    /// <param name="filter">The filter definition used to generate the expression.</param>
    /// <param name="filterOptions">Any options to apply such as case sensitivity.</param>
    /// <returns>An expression which can be executed to perform a filter.</returns>
    public static Expression<Func<T, bool>> GenerateExpression<T>(IFilterDefinition<T> filter, FilterOptions? filterOptions)
    {
        filterOptions ??= FilterOptions.Default; //Default if null
        var propertyExpression = filter.Column?.PropertyExpression;

        if (propertyExpression is null)
        {
            return x => true;
        }

        var fieldType = filter.FieldType;

        if (fieldType.IsString)
        {
            var value = filter.Value?.ToString();

            if (value is null && filter.Operator != FilterOperator.String.Empty && filter.Operator != FilterOperator.String.NotEmpty)
                return x => true;

            if (filterOptions.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Ignore)
            {
                return filter.Operator switch
                {
                    FilterOperator.String.Contains =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.Contains(value))),
                    FilterOperator.String.NotContains =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && !x.Contains(value))),
                    FilterOperator.String.Equal =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.Equals(value))),
                    FilterOperator.String.NotEqual =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && !x.Equals(value))),
                    FilterOperator.String.StartsWith =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.StartsWith(value))),
                    FilterOperator.String.EndsWith =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.EndsWith(value))),
                    FilterOperator.String.Empty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => string.IsNullOrWhiteSpace(x))),
                    FilterOperator.String.NotEmpty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => !string.IsNullOrWhiteSpace(x))),
                    _ => x => true
                };
            }

            var stringComparer = filterOptions.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            return filter.Operator switch
            {
                FilterOperator.String.Contains =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.Contains(value, stringComparer))),
                FilterOperator.String.NotContains =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && !x.Contains(value, stringComparer))),
                FilterOperator.String.Equal =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.Equals(value, stringComparer))),
                FilterOperator.String.NotEqual =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && !x.Equals(value, stringComparer))),
                FilterOperator.String.StartsWith =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.StartsWith(value, stringComparer))),
                FilterOperator.String.EndsWith =>
                    propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.EndsWith(value, stringComparer))),
                FilterOperator.String.Empty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => string.IsNullOrWhiteSpace(x))),
                FilterOperator.String.NotEmpty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => !string.IsNullOrWhiteSpace(x))),
                _ => x => true
            };
        }

        if (fieldType.IsNumber)
        {
            if (filter.Value is null && filter.Operator != FilterOperator.Number.Empty && filter.Operator != FilterOperator.Number.NotEmpty)
                return x => true;

            return filter.Operator switch
            {
                FilterOperator.Number.Equal => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, filter.Value),
                FilterOperator.Number.NotEqual => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, filter.Value),
                FilterOperator.Number.GreaterThan => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, filter.Value),
                FilterOperator.Number.GreaterThanOrEqual => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, filter.Value),
                FilterOperator.Number.LessThan => propertyExpression.GenerateBinary<T>(ExpressionType.LessThan, filter.Value),
                FilterOperator.Number.LessThanOrEqual => propertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, filter.Value),
                FilterOperator.Number.Empty => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                FilterOperator.Number.NotEmpty => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                _ => x => true
            };
        }

        if (fieldType.IsDateTime)
        {
            if (filter.Value is null && filter.Operator != FilterOperator.DateTime.Empty && filter.Operator != FilterOperator.DateTime.NotEmpty)
                return x => true;

            return filter.Operator switch
            {
                FilterOperator.DateTime.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, filter.Value),
                FilterOperator.DateTime.IsNot => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, filter.Value),
                FilterOperator.DateTime.After => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, filter.Value),
                FilterOperator.DateTime.OnOrAfter => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, filter.Value),
                FilterOperator.DateTime.Before => propertyExpression.GenerateBinary<T>(ExpressionType.LessThan, filter.Value),
                FilterOperator.DateTime.OnOrBefore => propertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, filter.Value),
                FilterOperator.DateTime.Empty => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                FilterOperator.DateTime.NotEmpty => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                _ => x => true
            };
        }

        if (fieldType.IsBoolean)
        {
            if (filter.Value is null)
                return x => true;

            return filter.Operator switch
            {
                FilterOperator.Boolean.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, filter.Value),
                _ => x => true
            };
        }

        if (fieldType.IsEnum)
        {
            if (filter.Value is null)
                return x => true;

            return filter.Operator switch
            {
                FilterOperator.Enum.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, filter.Value),
                FilterOperator.Enum.IsNot => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, filter.Value),
                _ => x => true
            };
        }

        if (fieldType.IsGuid)
        {
            return filter.Operator switch
            {
                FilterOperator.Guid.Equal => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, filter.Value),
                FilterOperator.Guid.NotEqual => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, filter.Value),
                _ => x => true
            };
        }

        return x => true;
    }
}

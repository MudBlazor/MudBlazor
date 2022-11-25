// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor
{
    public class FilterTypeExpressionGenerator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly FilterDefinition<T> _filterDefinition;

        public FilterTypeExpressionGenerator(FilterDefinition<T> filterDefinition)
        {
            _filterDefinition = filterDefinition;
        }

        public Expression<Func<T, bool>> GenerateFilterExpression(FieldType fieldType)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression;

            if (fieldType.IsString)
            {
                expression = GenerateFilterExpressionForStringType(parameter);
            }
            else if (fieldType.IsNumber)
            {
                expression = GenerateFilterExpressionForNumericTypes(parameter);
            }
            else if (fieldType.IsEnum)
            {
                expression = GenerateFilterExpressionForEnumTypes(parameter);
            }
            else if (fieldType.IsBoolean)
            {
                expression = GenerateFilterExpressionForBooleanTypes(parameter);
            }
            else if (fieldType.IsDateTime)
            {
                expression = GenerateFilterExpressionForDateTimeTypes(parameter);
            }
            else if (fieldType.IsGuid)
            {
                expression = GenerateFilterExpressionForGuidTypes(parameter);
            }
            else
            {
                expression = Expression.Constant(true, typeof(bool));
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }

        public Expression GenerateFilterExpressionForDateTimeTypes(ParameterExpression parameter)
        {
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(DateTime?));
            var valueDateTime = (DateTime?)_filterDefinition.Value;
            var isNotNullExpression = Expression.NotEqual(fieldExpression, Expression.Constant(null));
            var isNullExpression = Expression.Equal(fieldExpression, Expression.Constant(null));
            var notNullDateTimeExpression = Expression.Convert(fieldExpression, typeof(DateTime));
            var valueDateTimeConstantExpression = Expression.Constant(valueDateTime);

            return _filterDefinition.Operator switch
            {
                FilterOperator.DateTime.Is when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.IsNot when _filterDefinition.Value is not null =>
                    Expression.OrElse(isNullExpression,
                        Expression.NotEqual(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.After when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.GreaterThan(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.OnOrAfter when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.GreaterThanOrEqual(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.Before when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.LessThan(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.OnOrBefore when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.LessThanOrEqual(notNullDateTimeExpression, valueDateTimeConstantExpression)),

                FilterOperator.DateTime.Empty => isNullExpression,
                FilterOperator.DateTime.NotEmpty => isNotNullExpression,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        public Expression GenerateFilterExpressionForStringType(ParameterExpression parameter)
        {
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Property(parameter, propertyInfo);
            var valueString = _filterDefinition.Value?.ToString();

            var trimExpression = Expression.Call(fieldExpression, ReflectionString.GetTrimMethodInfo(_filterDefinition.FieldType));
            var isNullExpression = Expression.Equal(fieldExpression, Expression.Constant(null));
            var isNotNullExpression = Expression.NotEqual(fieldExpression, Expression.Constant(null));

            return _filterDefinition.Operator switch
            {
                FilterOperator.String.Contains when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression, Expression.Call(fieldExpression, ReflectionString.GetContainsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType }), Expression.Constant(valueString))),

                FilterOperator.String.Contains when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetContainsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),

                FilterOperator.String.NotContains when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Call(fieldExpression, ReflectionString.GetContainsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType }), Expression.Constant(valueString)))),

                FilterOperator.String.NotContains when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Call(fieldExpression, ReflectionString.GetContainsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) }))),

                FilterOperator.String.Equal when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(fieldExpression, Expression.Constant(valueString))),

                FilterOperator.String.Equal when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetEqualsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),

                FilterOperator.String.NotEqual when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Equal(fieldExpression, Expression.Constant(valueString)))),

                FilterOperator.String.NotEqual when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Call(fieldExpression, ReflectionString.GetEqualsMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) }))),

                FilterOperator.String.StartsWith when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetStartsWithMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType }), Expression.Constant(valueString))),

                FilterOperator.String.StartsWith when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetStartsWithMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),

                FilterOperator.String.EndsWith when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetEndsWithMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType }), Expression.Constant(valueString))),

                FilterOperator.String.EndsWith when _filterDefinition.Value is not null && _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.CaseInsensitive =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(fieldExpression, ReflectionString.GetEndsWithMethodInfo(_filterDefinition.FieldType, new[] { _filterDefinition.FieldType, typeof(StringComparison) }), new Expression[] { Expression.Constant(valueString), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),

                FilterOperator.String.Empty =>
                    Expression.OrElse(isNullExpression,
                        Expression.Equal(trimExpression, Expression.Constant(string.Empty, _filterDefinition.FieldType))),

                FilterOperator.String.NotEmpty =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.NotEqual(trimExpression, Expression.Constant(string.Empty, _filterDefinition.FieldType))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        public Expression GenerateFilterExpressionForEnumTypes(ParameterExpression parameter)
        {
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Convert(Expression.Property(parameter, propertyInfo), _filterDefinition.FieldType);
            var valueEnum = (Enum?)_filterDefinition.Value;
            var nullExpression = Expression.Convert(Expression.Constant(null), _filterDefinition.FieldType);
            var isNullExpression = Expression.Equal(fieldExpression, nullExpression);
            var isNotNullExpression = Expression.NotEqual(fieldExpression, nullExpression);
            var valueEnumConstantExpression = Expression.Convert(Expression.Constant(valueEnum), _filterDefinition.FieldType);

            static bool IsNullableEnum(Type type)
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return underlyingType is { IsEnum: true };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.Enum.Is when _filterDefinition.Value != null =>
                    IsNullableEnum(_filterDefinition.FieldType)
                        ? Expression.AndAlso(isNotNullExpression,
                            Expression.Equal(fieldExpression, valueEnumConstantExpression))
                        : Expression.Equal(fieldExpression, valueEnumConstantExpression),

                FilterOperator.Enum.IsNot when _filterDefinition.Value != null =>
                    IsNullableEnum(_filterDefinition.FieldType)
                        ? Expression.OrElse(isNullExpression,
                            Expression.NotEqual(fieldExpression, valueEnumConstantExpression))
                        : Expression.NotEqual(fieldExpression, valueEnumConstantExpression),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        public Expression GenerateFilterExpressionForBooleanTypes(ParameterExpression parameter)
        {
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(bool?));
            bool? valueBool = _filterDefinition.Value is null ? null : Convert.ToBoolean(_filterDefinition.Value);
            var isNotNullExpression = Expression.NotEqual(fieldExpression, Expression.Constant(null));
            var notNullBoolExpression = Expression.Convert(fieldExpression, typeof(bool));

            return _filterDefinition.Operator switch
            {
                FilterOperator.Enum.Is when _filterDefinition.Value is not null => Expression.AndAlso(isNotNullExpression,
                    Expression.Equal(notNullBoolExpression, Expression.Constant(valueBool))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        public Expression GenerateFilterExpressionForNumericTypes(ParameterExpression parameter)
        {
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(double?));
            double? valueNumber = _filterDefinition.Value is null ? null : Convert.ToDouble(_filterDefinition.Value);
            var isNotNullExpression = Expression.NotEqual(fieldExpression, Expression.Constant(null));
            var isNullExpression = Expression.Equal(fieldExpression, Expression.Constant(null));
            var notNullNumberExpression = Expression.Convert(fieldExpression, typeof(double));
            var valueNumberConstant = Expression.Constant(valueNumber);

            return _filterDefinition.Operator switch
            {
                FilterOperator.Number.Equal when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.NotEqual when _filterDefinition.Value is not null =>
                    Expression.OrElse(isNullExpression,
                        Expression.NotEqual(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.GreaterThan when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.GreaterThan(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.GreaterThanOrEqual when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.GreaterThanOrEqual(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.LessThan when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.LessThan(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.LessThanOrEqual when _filterDefinition.Value is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.LessThanOrEqual(notNullNumberExpression, valueNumberConstant)),

                FilterOperator.Number.Empty => isNullExpression,
                FilterOperator.Number.NotEmpty => isNotNullExpression,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        public Expression GenerateFilterExpressionForGuidTypes(ParameterExpression parameter)
        {
            const string HasValuePropertyName = "HasValue";
            var propertyInfo = GetPropertySafe<T>(_filterDefinition.Field);
            var fieldExpression = Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(Guid?));
            var valueGuid = ((string?)_filterDefinition.Value)?.ParseGuid();
            var isNotNullExpression = Expression.IsTrue(Expression.Property(fieldExpression, typeof(Guid?), HasValuePropertyName));
            var isNullExpression = Expression.IsFalse(Expression.Property(fieldExpression, typeof(Guid?), HasValuePropertyName));
            var notNullGuidExpression = Expression.Convert(fieldExpression, typeof(Guid));


            return _filterDefinition.Operator switch
            {
                FilterOperator.Guid.Equal when valueGuid is not null =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(notNullGuidExpression, Expression.Constant(valueGuid))),

                FilterOperator.Guid.NotEqual when valueGuid is not null =>
                    Expression.OrElse(
                        isNullExpression,
                        Expression.NotEqual(notNullGuidExpression, Expression.Constant(valueGuid))),

                // filtered value is not a valid GUID
                _ when valueGuid is null && _filterDefinition.Value is not null =>
                    Expression.Constant(false),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private static PropertyInfo GetPropertySafe<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string propertyName) => GetPropertySafe(typeof(T), propertyName);

        private static PropertyInfo GetPropertySafe([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type, string propertyName)
        {
            var fieldProperty = type.GetProperty(propertyName);
            if (fieldProperty is null)
            {
                throw new InvalidOperationException($"Property {propertyName} not found in {type.Name}.");
            }

            return fieldProperty;
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;
using MudBlazor.Utilities;

namespace MudBlazor
{
    [RequiresUnreferencedCode(CodeMessage.SerializationUnreferencedCodeMessage)]
    public class FilterDefinition<T>
    {
        internal MudDataGrid<T> DataGrid { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Field { get; set; }
        public string Title { get; set; }
        public Type FieldType { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public Func<T, bool> FilterFunction { get; set; }

        private Type dataType
        {
            get
            {
                if (FieldType != null)
                    return FieldType;

                if (Field == null)
                    return typeof(object);

                if (typeof(T) == typeof(IDictionary<string, object>) && FieldType == null)
                    throw new ArgumentNullException(nameof(FieldType));

                return typeof(T).GetProperty(Field).PropertyType;
            }
        }

        private bool isNumber
        {
            get
            {
                return FilterOperator.IsNumber(dataType);
            }
        }

        private bool isEnum
        {
            get
            {
                return FilterOperator.IsEnum(dataType);
            }
        }

        private bool isDateTime
        {
            get
            {
                return FilterOperator.IsDateTime(dataType);
            }
        }

        private bool isBoolean
        {
            get
            {
                return FilterOperator.IsBoolean(dataType);
            }
        }

        private bool isGuid
        {
            get
            {
                return FilterOperator.IsGuid(dataType);
            }
        }

        public Func<T, bool> GenerateFilterFunction()
        {
            if (FilterFunction != null)
                return FilterFunction;

            // Handle case where we have an IDictionary.
            if (typeof(T) == typeof(IDictionary<string, object>))
            {
                if (dataType == typeof(string))
                {
                    return GenerateFilterForStringTypeInIDictionary();
                }
                else if (isNumber)
                {
                    return GenerateFilterForNumericTypesInIDictionary();
                }
                else if (isEnum)
                {
                    return GenerateFilterForEnumTypesInIDictionary();
                }
                else if (isBoolean)
                {
                    return GenerateFilterForBooleanTypeInIDictionary();
                }
                else if (isDateTime)
                {
                    return GenerateFilterForDateTimeTypeInIDictionary();
                }
                else if (isGuid)
                {
                    return GenerateFilterForGuidTypeInIDictionary();
                }

                return x => true;
            }
            else
            {
                var expression = GenerateFilterExpression();

                return expression.Compile();
            }
        }

        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression;

            if (dataType == typeof(string))
            {
                expression = GenerateFilterExpressionForStringType(parameter);
            }
            else if (isNumber)
            {
                expression = GenerateFilterExpressionForNumericTypes(parameter);
            }
            else if (isEnum)
            {
                expression = GenerateFilterExpressionForEnumTypes(parameter);
            }
            else if (isBoolean)
            {
                expression = GenerateFilterExpressionForBooleanTypes(parameter);
            }
            else if (isDateTime)
            {
                expression = GenerateFilterExpressionForDateTimeTypes(parameter);
            }
            else if (isGuid)
            {
                expression = GenerateFilterExpressionForGuidTypes(parameter);
            }
            else
            {
                expression = Expression.Constant(true, typeof(bool));
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }

        private Expression GenerateFilterExpressionForDateTimeTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(DateTime?));
            DateTime? valueDateTime = Value == null ? null : (DateTime)Value;
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));
            var isnull = Expression.Equal(field, Expression.Constant(null));
            var notNullDateTime = Expression.Convert(field, typeof(DateTime));
            var valueDateTimeConstant = Expression.Constant(valueDateTime);

            return Operator switch
            {
                FilterOperator.DateTime.Is when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.IsNot when null != Value =>
                    Expression.OrElse(isnull,
                        Expression.NotEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.After when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThan(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.OnOrAfter when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThanOrEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.Before when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThan(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.OnOrBefore when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThanOrEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.Empty => isnull,
                FilterOperator.DateTime.NotEmpty => isnotnull,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForBooleanTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(bool?));
            bool? valueBool = Value == null ? null : Convert.ToBoolean(Value);
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));
            var notNullBool = Expression.Convert(field, typeof(bool));

            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null => Expression.AndAlso(isnotnull,
                    Expression.Equal(notNullBool, Expression.Constant(valueBool))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForGuidTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(Guid?));
            Guid? valueGuid = Value == null ? null : ParseGuid((String)Value);
            var isnotnull = Expression.IsTrue(Expression.Property(field, typeof(Guid?), "HasValue"));
            var isnull = Expression.IsFalse(Expression.Property(field, typeof(Guid?), "HasValue"));
            var notNullGuid = Expression.Convert(field, typeof(Guid));

            return Operator switch
            {
                FilterOperator.Guid.Equal when valueGuid != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(notNullGuid, Expression.Constant(valueGuid))),

                FilterOperator.Guid.NotEqual when valueGuid != null =>
                    Expression.OrElse(
                        isnull,
                        Expression.NotEqual(notNullGuid, Expression.Constant(valueGuid))),

                // filtered value is not a valid GUID
                _ when valueGuid == null && Value != null =>
                    Expression.Constant(false),
                
                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForEnumTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), dataType);
            var valueEnum = Value == null ? null : (Enum)Value;
            var _null = Expression.Convert(Expression.Constant(null), dataType);
            var isnull = Expression.Equal(field, _null);
            var isnotnull = Expression.NotEqual(field, _null);
            var valueEnumConstant = Expression.Convert(Expression.Constant(valueEnum), dataType);

            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null =>
                    IsNullableEnum(dataType) ? Expression.AndAlso(isnotnull,
                            Expression.Equal(field, valueEnumConstant))
                        : Expression.Equal(field, valueEnumConstant),

                FilterOperator.Enum.IsNot when Value != null =>
                    IsNullableEnum(dataType) ? Expression.OrElse(isnull,
                            Expression.NotEqual(field, valueEnumConstant))
                        : Expression.NotEqual(field, valueEnumConstant),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForNumericTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(double?));
            double? valueNumber = Value == null ? null : Convert.ToDouble(Value);
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));
            var isnull = Expression.Equal(field, Expression.Constant(null));
            var notNullNumber = Expression.Convert(field, typeof(double));
            var valueNumberConstant = Expression.Constant(valueNumber);

            return Operator switch
            {
                FilterOperator.Number.Equal when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.NotEqual when Value != null =>
                    Expression.OrElse(isnull,
                        Expression.NotEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.GreaterThan when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThan(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.GreaterThanOrEqual when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThanOrEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.LessThan when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThan(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.LessThanOrEqual when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThanOrEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.Empty => isnull,
                FilterOperator.Number.NotEmpty => isnotnull,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForStringType(ParameterExpression parameter)
        {
            var field = Expression.Property(parameter, typeof(T).GetProperty(Field));
            var valueString = Value?.ToString();
            var trim = Expression.Call(field, dataType.GetMethod("Trim", Type.EmptyTypes));
            var isnull = Expression.Equal(field, Expression.Constant(null));
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));

            return Operator switch
            {
                FilterOperator.String.Contains when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.NotContains when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Not(Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString)))),

                FilterOperator.String.Equal when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(field, Expression.Constant(valueString))),

                FilterOperator.String.NotEqual when Value != null =>
                    Expression.AndAlso(isnotnull,
                    Expression.Not(Expression.Equal(field, Expression.Constant(valueString)))),

                FilterOperator.String.StartsWith when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.EndsWith when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.Empty =>
                    Expression.OrElse(isnull,
                        Expression.Equal(trim, Expression.Constant(string.Empty, dataType))),

                FilterOperator.String.NotEmpty =>
                    Expression.AndAlso(isnotnull,
                        Expression.NotEqual(trim, Expression.Constant(string.Empty, dataType))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        #region IDictionary Filters

        private Func<T, bool> GenerateFilterForStringTypeInIDictionary()
        {
            var valueString = Value?.ToString();

            return Operator switch
            {
                FilterOperator.String.Contains when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null && v.Contains(valueString);
                }
                ,
                FilterOperator.String.NotContains when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null && !v.Contains(valueString);
                }
                ,

                FilterOperator.String.Equal when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return object.Equals(v, Value);
                }
                ,

                FilterOperator.String.NotEqual when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return !object.Equals(v, Value);
                }
                ,

                FilterOperator.String.StartsWith when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null && v.StartsWith(valueString);
                }
                ,

                FilterOperator.String.EndsWith when Value != null => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null && v.EndsWith(valueString);
                }
                ,

                FilterOperator.String.Empty => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return string.IsNullOrWhiteSpace(v);
                }
                ,

                FilterOperator.String.NotEmpty => x =>
                {
                    string v = GetStringFromObject(((IDictionary<string, object>)x)[Field]);

                    return !string.IsNullOrWhiteSpace(v);
                }
                ,

                _ => x => true
            };
        }

        private Func<T, bool> GenerateFilterForNumericTypesInIDictionary()
        {
            double? valueNumber = Value == null ? null : Convert.ToDouble(Value);

            return Operator switch
            {
                FilterOperator.Number.Equal when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v == valueNumber;
                }
                ,

                FilterOperator.Number.NotEqual when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != valueNumber;
                }
                ,

                FilterOperator.Number.GreaterThan when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v > valueNumber;
                }
                ,

                FilterOperator.Number.GreaterThanOrEqual when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v >= valueNumber;
                }
                ,

                FilterOperator.Number.LessThan when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v < valueNumber;
                }
                ,

                FilterOperator.Number.LessThanOrEqual when Value != null => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v <= valueNumber;
                }
                ,

                FilterOperator.Number.Empty => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v == null;
                }
                ,

                FilterOperator.Number.NotEmpty => x =>
                {
                    double? v = GetDoubleFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null;
                }
                ,

                _ => x => true
            };
        }

        private Func<T, bool> GenerateFilterForEnumTypesInIDictionary()
        {
            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null => x =>
                {
                    var v = GetEnumFromObject(((IDictionary<string, object>)x)[Field]);

                    return object.Equals(v, Value);
                }
                ,

                FilterOperator.Enum.IsNot when Value != null => x =>
                {
                    var v = GetEnumFromObject(((IDictionary<string, object>)x)[Field]);

                    return !object.Equals(v, Value);
                }
                ,

                _ => x => true
            };
        }

        private Func<T, bool> GenerateFilterForBooleanTypeInIDictionary()
        {
            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null => x =>
                {
                    var v = GetBoolFromObject(((IDictionary<string, object>)x)[Field]);

                    return object.Equals(v, Value);
                }
                ,

                _ => x => true
            };
        }

        private Func<T, bool> GenerateFilterForGuidTypeInIDictionary()
        {
            Guid? valueGuid = Value == null ? null : ParseGuid((string) Value);
            return Operator switch
            {
                FilterOperator.Guid.Equal when Value != null => x =>
                {
                    var v = GetGuidFromObject(((IDictionary<string, object>)x)[Field]);

                    return v == valueGuid;
                }
                ,
                FilterOperator.Guid.NotEqual when Value != null => x =>
                {
                    var v = GetGuidFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != valueGuid;
                },

                _ => x => true
            };
        }

        private Func<T, bool> GenerateFilterForDateTimeTypeInIDictionary()
        {
            DateTime? valueDateTime = Value == null ? null : (DateTime)Value;

            return Operator switch
            {
                FilterOperator.DateTime.Is when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v == valueDateTime;
                }
                ,

                FilterOperator.DateTime.IsNot when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != valueDateTime;
                }
                ,

                FilterOperator.DateTime.After when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v > valueDateTime;
                }
                ,

                FilterOperator.DateTime.OnOrAfter when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v >= valueDateTime;
                }
                ,

                FilterOperator.DateTime.Before when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v < valueDateTime;
                }
                ,

                FilterOperator.DateTime.OnOrBefore when Value != null => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v <= valueDateTime;
                }
                ,

                FilterOperator.DateTime.Empty => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v == null;
                }
                ,

                FilterOperator.DateTime.NotEmpty => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>)x)[Field]);

                    return v != null;
                }
                ,

                _ => x => true
            };
        }

        #endregion

        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        private string GetStringFromObject(object o)
        {
            if (o == null)
                return null;
            else if (o.GetType() == typeof(JsonElement))
            {
                return ((JsonElement)o).GetString();
            }
            else
            {
                return (string)o;
            }
        }

        private double? GetDoubleFromObject(object o)
        {
            if (o == null)
                return null;

            if (o.GetType() == typeof(JsonElement))
            {
                return ((JsonElement)o).GetDouble();
            }
            else
            {
                return Convert.ToDouble(o);
            }
        }

        private Enum GetEnumFromObject(object o)
        {
            if (o == null)
                return null;

            if (o.GetType() == typeof(JsonElement))
            {
                return (Enum)Enum.ToObject(FieldType, ((JsonElement)o).GetInt32());
            }
            else
            {
                return (Enum)Enum.ToObject(FieldType, o);
            }
        }

        private bool? GetBoolFromObject(object o)
        {
            if (o == null)
                return null;

            if (o.GetType() == typeof(JsonElement))
            {
                return ((JsonElement)o).GetBoolean();
            }
            else
            {
                return Convert.ToBoolean(o);
            }
        }

        private DateTime? GetDateTimeFromObject(object o)
        {
            if (o == null)
                return null;

            if (o.GetType() == typeof(JsonElement))
            {
                return ((JsonElement)o).GetDateTime();
            }
            else
            {
                return Convert.ToDateTime(o);
            }
        }

        private Guid? GetGuidFromObject(object o)
        {
            if (o == null)
                return null;

            if (o.GetType() == typeof(JsonElement))
            {
                return ParseGuid(((JsonElement)o).GetString());
            }
            else
            {
                return ParseGuid(Convert.ToString(o));
            }
        }

        private Guid? ParseGuid(string value)
        {
            if (value != null && Guid.TryParse(value, out Guid guid))
            {
                return guid;
            }
            else
            {
                return null;
            }
        }
    }
}

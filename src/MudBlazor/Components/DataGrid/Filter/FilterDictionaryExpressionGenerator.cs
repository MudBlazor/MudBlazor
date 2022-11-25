// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MudBlazor
{
    public class FilterDictionaryExpressionGenerator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly FilterDefinition<T> _filterDefinition;

        public FilterDictionaryExpressionGenerator(FilterDefinition<T> filterDefinition)
        {
            _filterDefinition = filterDefinition;
        }

        public Func<T, bool> GenerateFilterFunction(FieldType fieldType)
        {
            if (fieldType.IsString)
            {
                return GenerateFilterForStringTypeInIDictionary();
            }
            if (fieldType.IsNumber)
            {
                return GenerateFilterForNumericTypesInIDictionary();
            }
            if (fieldType.IsEnum)
            {
                return GenerateFilterForEnumTypesInIDictionary();
            }
            if (fieldType.IsBoolean)
            {
                return GenerateFilterForBooleanTypeInIDictionary();
            }
            if (fieldType.IsDateTime)
            {
                return GenerateFilterForDateTimeTypeInIDictionary();
            }
            if (fieldType.IsGuid)
            {
                return GenerateFilterForGuidTypeInIDictionary();
            }

            return _ => true;
        }

        public Func<T, bool> GenerateFilterForNumericTypesInIDictionary()
        {
            double? valueNumber = _filterDefinition.Value is null ? null : Convert.ToDouble(_filterDefinition.Value);

            static double? GetDoubleFromObject(object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => element.GetDouble(),
                    _ => Convert.ToDouble(obj)
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.Number.Equal when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);
                    //TODO: FIX ME THE POSSIBLE LOSS OF PRECISION
                    return @double == valueNumber;
                },
                FilterOperator.Number.NotEqual when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double != valueNumber;
                },
                FilterOperator.Number.GreaterThan when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double > valueNumber;
                },
                FilterOperator.Number.GreaterThanOrEqual when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double >= valueNumber;
                },
                FilterOperator.Number.LessThan when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double < valueNumber;
                },
                FilterOperator.Number.LessThanOrEqual when _filterDefinition.Value != null => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double <= valueNumber;
                },
                FilterOperator.Number.Empty => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double is null;
                },
                FilterOperator.Number.NotEmpty => x =>
                {
                    var @double = GetDoubleFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return @double is not null;
                },

                _ => _ => true
            };
        }

        public Func<T, bool> GenerateFilterForBooleanTypeInIDictionary()
        {
            static bool? GetBoolFromObject(object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => element.GetBoolean(),
                    _ => Convert.ToBoolean(obj)
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.Enum.Is when _filterDefinition.Value is not null => x =>
                {
                    var @bool = GetBoolFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return Equals(@bool, _filterDefinition.Value);
                }
                ,

                _ => _ => true
            };
        }

        public Func<T, bool> GenerateFilterForDateTimeTypeInIDictionary()
        {
            var valueDateTime = (DateTime?)_filterDefinition.Value;

            static DateTime? GetDateTimeFromObject(object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => element.GetDateTime(),
                    _ => Convert.ToDateTime(obj)
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.DateTime.Is when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime == valueDateTime;
                },
                FilterOperator.DateTime.IsNot when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime != valueDateTime;
                },
                FilterOperator.DateTime.After when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime > valueDateTime;
                },
                FilterOperator.DateTime.OnOrAfter when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime >= valueDateTime;
                },
                FilterOperator.DateTime.Before when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime < valueDateTime;
                },
                FilterOperator.DateTime.OnOrBefore when _filterDefinition.Value is not null => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime <= valueDateTime;
                },
                FilterOperator.DateTime.Empty => x =>
                {
                    var dateTime = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return dateTime is null;
                },
                FilterOperator.DateTime.NotEmpty => x =>
                {
                    var v = GetDateTimeFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return v is not null;
                },

                _ => _ => true
            };
        }

        public Func<T, bool> GenerateFilterForStringTypeInIDictionary()
        {
            var valueString = _filterDefinition.Value?.ToString() ?? string.Empty;
            var caseSensitivity = _filterDefinition.DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            static string? GetStringFromObject(object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => element.GetString(),
                    _ => (string)obj
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.String.Contains when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return str is not null && str.Contains(valueString, caseSensitivity);
                },
                FilterOperator.String.NotContains when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return str is not null && !str.Contains(valueString, caseSensitivity);
                },
                FilterOperator.String.Equal when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return str is not null && str.Equals(valueString, caseSensitivity);
                },
                FilterOperator.String.NotEqual when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return !valueString.Equals(str, caseSensitivity);
                },
                FilterOperator.String.StartsWith when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return str is not null && str.StartsWith(valueString, caseSensitivity);
                },
                FilterOperator.String.EndsWith when _filterDefinition.Value is not null => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return str is not null && str.EndsWith(valueString, caseSensitivity);
                },
                FilterOperator.String.Empty => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return string.IsNullOrWhiteSpace(str);
                },
                FilterOperator.String.NotEmpty => x =>
                {
                    var str = GetStringFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return !string.IsNullOrWhiteSpace(str);
                },

                _ => _ => true
            };
        }

        public Func<T, bool> GenerateFilterForGuidTypeInIDictionary()
        {
            var valueGuid = ((string?)_filterDefinition.Value)?.ParseGuid();

            static Guid? GetGuidFromObject(object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => element.GetString().ParseGuid(),
                    _ => Convert.ToString(obj).ParseGuid()
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.Guid.Equal when _filterDefinition.Value is not null => x =>
                {
                    var guid = GetGuidFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return guid == valueGuid;
                },
                FilterOperator.Guid.NotEqual when _filterDefinition.Value is not null => x =>
                {
                    var guid = GetGuidFromObject(((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return guid != valueGuid;
                },

                _ => _ => true
            };
        }

        public Func<T, bool> GenerateFilterForEnumTypesInIDictionary()
        {
            static Enum? GetEnumFromObject(Type fieldType, object? obj)
            {
                return obj switch
                {
                    null => null,
                    JsonElement element => (Enum)Enum.ToObject(fieldType, element.GetInt32()),
                    _ => (Enum)Enum.ToObject(fieldType, obj)
                };
            }

            return _filterDefinition.Operator switch
            {
                FilterOperator.Enum.Is when _filterDefinition.Value is not null => x =>
                {
                    var @enum = GetEnumFromObject(_filterDefinition.FieldType, ((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return Equals(@enum, _filterDefinition.Value);
                },
                FilterOperator.Enum.IsNot when _filterDefinition.Value is not null => x =>
                {
                    var @enum = GetEnumFromObject(_filterDefinition.FieldType, ((IDictionary<string, object>?)x)?[_filterDefinition.Field]);

                    return !Equals(@enum, _filterDefinition.Value);
                },

                _ => _ => true
            };
        }
    }
}

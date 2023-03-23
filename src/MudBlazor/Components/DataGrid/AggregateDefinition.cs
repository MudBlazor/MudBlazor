// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MudBlazor
{
#nullable enable
    public class AggregateDefinition<T>
    {
        private AggregateType? _cachedType;
        private Func<T, decimal>? _compiledAvgExpression;
        private Func<T, object>? _compiledMinMaxExpression;
        private Func<T, decimal>? _compiledSumExpression;

        public AggregateType Type { get; set; } = AggregateType.Count;

        public string DisplayFormat { get; set; } = "{value}";

        public Func<IEnumerable<T>, string>? CustomAggregate { get; set; }

        public string GetValue(LambdaExpression? propertyExpression, IEnumerable<T>? items)
        {
            //avoid multiple enumeration
            var itemsArray = items as T[] ?? items?.ToArray() ?? Array.Empty<T>();

            if (itemsArray.Length == 0)
            {
                return DisplayFormat.Replace("{value}", "0");
            }

            object? value = null;

            if (_cachedType != Type)
            {
                _cachedType = Type;

                if (Type == AggregateType.Avg)
                {
                    _compiledAvgExpression = propertyExpression?.ChangeExpressionReturnType<T, decimal>().Compile();
                    if (_compiledAvgExpression is not null)
                    {
                        value = itemsArray.Average(_compiledAvgExpression);
                    }
                }
                else if (Type == AggregateType.Count)
                {
                    value = itemsArray.Length;
                }
                else if (Type == AggregateType.Custom && CustomAggregate is not null)
                {
                    return CustomAggregate.Invoke(itemsArray);
                }
                else if (Type == AggregateType.Max)
                {
                    _compiledMinMaxExpression = propertyExpression?.ChangeExpressionReturnType<T, object>().Compile();
                    if (_compiledMinMaxExpression is not null)
                    {
                        value = itemsArray.Max(_compiledMinMaxExpression);
                    }
                }
                else if (Type == AggregateType.Min)
                {
                    _compiledMinMaxExpression = propertyExpression?.ChangeExpressionReturnType<T, object>().Compile();
                    if (_compiledMinMaxExpression is not null)
                    {
                        value = itemsArray.Min(_compiledMinMaxExpression);
                    }
                }
                else if (Type == AggregateType.Sum)
                {
                    _compiledSumExpression = propertyExpression?.ChangeExpressionReturnType<T, decimal>().Compile();
                    if (_compiledSumExpression is not null)
                    {
                        value = itemsArray.Sum(_compiledSumExpression);
                    }
                }
            }
            else
            {
                if (Type == AggregateType.Avg && _compiledAvgExpression is not null)
                {
                    value = itemsArray.Average(_compiledAvgExpression);
                }
                else if (Type == AggregateType.Count)
                {
                    value = itemsArray.Length;
                }
                else if (Type == AggregateType.Custom && CustomAggregate is not null)
                {
                    return CustomAggregate.Invoke(itemsArray);
                }
                else if (Type == AggregateType.Max && _compiledMinMaxExpression is not null)
                {
                    value = itemsArray.Max(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Min && _compiledMinMaxExpression is not null)
                {
                    value = itemsArray.Min(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Sum && _compiledSumExpression is not null)
                {
                    value = itemsArray.Sum(_compiledSumExpression);
                }
            }

            return DisplayFormat.Replace("{value}", (value ?? "").ToString());
        }

        public static AggregateDefinition<T> SimpleAvg()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Avg,
                DisplayFormat = "Average {value}"
            };
        }

        public static AggregateDefinition<T> SimpleCount()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Count,
                DisplayFormat = "Total {value}"
            };
        }

        public static AggregateDefinition<T> SimpleMax()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Max,
                DisplayFormat = "Max {value}"
            };
        }

        public static AggregateDefinition<T> SimpleMin()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Min,
                DisplayFormat = "Min {value}"
            };
        }

        public static AggregateDefinition<T> SimpleSum()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Sum,
                DisplayFormat = "Sum {value}"
            };
        }
    }
}

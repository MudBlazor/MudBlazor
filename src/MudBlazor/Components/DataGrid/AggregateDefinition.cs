// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MudBlazor
{
    public class AggregateDefinition<T>
    {
        public AggregateType Type { get; set; } = AggregateType.Count;
        public string DisplayFormat { get; set; } = "{value}";
        public Func<IEnumerable<T>, string> CustomAggregate { get; set; }

        private AggregateType? _cachedType;
        private Func<T, decimal> _compiledAvgExpression;
        private Func<T, object> _compiledMinMaxExpression;
        private Func<T, decimal> _compiledSumExpression;

        public string GetValue(string field, IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
            {
                return DisplayFormat.Replace("{value}", "0");
            }

            object value = null;

            if (_cachedType != Type)
            {
                _cachedType = Type;

                if (Type == AggregateType.Avg)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var f = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(field)), typeof(decimal));
                    _compiledAvgExpression = Expression.Lambda<Func<T, decimal>>(f, parameter).Compile();
                    value = items.Average(_compiledAvgExpression);
                }
                else if (Type == AggregateType.Count)
                {
                    value = items.Count();
                }
                else if (Type == AggregateType.Custom)
                {
                    return CustomAggregate.Invoke(items);
                }
                else if (Type == AggregateType.Max)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var f = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(field)), typeof(object));
                    _compiledMinMaxExpression = Expression.Lambda<Func<T, object>>(f, parameter).Compile();
                    value = items.Max(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Min)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var f = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(field)), typeof(object));
                    _compiledMinMaxExpression = Expression.Lambda<Func<T, object>>(f, parameter).Compile();
                    value = items.Min(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Sum)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var f = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(field)), typeof(decimal));
                    _compiledSumExpression = Expression.Lambda<Func<T, decimal>>(f, parameter).Compile();
                    value = items.Sum(_compiledSumExpression);
                }
            }
            else
            {
                if (Type == AggregateType.Avg)
                {
                    value = items.Average(_compiledAvgExpression);
                }
                else if (Type == AggregateType.Count)
                {
                    value = items.Count();
                }
                else if (Type == AggregateType.Custom)
                {
                    return CustomAggregate.Invoke(items);
                }
                else if (Type == AggregateType.Max)
                {
                    value = items.Max(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Min)
                {
                    value = items.Min(_compiledMinMaxExpression);
                }
                else if (Type == AggregateType.Sum)
                {
                    value = items.Sum(_compiledSumExpression);
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

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MudBlazor.Utilities.Expressions;

namespace MudBlazor
{
#nullable enable
    public class AggregateDefinition<T>
    {
        private readonly AggregateDefinitionExpressionCache _expressionCache = new();

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

            if (Type == AggregateType.Custom && CustomAggregate is not null)
            {
                return CustomAggregate.Invoke(itemsArray);
            }

            if (propertyExpression is null)
            {
                return DisplayFormat.Replace("{value}", "0");
            }

            if (Type == AggregateType.Count)
            {
                var value = itemsArray.Length;
                return DisplayFormat.Replace("{value}", value.ToString());
            }

            var expression = propertyExpression.ChangeExpressionReturnType<T, decimal?>();
            var compiledExpression = _expressionCache.CachedCompile(expression);

            if (Type == AggregateType.Avg)
            {
                var value = itemsArray.Average(compiledExpression);
                return DisplayFormat.Replace("{value}", value.ToString());
            }

            if (Type == AggregateType.Max)
            {
                var value = itemsArray.Max(compiledExpression);
                return DisplayFormat.Replace("{value}", value.ToString());
            }

            if (Type == AggregateType.Min)
            {
                var value = itemsArray.Min(compiledExpression);
                return DisplayFormat.Replace("{value}", value.ToString());
            }

            if (Type == AggregateType.Sum)
            {
                var value = itemsArray.Sum(compiledExpression);
                return DisplayFormat.Replace("{value}", value.ToString());
            }

            return DisplayFormat.Replace("{value}", "0");
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

        internal class AggregateDefinitionExpressionCache
        {
            //Concrete type since all Functions converts to Func<T, decimal?>
            //"Delegate" could be used, but then we will lose some performance
            private readonly ConcurrentDictionary<int, Func<T, decimal?>> _cache = new();

            public Func<T, decimal?> CachedCompile(Expression<Func<T, decimal?>> expression)
            {
                var cacheKey = ExpressionHasher.GetHashCode(expression);
                var cacheObject = _cache.GetOrAdd(cacheKey, _ => expression.Compile());
                
                return cacheObject;
            }
        }
    }
}

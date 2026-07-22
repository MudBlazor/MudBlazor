// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MudBlazor.Utilities.Expressions;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a function which calculates aggregate values such as counts, sums, averages, and custom functions.
    /// </summary>
    /// <typeparam name="T">The type of object to aggregate.</typeparam>
    public class AggregateDefinition<T>
    {
        private readonly AggregateDefinitionExpressionCache _expressionCache = new();

        /// <summary>
        /// The type of aggregate calculation to perform.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="AggregateType.Count"/>.  When <see cref="AggregateType.Custom"/>, the function defined in <see cref="CustomAggregate"/> is used.
        /// </remarks>
        public AggregateType Type { get; set; } = AggregateType.Count;

        /// <summary>
        /// A numeric format string.
        /// </summary>
        /// <remarks>
        /// Defaults to null.
        /// </remarks>
        public string? NumberFormat { get; set; }

        /// <summary>
        /// An object that supplies culture-specific formatting information.
        /// </summary>
        /// <remarks>
        /// Defaults to null.
        /// </remarks>
        public CultureInfo? Culture { get; set; }

        /// <summary>
        /// The format used to display aggregate values with prepended or appended text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>{value}</c>.
        /// </remarks>
        public string DisplayFormat { get; set; } = "{value}";

        /// <summary>
        /// The custom function used to calculate aggregate values.
        /// </summary>
        /// <remarks>
        /// This function is used when <see cref="Type"/> is <see cref="AggregateType.Custom"/>.
        /// </remarks>
        public Func<IEnumerable<T>, string>? CustomAggregate { get; set; }

        /// <summary>
        /// Calculates the aggregate value to display.
        /// </summary>
        /// <param name="propertyExpression">The expression which calculates the aggregate.</param>
        /// <param name="items">The items involved in the aggregate calculation.</param>
        /// <returns>The calculated aggregate value formatted using <see cref="DisplayFormat"/>.</returns>
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
                return DisplayFormat.Replace("{value}", value.ToString(NumberFormat, Culture));
            }

            var expression = propertyExpression.ChangeExpressionReturnType<T, decimal?>();
            var compiledExpression = _expressionCache.CachedCompile(expression);

            if (Type == AggregateType.Avg)
            {
                var value = itemsArray.Average(compiledExpression);
                return DisplayFormat.Replace("{value}", value != null ? value.Value.ToString(NumberFormat, Culture) : value.ToString());
            }

            if (Type == AggregateType.Max)
            {
                var value = itemsArray.Max(compiledExpression);
                return DisplayFormat.Replace("{value}", value != null ? value.Value.ToString(NumberFormat, Culture) : value.ToString());
            }

            if (Type == AggregateType.Min)
            {
                var value = itemsArray.Min(compiledExpression);
                return DisplayFormat.Replace("{value}", value != null ? value.Value.ToString(NumberFormat, Culture) : value.ToString());
            }

            if (Type == AggregateType.Sum)
            {
                var value = itemsArray.Sum(compiledExpression);
                return DisplayFormat.Replace("{value}", value != null ? value.Value.ToString(NumberFormat, Culture) : value.ToString());
            }

            return DisplayFormat.Replace("{value}", "0");
        }

        /// <summary>
        /// Represents a basic average aggregate calculation.
        /// </summary>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Avg"/> and a <see cref="DisplayFormat"/> of <c>Average {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleAvg()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Avg,
                DisplayFormat = "Average {value}"
            };
        }

        /// <summary>
        /// Represents a basic average aggregate calculation with custom format.
        /// </summary>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Avg"/> and a <see cref="DisplayFormat"/> of <c>Average {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleAvg(string? numberFormat, CultureInfo? culture = null)
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Avg,
                DisplayFormat = "Average {value}",
                NumberFormat = numberFormat,
                Culture = culture
            };
        }

        /// <summary>
        /// Represents a basic count aggregate calculation.
        /// </summary>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Count"/> and a <see cref="DisplayFormat"/> of <c>Total {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleCount()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Count,
                DisplayFormat = "Total {value}"
            };
        }

        /// <summary>
        /// Represents a basic count aggregate calculation with custom format.
        /// </summary>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>/// 
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Count"/> and a <see cref="DisplayFormat"/> of <c>Total {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleCount(string? numberFormat, CultureInfo? culture = null)
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Count,
                DisplayFormat = "Total {value}",
                NumberFormat = numberFormat,
                Culture = culture
            };
        }

        /// <summary>
        /// Represents a basic maximum aggregate calculation.
        /// </summary>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Max"/> and a <see cref="DisplayFormat"/> of <c>Max {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleMax()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Max,
                DisplayFormat = "Max {value}"
            };
        }

        /// <summary>
        /// Represents a basic maximum aggregate calculation with custom format.
        /// </summary>
        /// <returns>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>/// /// 
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Max"/> and a <see cref="DisplayFormat"/> of <c>Max {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleMax(string? numberFormat, CultureInfo? culture = null)
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Max,
                DisplayFormat = "Max {value}",
                NumberFormat = numberFormat,
                Culture = culture
            };
        }

        /// <summary>
        /// Represents a basic minimum aggregate calculation.
        /// </summary>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Min"/> and a <see cref="DisplayFormat"/> of <c>Min {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleMin()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Min,
                DisplayFormat = "Min {value}"
            };
        }

        /// <summary>
        /// Represents a basic minimum aggregate calculation with custom format.
        /// </summary>
        /// <returns>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>/// /// 
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Min"/> and a <see cref="DisplayFormat"/> of <c>Min {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleMin(string? numberFormat, CultureInfo? culture = null)
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Min,
                DisplayFormat = "Min {value}",
                NumberFormat = numberFormat,
                Culture = culture
            };
        }

        /// <summary>
        /// Represents a basic sum aggregate calculation.
        /// </summary>
        /// <returns>
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Sum"/> and a <see cref="DisplayFormat"/> of <c>Sum {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleSum()
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Sum,
                DisplayFormat = "Sum {value}"
            };
        }

        /// <summary>
        /// Represents a basic sum aggregate calculation with custom format.
        /// </summary>
        /// <returns>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>/// /// /// 
        /// An aggregate definition with a <see cref="Type"/> of <see cref="AggregateType.Sum"/> and a <see cref="DisplayFormat"/> of <c>Sum {value}</c>.
        /// </returns>
        public static AggregateDefinition<T> SimpleSum(string? numberFormat, CultureInfo? culture = null)
        {
            return new AggregateDefinition<T>
            {
                Type = AggregateType.Sum,
                DisplayFormat = "Sum {value}",
                NumberFormat = numberFormat,
                Culture = culture
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

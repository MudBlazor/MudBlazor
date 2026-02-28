// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MudBlazor
{
    public static class ExpressionExtensions
    {
        public static string GetFullPathOfMember<T>(this Expression<Func<T>> property)
        {
            var pathSegments = new List<string>();
            var p = property.Body as MemberExpression;

            while (p is not null)
            {
                if (p.Expression is MemberExpression)
                {
                    pathSegments.Add(p.Member.Name);
                }
                p = p.Expression as MemberExpression;
            }

            pathSegments.Reverse();
            return string.Join(".", pathSegments);
        }

        /// <summary>
        /// Returns the display name attribute of the provided field property as a string. If this attribute is missing, the member name will be returned.
        /// </summary>
        public static string GetLabelString<T>(this Expression<Func<T>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;

            // Currently we have no solution for this which is trimming incompatible
            // A possible solution is to use source gen
#pragma warning disable IL2075
            var propertyInfo = memberExpression.Expression?.Type.GetProperty(memberExpression.Member.Name);
#pragma warning restore IL2075
            return propertyInfo?.GetCustomAttributes(typeof(LabelAttribute), true).Cast<LabelAttribute>().FirstOrDefault()?.Name ?? string.Empty;
        }
    }
}

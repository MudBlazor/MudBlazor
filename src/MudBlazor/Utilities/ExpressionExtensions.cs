// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudBlazor.Utilities
{
    public static class ExpressionExtensions
    {
        internal static string GetFullPathOfMember<T>(this Expression<Func<T>> property)
        {
            var resultingString = string.Empty;
            var p = property.Body as MemberExpression;

            while (p != null)
            {
                if (p.Expression is MemberExpression)
                {
                    resultingString = p.Member.Name + (resultingString != string.Empty ? "." : string.Empty) + resultingString;
                }
                p = p.Expression as MemberExpression;
            }
            return resultingString;
        }
    }
}

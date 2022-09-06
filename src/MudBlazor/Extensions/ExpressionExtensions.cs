// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Returns the display name attribute of the provided field property as a string. If this attribute is missing, the member name will be returned.
        /// </summary>
        public static string GetDisplayNameString<T>(this Expression<Func<T>> expression)
        {
            var memberInfo = GetMemberInfo(expression.Body);
            return memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? "";
        }

        private static MemberInfo GetMemberInfo(Expression expression) //see https://stackoverflow.com/a/5015911
        {
            var memberExpr = expression as MemberExpression; //get MemberExpression
            if (memberExpr == null) //if null, check unary expression. Some types return unary expressions rather than member expressions
            {
                if (expression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }
}

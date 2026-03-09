using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace MudBlazor
{
    public static class CustomHelper
    {
        [SuppressMessage("Trimming", "IL2075:\'this\' argument does not satisfy \'DynamicallyAccessedMembersAttribute\' in call to target method. The return value of the source method does not have matching annotations.")]
        [SuppressMessage("Trimming", "IL2070:\'this\' argument does not satisfy \'DynamicallyAccessedMembersAttribute\' in call to target method. The parameter of method does not have matching annotations.")]
        public static PropertyInfo SBS_PropertyInfo<T2>(this Expression<T2> expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            MemberExpression memberExpression = SBS_MemberExpression(expression);

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException($"Expression not a Property: {expression}", nameof(expression));
            }

            MethodInfo? getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod is null)
            {
                throw new ArgumentException($"Expression has no GET method: {expression}", nameof(expression));
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException($"Expression cannot be static: {expression}", nameof(expression));
            }

            if (memberExpression.Expression is null)
            {
                throw new ArgumentException($"MemberExpression has no expression: {expression}", nameof(expression));
            }

            Type realType = memberExpression.Expression.Type;
            if (realType == null)
            {
                throw new ArgumentException($"Expression has no DeclaringType: {expression}", nameof(expression));
            }

            PropertyInfo? realPropertyInfo = realType.GetProperty(propertyInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // If the "realType" is an interface which itself does not have the property you are looking for
            // because it is in a parent interface, then the property is not found.
            // In this case, the property must be loaded via the interfaces
            if (realPropertyInfo is null && realType.IsInterface)
            {
                realPropertyInfo = realType.GetInterfaces()
                    .SelectMany(type => type.GetProperties())
                    .FirstOrDefault(info => info.Name == propertyInfo.Name);
            }

            if (realPropertyInfo is null)
            {
                throw new ArgumentException($"Cannot get real property info: {expression}", nameof(expression));
            }

            return realPropertyInfo;
        }

        public static MemberExpression SBS_MemberExpression<T2>(this Expression<T2> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            Expression op = ((UnaryExpression)expression.Body).Operand;
            return (MemberExpression)op;
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable LocalizableElement
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable ConvertToExtensionBlock

namespace MudBlazor
{
    [SuppressMessage("Trimming",
        "IL2075:\'this\' argument does not satisfy \'DynamicallyAccessedMembersAttribute\' in call to target method. The return value of the source method does not have matching annotations.")]
    [SuppressMessage("Trimming",
        "IL2070:\'this\' argument does not satisfy \'DynamicallyAccessedMembersAttribute\' in call to target method. The parameter of method does not have matching annotations.")]
    public static class CustomHelper
    {
        public static PropertyInfo SBS_PropertyInfo<T2>(this Expression<T2> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            MemberExpression memberExpression = expression.SBS_MemberExpression();

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException($"The member of the expression is not a property: {expression}", nameof(expression));
            }

            MethodInfo? getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod is null)
            {
                throw new ArgumentException($"The expression has no GET method: {expression}", nameof(expression));
            }

            if (getMethod.IsStatic)
            {
                return propertyInfo;
            }

            if (memberExpression.Expression is null)
            {
                throw new ArgumentException($"The {memberExpression.Expression} of the MemberExpression is null: {expression}", nameof(expression));
            }

            Type realType = memberExpression.Expression.Type;
            if (realType == null)
            {
                throw new ArgumentException($"Expression has no DeclaringType: {expression}", nameof(expression));
            }

            PropertyInfo? realPropertyInfo = realType.GetProperty(propertyInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // If the "realType" is an interface which itself does not have the property you are looking for because it is in a parent interface,
            // then the property is not found.
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

        public static MemberExpression SBS_MemberExpression<T>(this Expression<T> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return SBS_ExtractMemberExpression(expression.Body, expression, 0);
        }


        // ReSharper disable once CognitiveComplexity
        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        private static MemberExpression SBS_ExtractMemberExpression(Expression body, Expression originalExpression, int counter)
        {
            counter++;

            if (counter >= 100)
            {
                throw new InvalidOperationException("ExpressionHelper: Maximum recursion depth reached");
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (body is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            if (body is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MemberExpression operandMemberExpression)
                {
                    return operandMemberExpression;
                }

                return SBS_ExtractMemberExpression(unaryExpression.Operand, originalExpression, counter);
            }

            if (body is ConditionalExpression conditionalExpression)
            {
                if (conditionalExpression.IfFalse.NodeType != ExpressionType.Constant)
                {
                    return SBS_ExtractMemberExpression(conditionalExpression.IfFalse, originalExpression, counter);
                }

                if (conditionalExpression.IfTrue.NodeType != ExpressionType.Constant)
                {
                    return SBS_ExtractMemberExpression(conditionalExpression.IfTrue, originalExpression, counter);
                }

                // Null-safe access (x?.Property) or ternary null check (x.Property == null ? null : ...)
                // usually results in a ConditionalExpression.
                // First, check if the condition is a null-check (e.g. x.Parent == null),
                // and if so, extract the member from the null-checked side of the condition.
                if (conditionalExpression.Test is BinaryExpression { NodeType: ExpressionType.Equal or ExpressionType.NotEqual } binaryExpression)
                {
                    Expression? nonNullSide = null;
                    if (binaryExpression.Right is ConstantExpression { Value: null })
                    {
                        nonNullSide = binaryExpression.Left;
                    }
                    else if (binaryExpression.Left is ConstantExpression { Value: null })
                    {
                        nonNullSide = binaryExpression.Right;
                    }

                    if (nonNullSide is not null)
                    {
                        try
                        {
                            return SBS_ExtractMemberExpression(nonNullSide, originalExpression, counter);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }

                // Fall back to searching in both branches.
                try
                {
                    return SBS_ExtractMemberExpression(conditionalExpression.IfFalse, originalExpression, counter);
                }
                catch (ArgumentException)
                {
                    return SBS_ExtractMemberExpression(conditionalExpression.IfTrue, originalExpression, counter);
                }
            }

            throw new ArgumentException($"Unable to cast expression to MemberExpression: {originalExpression}", nameof(originalExpression));
        }
    }
}

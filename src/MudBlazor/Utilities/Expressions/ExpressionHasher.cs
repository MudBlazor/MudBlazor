// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor.Utilities.Expressions
{
#nullable enable
    internal static class ExpressionHasher
    {
        private const int NullHashCode = 0x61E04917;

        public static int GetHashCode(Expression? expression)
        {
            if (expression is null)
                return NullHashCode;

            var visitor = new HashVisitor();

            visitor.Visit(expression);

            return visitor.ToHashCode();
        }

        private sealed class HashVisitor : ExpressionVisitor
        {
            private HashCode _hashCode;

            public HashVisitor()
            {
                _hashCode = new HashCode();
            }

            public int ToHashCode() => _hashCode.ToHashCode();

            private void UpdateHash(int value) => _hashCode.Add(value);

            private void UpdateHash(object? component)
            {
                if (component is MemberInfo member)
                {
                    _hashCode.Add(member.Name.GetHashCode());

                    var declaringType = member.DeclaringType;
                    if (declaringType?.AssemblyQualifiedName is not null)
                    {
                        _hashCode.Add(declaringType.AssemblyQualifiedName.GetHashCode());
                    }
                }
                else
                {
                    _hashCode.Add(component);
                }
            }

            [return: NotNullIfNotNull(nameof(node))]
            public override Expression? Visit(Expression? node)
            {
                if (node is null)
                    return base.Visit(node);

                UpdateHash((int)node.NodeType);
                return base.Visit(node);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                UpdateHash(node.Value);
                return base.VisitConstant(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                UpdateHash(node.Member);
                return base.VisitMember(node);
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                UpdateHash(node.Member);
                return base.VisitMemberAssignment(node);
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                UpdateHash((int)node.BindingType);
                UpdateHash(node.Member);
                return base.VisitMemberBinding(node);
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                UpdateHash((int)node.BindingType);
                UpdateHash(node.Member);
                return base.VisitMemberListBinding(node);
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                UpdateHash((int)node.BindingType);
                UpdateHash(node.Member);
                return base.VisitMemberMemberBinding(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                UpdateHash(node.Method);
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                UpdateHash(node.Constructor);
                return base.VisitNew(node);
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                UpdateHash(node.Type);
                return base.VisitNewArray(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                UpdateHash(node.Type);
                return base.VisitParameter(node);
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                UpdateHash(node.Type);
                UpdateHash(node.TypeOperand);
                return base.VisitTypeBinary(node);
            }
        }
    }
}

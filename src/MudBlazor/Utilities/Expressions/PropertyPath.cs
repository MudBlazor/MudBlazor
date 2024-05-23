// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor.Utilities.Expressions;

#nullable enable
internal static class PropertyPath
{
    public static PropertyHolder Visit<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
    {
        var body = expression.Body;
        var visitor = new PropertyVisitor(body is MemberExpression);
        visitor.Visit(body);

        return visitor.PropertyHolder;
    }

    public sealed class PropertyHolder
    {
        private readonly List<MemberInfo> _members;

        public bool IsBodyMemberExpression { get; }

        public PropertyHolder(bool isBodyMemberExpression)
        {
            IsBodyMemberExpression = isBodyMemberExpression;
            _members = new List<MemberInfo>();
        }

        public void AddMember(MemberInfo member) => _members.Insert(0, member);

        public IReadOnlyList<MemberInfo> GetMembers() => _members;

        public string GetPath() => string.Join(".", _members.Select(x => x.Name));

        public string GetLastMemberName()
        {
            var lastMemberName = _members
                .Select(x => x.Name)
                .LastOrDefault();

            return string.IsNullOrEmpty(lastMemberName) ? string.Empty : lastMemberName;
        }

        public override string ToString() => GetPath();
    }

    public sealed class PropertyVisitor : ExpressionVisitor
    {
        public PropertyHolder PropertyHolder { get; }

        public PropertyVisitor(bool isBodyMemberExpression)
        {
            PropertyHolder = new PropertyHolder(isBodyMemberExpression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            PropertyHolder.AddMember(node.Member);

            return base.VisitMember(node);
        }
    }
}

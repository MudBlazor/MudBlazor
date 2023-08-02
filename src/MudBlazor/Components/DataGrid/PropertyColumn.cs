﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities.Expressions;

namespace MudBlazor
{
#nullable enable
    /// <typeparam name="T">The type of data represented by each row in the data grid.</typeparam>
    /// <typeparam name="TProperty">The type of the value being displayed in the column's cells.</typeparam>
    public partial class PropertyColumn<T, TProperty> : Column<T>
    {
        private readonly Guid _id = Guid.NewGuid();

        private string? _propertyName;
        private Func<T, object?>? _cellContentFunc;
        private Func<T, TProperty>? _compiledPropertyFunc;
        private Expression<Func<T, TProperty>>? _lastAssignedProperty;
        private Expression<Func<T, TProperty>>? _compiledPropertyFuncFor;

        [Parameter]
        [EditorRequired]
        public Expression<Func<T, TProperty>> Property { get; set; } = Expression.Lambda<Func<T, TProperty>>(Expression.Default(typeof(TProperty)), Expression.Parameter(typeof(T)));

        [Parameter]
        public string? Format { get; set; }

        protected override void OnParametersSet()
        {
            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedProperty != Property)
            {
                _lastAssignedProperty = Property;
                var compiledPropertyExpression = Property.Compile();
                _cellContentFunc = item => compiledPropertyExpression(item);
            }

            var property = PropertyPath.Visit(Property);
            if (property.IsBodyMemberExpression)
            {
                _propertyName = property.GetPath();
            }
            else
            {
                // Most likely this is a dynamic expression that people use as workaround https://try.mudblazor.com/snippet/cYGxuTmhyqAQeCVM
                // We can't assign any meaningful name at all, therefore we should assign an unique ID like we do for TemplateColumn
                _propertyName = _id.ToString();
            }
            Title ??= property.GetLastMemberName();

            CompileGroupBy();
        }

        protected internal override LambdaExpression? PropertyExpression
            => Property;

        public override string? PropertyName
            => _propertyName;

        protected internal override string? ContentFormat
            => Format;

        protected internal override object? CellContent(T item)
            => _cellContentFunc!(item);

        protected internal override object? PropertyFunc(T item)
        {
            if (_compiledPropertyFunc == null || _compiledPropertyFuncFor != Property)
            {
                _compiledPropertyFunc = Property.Compile();
                _compiledPropertyFuncFor = Property;
            }

            return _compiledPropertyFunc(item);
        }

        protected internal override Type PropertyType
            => typeof(TProperty);

        protected internal override void SetProperty(object item, object value)
        {
            if (Property.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            {
                var actualType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? PropertyType;
                propertyInfo.SetValue(item, Convert.ChangeType(value, actualType), null);
            }
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable

    /// <typeparam name="T">The type of data represented by each row in the data grid.</typeparam>
    /// <typeparam name="TProperty">The type of the value being displayed in the column's cells.</typeparam>
    public partial class PropertyColumn<T, TProperty> : Column<T>
    {
        [Parameter] public Expression<Func<T, TProperty>> Property { get; set; } = default!;
        [Parameter] public string Format { get; set; }

        private Expression<Func<T, TProperty>>? _lastAssignedProperty;
        private Func<T, object?>? _cellContentFunc;
        private string? _propertyName;
        private string? _fullPropertyName;

        protected override void OnParametersSet()
        {
            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedProperty != Property)
            {
                _lastAssignedProperty = Property;
                var compiledPropertyExpression = Property.Compile();
                _cellContentFunc = item => compiledPropertyExpression!(item);
            }

            if (Property != null)
            {
                if (Property.Body is MemberExpression memberExpression)
                {
                    _fullPropertyName = Property.Body.ToString();
                    _propertyName = memberExpression.Member.Name;

                    if (Title is null)
                    {
                        Title = _propertyName;
                    }
                }
                else
                {
                    _propertyName = _fullPropertyName = Property.Body.ToString();
                }
            }

            CompileGroupBy();
        }

        protected internal override LambdaExpression? PropertyExpression
            => Property;

        public override string? PropertyName
            => _propertyName;

        protected internal override string ContentFormat
            => Format;

        protected internal override object? CellContent(T item)
            => _cellContentFunc!(item);

        protected internal override object? PropertyFunc(T item)
            => Property.Compile()(item);

        protected internal override Type PropertyType
            => typeof(TProperty);

        protected internal override string? FullPropertyName
            => _fullPropertyName;

        protected internal override void SetProperty(object item, object value)
        {
            var memberExpression = Property.Body as MemberExpression;

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (propertyInfo != null)
                {
                    var actualType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? PropertyType;
                    propertyInfo.SetValue(item, Convert.ChangeType(value, actualType), null);
                }
            }
        }


    }

#nullable disable
}

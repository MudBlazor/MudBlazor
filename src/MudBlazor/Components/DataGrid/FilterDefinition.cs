// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MudBlazor
{
    public class FilterDefinition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TProp>
        : FilterDefinition<T>
    {
        public new Expression<Func<T, TProp>> Field { get; }

        public FilterDefinition(Expression<Func<T, TProp>> field) : base(field.GetPropertyInfo().Name, typeof(TProp))
        {
            Field = field;
        }
    }

    public class FilterDefinition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : IField
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        private Type _fieldType;
        internal MudDataGrid<T> DataGrid { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
        //Keep old Field and FieldType for now for backward compatibility and make IField implementation explicit
        public string Field { get; set; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type FieldType
        {
            get
            {
                if (_fieldType is null)
                {
                    //throw for now
                    throw new ArgumentNullException(nameof(FieldType));
                }
                return _fieldType;
            }
            set
            {
                _fieldType = value;
            }
        }
        public string Title { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public Func<T, bool> FilterFunction { get; set; }

        string IField.Name => Field;
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type IField.Type => FieldType;

        public FilterDefinition(string filedName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fieldType)
        {
            Field = filedName;
            FieldType = fieldType;
        }

        public FilterDefinition()
        {
        }

        public Func<T, bool> GenerateFilterFunction()
        {
            if (FilterFunction is not null)
                return FilterFunction;

            var fieldType = MudBlazor.FieldType.Identify(this);
            // Handle case where we have an IDictionary.
            if (typeof(T) == typeof(IDictionary<string, object>))
            {
                var dictionaryExpressionGenerator = new FilterDictionaryExpressionGenerator<T>(this);

                return dictionaryExpressionGenerator.GenerateFilterFunction(fieldType);
            }

            var typeExpressionGenerator = new FilterTypeExpressionGenerator<T>(this);
            var expression = typeExpressionGenerator.GenerateFilterExpression(fieldType);

            return expression.Compile();
        }
    }
}

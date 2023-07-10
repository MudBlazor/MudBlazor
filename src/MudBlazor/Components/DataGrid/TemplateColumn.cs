// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    /// <typeparam name="T">The type of data represented by each row in the data grid.</typeparam>
    public partial class TemplateColumn<T> : Column<T>
    {
        private readonly string _propertyName = Guid.NewGuid().ToString();

        protected internal override object? CellContent(T item)
            => null;

        public override string? PropertyName
            => _propertyName;

        protected internal override object? PropertyFunc(T item)
            => null;

        protected internal override void SetProperty(object item, object value)
        {
        }
    }
}

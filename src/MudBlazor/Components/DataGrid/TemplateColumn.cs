// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents an additional column for a <see cref="MudDataGrid{T}"/> which isn't tied to data.
    /// </summary>
    /// <typeparam name="T">The type of data represented by this column.</typeparam>
    public partial class TemplateColumn<T> : Column<T>
    {
        protected internal override object? CellContent(T item)
            => null;

        /// <summary>
        /// The name of this column.
        /// </summary>
        public override string PropertyName { get; } = Guid.NewGuid().ToString();

        protected internal override object? PropertyFunc(T item)
            => null;

        protected internal override void SetProperty(object item, object value)
        {
        }
    }
}

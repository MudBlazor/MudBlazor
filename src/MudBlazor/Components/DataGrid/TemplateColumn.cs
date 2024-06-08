// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

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

        /// <summary>
        /// Allows filters to be used on this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When set, this overrides the <see cref="MudDataGrid{T}.Filterable"/> property.
        /// </remarks>
        [Parameter]
        public override bool? Filterable { get; set; } = false;

        /// <summary>
        /// Sorts values in this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When set, this overrides the <see cref="MudDataGrid{T}.SortMode"/> property.
        /// </remarks>
        [Parameter]
        public override bool? Sortable { get; set; } = false;

        /// <summary>
        /// Allows this column to be reordered via drag-and-drop operations.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When set, this overrides the <see cref="MudDataGrid{T}.DragDropColumnReordering"/> property.
        /// </remarks>
        [Parameter]
        public override bool? DragAndDropEnabled { get; set; } = false;

        /// <summary>
        /// Allows this column's width to be changed.
        /// </summary>
        [Parameter]
        public override bool? Resizable { get; set; } = false;

        /// <summary>
        /// Shows options for this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When set, this overrides the <see cref="MudDataGrid{T}.ShowColumnOptions"/> property.
        /// </remarks>
        [Parameter]
        public override bool? ShowColumnOptions { get; set; } = false;
    }
}

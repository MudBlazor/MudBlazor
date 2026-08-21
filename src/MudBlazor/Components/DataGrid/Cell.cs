// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MudBlazor.Utilities;

namespace MudBlazor
{
    internal class Cell<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly Column<T> _column;
        private readonly T _sourceItem;
        private CellContext<T>? _cellContext;
        private object? _computedValue;
        private bool _hasComputedValue;
        internal T _item;
        internal string? _valueString;
        internal double? _valueNumber;
        internal bool _editing;

        #region Computed Properties

        /// <summary>
        /// The cell's value.
        /// </summary>
        /// <remarks>
        /// <see cref="Column{T}.CellContent"/> invokes a compiled property expression and boxes value types, so the result is
        /// cached for the lifetime of this cell, which is a single render.
        /// </remarks>
        internal object? ComputedValue
        {
            get
            {
                if (!_hasComputedValue)
                {
                    _computedValue = _column.CellContent(_item);
                    _hasComputedValue = true;
                }

                return _computedValue;
            }
        }

        /// <summary>
        /// The state and actions passed to this cell's template.
        /// </summary>
        /// <remarks>
        /// Only templates read this, so it is built on demand rather than for every cell of every render.
        /// </remarks>
        internal CellContext<T> Context => _cellContext ??= new CellContext<T>(_dataGrid, _item, _sourceItem);

        internal string ComputedClass =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", _column.HideSmall)
                .AddClass("sticky-left", _column.StickyLeft)
                .AddClass("sticky-right", _column.StickyRight)
                .AddClass($"edit-mode-cell", (_dataGrid.EditMode == DataGridEditMode.Cell || (_dataGrid.EditMode == DataGridEditMode.Inline && _dataGrid.IsEditingItem(_item))) && _column.Editable)
                .AddClass(_column.CellClassFunc?.Invoke(_item))
                .AddClass(_column.CellClass)
                .Build();

        internal string ComputedStyle
        {
            get
            {
                var cellStyle = _column.CellStyleFunc?.Invoke(_item);
                // Most cells style nothing, and StyleBuilder would return an empty string anyway.
                if (cellStyle is null && _column.CellStyle is null)
                {
                    return string.Empty;
                }

                return new StyleBuilder()
                    .AddStyle(cellStyle)
                    .AddStyle(_column.CellStyle)
                    .Build();
            }
        }

        #endregion

        public Cell(MudDataGrid<T> dataGrid, Column<T> column, T item, T sourceItem)
        {
            _dataGrid = dataGrid;
            _column = column;
            _item = item;
            _sourceItem = sourceItem;

            OnStartedEditingItem();
        }

        public async Task StringValueChangedAsync(string? value)
        {
            // In cell edit mode, raise StartedEditingItem before the value is written so consumers can snapshot the pre-edit item, then commit the change immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.BeginCellEditAsync(_item);

            _column.SetProperty(_item, value);
            _hasComputedValue = false;

            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        public async Task NumberValueChangedAsync(double? value)
        {
            // In cell edit mode, raise StartedEditingItem before the value is written so consumers can snapshot the pre-edit item, then commit the change immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.BeginCellEditAsync(_item);

            _column.SetProperty(_item, value);
            _hasComputedValue = false;

            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        private void OnStartedEditingItem()
        {
            var computedValue = ComputedValue;
            if (computedValue is null)
            {
                return;
            }

            if (computedValue is JsonElement element)
            {
                if (_column.dataType == typeof(string))
                {
                    _valueString = element.GetString();
                }
                else if (_column.isNumber)
                {
                    _valueNumber = element.GetDouble();
                }
            }
            else
            {
                if (_column.dataType == typeof(string))
                {
                    _valueString = (string)computedValue;
                }
                else if (_column.isNumber)
                {
                    _valueNumber = Convert.ToDouble(computedValue);
                }
            }
        }
    }
}

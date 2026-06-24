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
        internal T _item;
        internal string? _valueString;
        internal double? _valueNumber;
        internal bool _editing;
        internal CellContext<T> _cellContext;

        #region Computed Properties

        internal object? ComputedValue
        {
            get
            {
                return _column.CellContent(_item);
            }
        }

        internal string ComputedClass =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", _column.HideSmall)
                .AddClass("sticky-left", _column.StickyLeft)
                .AddClass("sticky-right", _column.StickyRight)
                .AddClass($"edit-mode-cell", _dataGrid.EditMode == DataGridEditMode.Cell && _column.Editable)
                .AddClass(_column.CellClassFunc?.Invoke(_item))
                .AddClass(_column.CellClass)
                .Build();

        internal string ComputedStyle =>
            new StyleBuilder()
                .AddStyle(_column.CellStyleFunc?.Invoke(_item))
                .AddStyle(_column.CellStyle)
                .Build();

        #endregion

        public Cell(MudDataGrid<T> dataGrid, Column<T> column, T item)
        {
            _dataGrid = dataGrid;
            _column = column;
            _item = item;

            OnStartedEditingItem();

            // Create the CellContext
            _cellContext = new CellContext<T>(_dataGrid, _item);
        }

        public async Task StringValueChangedAsync(string? value)
        {
            // In cell edit mode, raise StartedEditingItem before the value is written so consumers can snapshot the pre-edit item, then commit the change immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.BeginCellEditAsync(_item);

            _column.SetProperty(_item, value);

            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        public async Task NumberValueChangedAsync(double? value)
        {
            // In cell edit mode, raise StartedEditingItem before the value is written so consumers can snapshot the pre-edit item, then commit the change immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.BeginCellEditAsync(_item);

            _column.SetProperty(_item, value);

            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        private void OnStartedEditingItem()
        {
            if (ComputedValue is null)
            {
                return;
            }

            if (ComputedValue is JsonElement element)
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
                    _valueString = (string)ComputedValue;
                }
                else if (_column.isNumber)
                {
                    _valueNumber = Convert.ToDouble(ComputedValue);
                }
            }
        }
    }
}

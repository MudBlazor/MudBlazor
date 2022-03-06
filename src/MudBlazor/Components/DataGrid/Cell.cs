// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor
{
    internal class Cell<T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly Column<T> _column;
        internal T _item;
        internal string valueString;
        internal double valueNumber;
        internal bool isEditing;
        internal CellContext<T> cellContext;

        #region Computed Properties

        internal object ComputedValue
        {
            get
            {
                if (_item == null || _column.Field == null)
                    return null;

                var property = _item.GetType().GetProperties().SingleOrDefault(x => x.Name == _column.Field);
                return property.GetValue(_item);
            }
        }
        internal string computedClass
        {
            get
            {
                return new CssBuilder(_column.CellClassFunc?.Invoke(_item))
                    .AddClass(_column.CellClass)
                    .AddClass("mud-table-cell")
                    .AddClass("mud-table-cell-hide", _column.HideSmall)
                    .AddClass($"edit-mode-cell", _dataGrid.EditMode == DataGridEditMode.Cell && _column.IsEditable)
                    .Build();
            }
        }
        internal string computedStyle
        {
            get
            {
                return new StyleBuilder()
                    .AddStyle(_column.CellStyleFunc?.Invoke(_item))
                    .AddStyle(_column.CellStyle)
                    .Build();
            }
        }

        #endregion

        public Cell(MudDataGrid<T> dataGrid, Column<T> column, T item)
        {
            _dataGrid = dataGrid;
            _column = column;
            _item = item;

            OnStartedEditingItem();

            // Create the CellContext
            cellContext = new CellContext<T>
            {
                selection = _dataGrid.Selection,
                Item = _item,
                Actions = new CellContext<T>.CellActions
                {
                    SetSelectedItem = async (x) => await _dataGrid.SetSelectedItemAsync(x, _item),
                    StartEditingItem = async () => await _dataGrid.SetEditingItemAsync(_item),
                    CancelEditingItem = async () => await _dataGrid.CancelEditingItemAsync(),
                }
            };
        }

        public async Task StringValueChangedAsync(string value)
        {
            var property = _item.GetType().GetProperties().SingleOrDefault(x => x.Name == _column.Field);
            property.SetValue(_item, value);

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        public async Task NumberValueChangedAsync(double value)
        {
            var property = _item.GetType().GetProperties().SingleOrDefault(x => x.Name == _column.Field);
            property.SetValue(_item, Convert.ChangeType(value, property.PropertyType));

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        private void OnStartedEditingItem()
        {

            if (ComputedValue != null)
            {
                if (_column.dataType == typeof(string))
                {
                    valueString = (string)ComputedValue;
                }
                else if (_column.isNumber)
                {
                    valueNumber = Convert.ToDouble(ComputedValue);
                }
            }
        }
    }
}

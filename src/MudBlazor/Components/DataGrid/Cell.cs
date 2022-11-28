// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor
{
    [RequiresUnreferencedCode(CodeMessage.SerializationUnreferencedCodeMessage)]
    internal class Cell<T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly Column<T> _column;
        internal T _item;
        internal string valueString;
        internal double? valueNumber;
        internal bool isEditing;
        internal CellContext<T> cellContext;

        #region Computed Properties

        internal object ComputedValue
        {
            get
            {
                if (_item == null || _column.Field == null)
                    return null;

                // Handle case where T is IDictionary.
                if (typeof(T) == typeof(IDictionary<string, object>))
                {
                    if (_column.FieldType == null)
                        throw new ArgumentNullException(nameof(_column.FieldType));

                    if (_column.FieldType.IsEnum)
                    {
                        var o = ((IDictionary<string, object>)_item)[_column.Field];

                        if (o == null)
                            return null;

                        if (o.GetType() == typeof(JsonElement))
                        {
                            var json = (JsonElement)o;
                            return Enum.ToObject(_column.FieldType, json.GetInt32());
                        }
                        else
                        {
                            return Enum.ToObject(_column.FieldType, o);
                        }
                    }

                    return ((IDictionary<string, object>)_item)[_column.Field];
                }

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
                    .AddClass("sticky-left", _column.StickyLeft)
                    .AddClass("sticky-right", _column.StickyRight)
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
            cellContext = new CellContext<T>(_dataGrid, _item);
        }

        public async Task StringValueChangedAsync(string value)
        {
            var property = _item.GetType().GetProperties().SingleOrDefault(x => x.Name == _column.Field);
            property.SetValue(_item, value);

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        public async Task NumberValueChangedAsync(double? value)
        {
            var property = _item.GetType().GetProperties().SingleOrDefault(x => x.Name == _column.Field);
            property.SetValue(_item, ChangeType(value, property.PropertyType));

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        private void OnStartedEditingItem()
        {

            if (ComputedValue != null)
            {
                if (ComputedValue.GetType() == typeof(JsonElement))
                {
                    if (_column.dataType == typeof(string))
                    {
                        valueString = ((JsonElement)ComputedValue).GetString();
                    }
                    else if (_column.isNumber)
                    {
                        valueNumber = ((JsonElement)ComputedValue).GetDouble();
                    }
                }
                else
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

        private object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}

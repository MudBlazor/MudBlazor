// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.DataGrid;

namespace MudBlazor
{
    public partial class Cell<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public T Value { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public RenderFragment<T> CellTemplate { get; set; }
        [Parameter] public RenderFragment<T> EditTemplate { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; }
        [Parameter] public bool IsEditable { get; set; }

        private bool _isSelected = false;
        private object _cachedValue;
        private string _valueString;
        private double _valueNumber;

        #region Computed Properties and Functions

        /// <summary>
        /// Evaluates the correct data to display in the cell.
        /// </summary>
        private object computedValue
        {
            get
            {
                if (Value == null || Field == null)
                    return null;

                var property = Value.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);
                return property.GetValue(Value);
            }
        }
        private string computedTitle
        {
            get
            {
                return Title ?? Field;
            }
        }
        private Type dataType
        {
            get
            {
                return typeof(T).GetProperty(Field).PropertyType;
            }
        }
        private bool isNumber
        {
            get
            {
                return FilterDefinition<T>.NumericTypes.Contains(dataType);
            }
        }
        private bool isEditing
        {
            get
            {
                return !DataGrid.ReadOnly && (IsEditable || ColumnType == ColumnType.InlineEditCommand) && Equals(DataGrid._editingItem, Value);
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            if (ColumnType != ColumnType.InlineEditCommand)
            {
                DataGrid.OnSelectedItemsChanged += OnSelectedItemsChanged;
                DataGrid.StartedEditingItem += OnStartedEditingItem;
                DataGrid.StartedCommittingItemChanges += OnStartedCommittingItemChanges;
                DataGrid.EditingCancelled += OnEditingCancelled;
            }
        }

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            _isSelected = items.Contains(Value);
        }

        private void OnStartedEditingItem()
        {
            if (isEditing)
            {
                _cachedValue = computedValue;

                if (computedValue != null)
                {
                    if (dataType == typeof(string))
                    {
                        _valueString = (string)computedValue;
                    }
                    else if (isNumber)
                    {
                        _valueNumber = Convert.ToDouble(computedValue);
                    }
                }
            }
        }

        private void OnEditingCancelled()
        {
            if (EditTemplate != null && _cachedValue != null)
            {
                // since we have an EditTemplate with possible bound values, we need to revert those here.
                var property = Value.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);
                property.SetValue(Value, _cachedValue);
            }
        }

        private void OnStartedCommittingItemChanges(T item)
        {
            if (IsEditable && EditTemplate == null)
            {
                if (ReferenceEquals(Value, item))
                {
                    var property = Value.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);

                    // commit the change at the cell.
                    if (dataType == typeof(string))
                    {
                        property.SetValue(Value, _valueString);
                    }
                    else if (isNumber)
                    {
                        property.SetValue(Value, Convert.ChangeType(_valueNumber, property.PropertyType));
                    }
                }
            }
        }

        #region Events

        private void CellCheckedChanged(bool value, T item)
        {
            _isSelected = value;
            DataGrid?.SetSelectedItem(value, item);
        }

        private void StringValueChanged(string value)
        {
            _valueString = value;
        }

        private void NumberValueChanged(double value)
        {
            _valueNumber = value;
        }

        private void Save()
        {
            DataGrid.CommitItemChanges(Value);
        }

        private void Cancel()
        {
            DataGrid.CancelEditingItem();
        }

        #endregion

        public void Dispose()
        {
            if (ColumnType != ColumnType.InlineEditCommand)
            {
                DataGrid.OnSelectedItemsChanged -= OnSelectedItemsChanged;
                DataGrid.StartedEditingItem -= OnStartedEditingItem;
                DataGrid.StartedCommittingItemChanges -= OnStartedCommittingItemChanges;
                DataGrid.EditingCancelled -= OnEditingCancelled;
            }
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class Cell<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public T Item { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public RenderFragment<T> CellTemplate { get; set; }
        [Parameter] public RenderFragment<T> EditTemplate { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; }
        [Parameter] public bool IsEditable { get; set; }
        [Parameter] public string CellClass { get; set; }
        [Parameter] public string CellStyle { get; set; }
        [Parameter] public Func<T, string> CellClassFunc { get; set; }
        [Parameter] public Func<T, string> CellStyleFunc { get; set; }

        private bool _isSelected = false;
        private object _cachedValue;
        private string _valueString;
        private double _valueNumber;
        private string _classname =>
            new CssBuilder(CellClass)
                .AddClass(Class)
            .Build();
        private string _style =>
            new StyleBuilder()
                .AddStyle(CellStyle)
                .AddStyle(Style)
            .Build();

        #region Computed Properties and Functions

        /// <summary>
        /// Evaluates the correct data to display in the cell.
        /// </summary>
        private object computedValue
        {
            get
            {
                if (Item == null || Field == null)
                    return null;

                var property = Item.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);
                return property.GetValue(Item);
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
                if (Field == null)
                    return typeof(object);

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
                if (DataGrid == null)
                    return false;

                return !DataGrid.ReadOnly && (IsEditable || ColumnType == ColumnType.InlineEditCommand) && Equals(DataGrid._editingItem, Item);
            }
        }
        private string classname
        {
            get
            {
                return new CssBuilder(CellClassFunc?.Invoke(Item))
                    .AddClass(_classname)
                    .Build();
            }
        }
        private string style
        {
            get
            {
                return new CssBuilder(CellStyleFunc?.Invoke(Item))
                    .AddClass(_style)
                    .Build();
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            if (DataGrid != null)
            {
                if (ColumnType != ColumnType.InlineEditCommand)
                {
                    DataGrid.SelectedItemsChangedEvent += OnSelectedItemsChanged;
                    DataGrid.StartedEditingItemEvent += OnStartedEditingItem;
                    DataGrid.StartedCommittingItemChangesEvent += OnStartedCommittingItemChanges;
                    DataGrid.EditingCancelledEvent += OnEditingCancelled;
                }

                DataGrid.PagerStateHasChangedEvent += OnPagerStateHasChanged;
            }
        }

        #region Events

        private void OnSelectedItemsChanged(HashSet<T> items)
        {
            _isSelected = items.Contains(Item);
            StateHasChanged();
        }

        private void OnPagerStateHasChanged()
        {
            // Here, we need to make sure that the item is selected if it is in the list of selected items.
            _isSelected = DataGrid.SelectedItems.Contains(Item);
            StateHasChanged();
        }

        private void OnStartedEditingItem()
        {
            if (IsEditable && isEditing)
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
            if (IsEditable && EditTemplate != null && _cachedValue != null)
            {
                // since we have an EditTemplate with possible bound values, we need to revert those here.
                var property = Item.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);
                property.SetValue(Item, _cachedValue);
            }
        }

        private void OnStartedCommittingItemChanges(T item)
        {
            _cachedValue = null;

            if (IsEditable && EditTemplate == null)
            {
                if (ReferenceEquals(Item, item))
                {
                    var property = Item.GetType().GetProperties().SingleOrDefault(x => x.Name == Field);

                    // commit the change at the cell.
                    if (dataType == typeof(string))
                    {
                        property.SetValue(Item, _valueString);
                    }
                    else if (isNumber)
                    {
                        property.SetValue(Item, Convert.ChangeType(_valueNumber, property.PropertyType));
                    }
                }
            }
        }

        internal async Task CellCheckedChangedAsync(bool value, T item)
        {
            _isSelected = value;
            await DataGrid?.SetSelectedItemAsync(value, item);
        }

        private void StringValueChanged(string value)
        {
            _valueString = value;
        }

        private void NumberValueChanged(double value)
        {
            _valueNumber = value;
        }

        private async Task Save()
        {
            await DataGrid?.CommitItemChangesAsync(Item);
        }

        private async Task CancelAsync()
        {
            await DataGrid?.CancelEditingItemAsync();
        }

        #endregion

        public void Dispose()
        {
            if (DataGrid != null)
            {
                if (ColumnType != ColumnType.InlineEditCommand)
                {
                    DataGrid.SelectedItemsChangedEvent -= OnSelectedItemsChanged;
                    DataGrid.StartedEditingItemEvent -= OnStartedEditingItem;
                    DataGrid.StartedCommittingItemChangesEvent -= OnStartedCommittingItemChanges;
                    DataGrid.EditingCancelledEvent -= OnEditingCancelled;
                }

                DataGrid.PagerStateHasChangedEvent -= OnPagerStateHasChanged;
            }
        }
    }
}

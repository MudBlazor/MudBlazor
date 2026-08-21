// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    internal class Filter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly IFilterDefinition<T> _filterDefinition;
        private readonly Column<T>? _column;

        internal string? _valueString;
        internal double? _valueNumber;
        internal Enum? _valueEnum;
        internal bool? _valueBool;
        internal DateTime? _valueDateOnlyForPicker;
        internal DateTime? _valueDateTimeForPicker;
        internal TimeSpan? _valueTime;
        internal Guid? _valueGuid;

        internal Column<T>? FilterColumn =>
            _column ?? (_dataGrid.RenderedColumns?.FirstOrDefault(c => c.PropertyName == _filterDefinition.Column?.PropertyName));

        // The filter editors live in a render fragment owned by MudDataGrid, so binding their ValueChanged straight to a handler makes the grid the callback's receiver, and Blazor then re-renders the whole grid - every header cell, every popover, every visible row - after each keystroke (#13639).
        // Routing them through this non-component receiver opts out of that automatic render; ApplyChangesAsync re-renders deliberately, and only when the edited filter is actually applied to the data.
        internal EventCallback<Column<T>> FieldChanged => EventCallback.Factory.Create<Column<T>>(this, FieldChangedAsync);
        internal EventCallback<string> OperatorChanged => EventCallback.Factory.Create<string>(this, OperatorChangedAsync);
        internal EventCallback<string> StringValueChanged => EventCallback.Factory.Create<string>(this, StringValueChangedAsync);
        internal EventCallback<double?> NumberValueChanged => EventCallback.Factory.Create<double?>(this, NumberValueChangedAsync);
        internal EventCallback<Enum> EnumValueChanged => EventCallback.Factory.Create<Enum>(this, EnumValueChangedAsync);
        internal EventCallback<bool?> BoolValueChanged => EventCallback.Factory.Create<bool?>(this, BoolValueChangedAsync);
        internal EventCallback<DateTime?> DateValueChanged => EventCallback.Factory.Create<DateTime?>(this, DateValueChangedAsync);
        internal EventCallback<TimeSpan?> TimeValueChanged => EventCallback.Factory.Create<TimeSpan?>(this, TimeValueChangedAsync);
        internal EventCallback<DateTime?> DateOnlyValueChanged => EventCallback.Factory.Create<DateTime?>(this, DateOnlyValueChangedAsync);
        internal EventCallback<Guid?> GuidValueChanged => EventCallback.Factory.Create<Guid?>(this, GuidValueChangedAsync);

        public Filter(MudDataGrid<T> dataGrid, IFilterDefinition<T> filterDefinition, Column<T>? column)
        {
            _dataGrid = dataGrid;
            _filterDefinition = filterDefinition;
            _column = column;

            var fieldType = _filterDefinition.FieldType;

            if (fieldType.IsString)
                _valueString = _filterDefinition.Value?.ToString();
            else if (fieldType.IsNumber)
                _valueNumber = _filterDefinition.Value == null ? null : Convert.ToDouble(_filterDefinition.Value);
            else if (fieldType.IsEnum)
                _valueEnum = (Enum?)_filterDefinition.Value;
            else if (fieldType.IsBoolean)
                _valueBool = _filterDefinition.Value == null ? null : Convert.ToBoolean(_filterDefinition.Value);
            else if (fieldType.IsDateTime)
            {
                var dateTime = Convert.ToDateTime(_filterDefinition.Value);
                _valueDateTimeForPicker = _filterDefinition.Value == null ? null : dateTime;
                _valueTime = _filterDefinition.Value == null ? null : dateTime.TimeOfDay;
            }
            else if (fieldType.IsDateOnly)
            {
                _valueDateOnlyForPicker = ((DateOnly?)_filterDefinition.Value)?.ToDateTime(TimeOnly.MinValue);
            }
            else if (fieldType.IsGuid)
                _valueGuid = _filterDefinition.Value as Guid?;
        }

        internal async Task RemoveFilterAsync()
        {
            await _dataGrid.RemoveFilterAsync(_filterDefinition.Id);
        }

        internal Task FieldChangedAsync(Column<T> column)
        {
            _filterDefinition.Column = column;
            var operators = column.GetFilterOperators(FieldType.Identify(column.PropertyType));
            _filterDefinition.Operator = operators.FirstOrDefault();
            _filterDefinition.Title = column.Title;
            _filterDefinition.Value = null;
            if (_filterDefinition is FilterDefinition<T> filterDefinition)
            {
                filterDefinition.FilterFunction = null;
            }
            return ApplyChangesAsync();
        }

        internal Task OperatorChangedAsync(string? value)
        {
            _filterDefinition.Operator = value;
            return ApplyChangesAsync();
        }

        internal Task StringValueChangedAsync(string value)
        {
            _valueString = value;
            _filterDefinition.Value = _valueString;
            return ApplyChangesAsync();
        }

        internal Task NumberValueChangedAsync(double? value)
        {
            _valueNumber = value;
            _filterDefinition.Value = _valueNumber;
            return ApplyChangesAsync();
        }

        internal Task EnumValueChangedAsync(Enum value)
        {
            _valueEnum = value;
            _filterDefinition.Value = _valueEnum;
            return ApplyChangesAsync();
        }

        internal Task BoolValueChangedAsync(bool? value)
        {
            _valueBool = value;
            _filterDefinition.Value = _valueBool;
            return ApplyChangesAsync();
        }

        internal Task DateValueChangedAsync(DateTime? value)
        {
            _valueDateTimeForPicker = value;

            if (value is not null)
            {
                var date = value.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime is not null)
                {
                    date = date.Add(_valueTime.Value);
                }

                _filterDefinition.Value = date;
            }
            else
                _filterDefinition.Value = null;

            return ApplyChangesAsync();
        }

        internal Task TimeValueChangedAsync(TimeSpan? value)
        {
            _valueTime = value;

            if (_valueDateTimeForPicker is not null)
            {
                var date = _valueDateTimeForPicker.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime is not null)
                {
                    date = date.Add(_valueTime.Value);
                }

                _filterDefinition.Value = date;
            }

            return ApplyChangesAsync();
        }

        internal Task DateOnlyValueChangedAsync(DateTime? value)
        {
            _valueDateOnlyForPicker = value;

            if (value is not null)
            {
                _filterDefinition.Value = DateOnly.FromDateTime(value.Value);
            }
            else
                _filterDefinition.Value = null;

            return ApplyChangesAsync();
        }

        internal Task GuidValueChangedAsync(Guid? value)
        {
            _valueGuid = value;
            _filterDefinition.Value = _valueGuid;
            return ApplyChangesAsync();
        }

        // Regroups the data after a filter edit and, in Simple mode, raises FilterChanged.
        // Simple mode applies filters live, so it notifies here; the row and menu modes notify from their own apply paths instead.
        private Task ApplyChangesAsync()
        {
            // The column filter menu edits a definition the grid has not applied yet, and only FilterDefinitions feeds the rows and the header's filtered icon, so its value cannot change anything on screen.
            // Regrouping and re-rendering every cell per keystroke there is pure waste, and it is what makes typing in the menu's value box lag on a large grid (#13639). The menu's Filter button applies the definition and refreshes the grid then.
            if (_dataGrid.FilterDefinitions.All(x => x.Id != _filterDefinition.Id))
            {
                return Task.CompletedTask;
            }

            _dataGrid.GroupItems();
            return _dataGrid.FilterMode == DataGridFilterMode.Simple
                ? _dataGrid.NotifyFilterChangedAsync()
                : Task.CompletedTask;
        }
    }
}

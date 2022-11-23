// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
    internal class Filter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly FilterDefinition<T> _filterDefinition;
        private readonly Column<T>? _column;

        internal string? _valueString;
        internal double? _valueNumber;
        internal Enum? _valueEnum;
        internal bool? _valueBool;
        internal DateTime? _valueDate;
        internal TimeSpan? _valueTime;

        internal Type dataType
        {
            get
            {
                if (_column is not null)
                    return _column.dataType;

                if (_filterDefinition.FieldType is not null)
                    return _filterDefinition.FieldType;

                if (_filterDefinition.Field is null)
                    return typeof(object);

                if (typeof(T) == typeof(IDictionary<string, object>) && _filterDefinition.FieldType is null)
                    throw new ArgumentNullException(nameof(_filterDefinition.FieldType));

                var type = typeof(T).GetProperty(_filterDefinition.Field)?.PropertyType!;
                return Nullable.GetUnderlyingType(type) ?? type;

            }
        }
        internal bool isNumber
        {
            get => FilterOperator.IsNumber(dataType);
        }
        internal bool isEnum
        {
            get => FilterOperator.IsEnum(dataType);
        }

        internal Column<T>? filterColumn => _column ?? _dataGrid.RenderedColumns?.FirstOrDefault(c => c.Field == _filterDefinition.Field);

        public Filter(MudDataGrid<T> dataGrid, FilterDefinition<T> filterDefinition)
            : this(dataGrid, filterDefinition, null)
        {
        }

        public Filter(MudDataGrid<T> dataGrid, FilterDefinition<T> filterDefinition, Column<T>? column)
        {
            _dataGrid = dataGrid;
            _filterDefinition = filterDefinition;
            _column = column;

            if (dataType == typeof(string))
                _valueString = _filterDefinition.Value?.ToString();
            else if (isNumber)
                _valueNumber = _filterDefinition.Value is null ? null : Convert.ToDouble(_filterDefinition.Value);
            else if (isEnum)
                _valueEnum = (Enum?)_filterDefinition.Value;
            else if (dataType == typeof(bool))
                _valueBool = _filterDefinition.Value is null ? null : Convert.ToBoolean(_filterDefinition.Value);
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                var dateTime = Convert.ToDateTime(_filterDefinition.Value);
                _valueDate = _filterDefinition.Value is null ? null : dateTime;
                _valueTime = _filterDefinition.Value is null ? null : dateTime.TimeOfDay;
            }
        }

        internal void RemoveFilter()
        {
            _dataGrid.RemoveFilter(_filterDefinition.Id);
        }

        internal void FieldChanged(string field)
        {
            var column = _dataGrid.RenderedColumns.FirstOrDefault(x => x.Field == field);
            if (column is not null)
            {
                _filterDefinition.Field = column.Field;
                _filterDefinition.FieldType = column.FieldType;
                var operators = FilterOperator.GetOperatorByDataType(dataType);
                _filterDefinition.Operator = operators.FirstOrDefault();
                _filterDefinition.Value = null;
            }
        }

        internal void StringValueChanged(string value)
        {
            _valueString = value;
            _filterDefinition.Value = _valueString;
            _dataGrid.GroupItems();
        }

        internal void NumberValueChanged(double? value)
        {
            _valueNumber = value;
            _filterDefinition.Value = _valueNumber;
            _dataGrid.GroupItems();
        }

        internal void EnumValueChanged(Enum value)
        {
            _valueEnum = value;
            _filterDefinition.Value = _valueEnum;
            _dataGrid.GroupItems();
        }

        internal void BoolValueChanged(bool? value)
        {
            _valueBool = value;
            _filterDefinition.Value = _valueBool;
            _dataGrid.GroupItems();
        }

        internal void DateValueChanged(DateTime? value)
        {
            _valueDate = value;

            if (value is not null)
            {
                var date = value.Value.Date;
                _filterDefinition.Value = date;
            }
            else
                _filterDefinition.Value = null;

            _dataGrid.GroupItems();
        }

        internal void TimeValueChanged(TimeSpan? value)
        {
            _valueTime = value;

            if (_valueDate is not null)
            {
                var date = _valueDate.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime is not null)
                {
                    date = date.Add(_valueTime.Value);
                }

                _filterDefinition.Value = date;
            }

            _dataGrid.GroupItems();
        }
    }
}

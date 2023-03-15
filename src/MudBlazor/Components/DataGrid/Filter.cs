// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace MudBlazor
{
    internal class Filter<T>
    {
        private MudDataGrid<T> _dataGrid;
        internal readonly FilterDefinition<T> _filterDefinition;
        private Column<T> _column;

        internal string _valueString;
        internal double? _valueNumber;
        internal Enum _valueEnum = null;
        internal bool? _valueBool;
        internal DateTime? _valueDate;
        internal TimeSpan? _valueTime;
        
        internal bool isNumber
        {
            get
            {
                return FilterOperator.IsNumber(_filterDefinition.dataType);
            }
        }
        internal bool isEnum
        {
            get
            {
                return FilterOperator.IsEnum(_filterDefinition.dataType);
            }
        }

        internal Column<T> filterColumn =>
            _column ?? (_dataGrid.RenderedColumns?.FirstOrDefault(c => c.PropertyName == _filterDefinition.Column.PropertyName));

        public Filter(MudDataGrid<T> dataGrid, FilterDefinition<T> filterDefinition, Column<T> column)
        {
            _dataGrid = dataGrid;
            _filterDefinition = filterDefinition;
            _column = column;

            if (_filterDefinition.dataType == typeof(string))
                _valueString = _filterDefinition.Value == null ? null : _filterDefinition.Value.ToString();
            else if (isNumber)
                _valueNumber = _filterDefinition.Value == null ? null : Convert.ToDouble(_filterDefinition.Value);
            else if (isEnum)
                _valueEnum = _filterDefinition.Value == null ? null : (Enum)_filterDefinition.Value;
            else if (_filterDefinition.dataType == typeof(bool))
                _valueBool = _filterDefinition.Value == null ? null : Convert.ToBoolean(_filterDefinition.Value);
            else if (_filterDefinition.dataType == typeof(DateTime) || _filterDefinition.dataType == typeof(DateTime?))
            {
                var dateTime = Convert.ToDateTime(_filterDefinition.Value);
                _valueDate = _filterDefinition.Value == null ? null : dateTime;
                _valueTime = _filterDefinition.Value == null ? null : dateTime.TimeOfDay;
            }
        }

        internal void RemoveFilter()
        {
            _dataGrid.RemoveFilter(_filterDefinition.Id);
        }

        internal void FieldChanged(Column<T> column)
        {
            _filterDefinition.Column = column;
            //_filterDefinition.Field = column.PropertyName;
            //_filterDefinition.FieldType = column.PropertyType;
            _filterDefinition.PropertyExpression = column.PropertyExpression;
            var operators = FilterOperator.GetOperatorByDataType(column.PropertyType);
            _filterDefinition.Operator = operators.FirstOrDefault();
            _filterDefinition.Value = null;
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

            if (value != null)
            {
                var date = value.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime != null)
                {
                    date = date.Add(_valueTime.Value);
                }

                _filterDefinition.Value = date;
            }
            else
                _filterDefinition.Value = null;

            _dataGrid.GroupItems();
        }

        internal void TimeValueChanged(TimeSpan? value)
        {
            _valueTime = value;

            if (_valueDate != null)
            {
                var date = _valueDate.Value.Date;


                // get the time component and add it to the date.
                if (_valueTime != null)
                {
                    date = date.Add(_valueTime.Value);
                }

                _filterDefinition.Value = date;
            }

            _dataGrid.GroupItems();
        }
    }
}

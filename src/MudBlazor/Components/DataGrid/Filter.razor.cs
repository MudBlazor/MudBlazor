// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class Filter<T>
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public Guid Id { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public Type FieldType { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Operator { get; set; }
        [Parameter] public object Value { get; set; }
        [Parameter] public Column<T> Column { get; set; }
        [Parameter] public EventCallback<string> FieldChanged { get; set; }
        [Parameter] public EventCallback<string> TitleChanged { get; set; }
        [Parameter] public EventCallback<string> OperatorChanged { get; set; }
        [Parameter] public EventCallback<object> ValueChanged { get; set; }

        private string __operator;
        private string _operator
        {
            get
            {
                return __operator;
            }
            set
            {
                __operator = value;
                Operator = __operator;
                OperatorChanged.InvokeAsync(Operator);
            }
        }
        private string _valueString;
        private double? _valueNumber;
        private Enum _valueEnum = null;
        private bool? _valueBool;
        private DateTime? _valueDate;
        private TimeSpan? _valueTime;

        #region Computed Properties and Functions

        private Type dataType
        {
            get
            {
                if (Column != null)
                    return Column.dataType;

                if (FieldType != null)
                    return FieldType;

                if (Field == null)
                    return typeof(object);

                if (typeof(T) == typeof(IDictionary<string, object>) && FieldType == null)
                    throw new ArgumentNullException(nameof(FieldType));

                var t = typeof(T).GetProperty(Field).PropertyType;
                return Nullable.GetUnderlyingType(t) ?? t;
            }
        }

        private bool isNumber
        {
            get
            {
                return FilterOperator.IsNumber(dataType);
            }
        }
        private bool isEnum
        {
            get
            {
                return FilterOperator.IsEnum(dataType);
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            __operator = Operator;

            if (DataGrid == null)
                DataGrid = Column?.DataGrid;

            if (dataType == typeof(string))
                _valueString = Value == null ? null : Value.ToString();
            else if (isNumber)
                _valueNumber = Value == null ? null : Convert.ToDouble(Value);
            else if (isEnum)
                _valueEnum = Value == null ? null : (Enum)Value;
            else if (dataType == typeof(bool))
                _valueBool = Value == null ? null : Convert.ToBoolean(Value);
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                var dateTime = Convert.ToDateTime(Value);
                _valueDate = Value == null ? null : dateTime;
                _valueTime = Value == null ? null : dateTime.TimeOfDay;
            }
        }

        internal async Task FieldChangedAsync(string field)
        {
            Field = field;
            var operators = FilterOperator.GetOperatorByDataType(dataType);
            Operator = operators.FirstOrDefault();
            Value = null;
            await OperatorChanged.InvokeAsync(Operator);
            await ValueChanged.InvokeAsync(Value);
            await FieldChanged.InvokeAsync(Field);
            Field = field;
        }

        internal void TitleChangedAsync(string title)
        {
            Title = title;
        }

        internal void StringValueChanged(string value)
        {
            _valueString = value;
            ValueChanged.InvokeAsync(value);
            DataGrid.GroupItems();
        }

        internal void NumberValueChanged(double? value)
        {
            _valueNumber = value;
            ValueChanged.InvokeAsync(value);
            DataGrid.GroupItems();
        }

        internal void EnumValueChanged(Enum value)
        {
            _valueEnum = value;
            ValueChanged.InvokeAsync(value);
            DataGrid.GroupItems();
        }

        internal void BoolValueChanged(bool? value)
        {
            _valueBool = value;
            ValueChanged.InvokeAsync(value);
            DataGrid.GroupItems();
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
                    date.Add(_valueTime.Value);
                }

                ValueChanged.InvokeAsync(date);
            }
            else
                ValueChanged.InvokeAsync(value);

            DataGrid.GroupItems();
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

                ValueChanged.InvokeAsync(date);
            }

            DataGrid.GroupItems();
        }

        internal void RemoveFilter()
        {
            DataGrid.RemoveFilter(Id);
        }
    }
}

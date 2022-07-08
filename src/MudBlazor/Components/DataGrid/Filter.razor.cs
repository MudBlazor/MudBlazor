// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class Filter<T>
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public Guid Id { get; set; }
        [Parameter] public string Field { get; set; }
        [Parameter] public string Operator { get; set; }
        [Parameter] public object Value { get; set; }
        [Parameter] public EventCallback<string> FieldChanged { get; set; }
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
        private string __field;
        private string _field
        {
            get
            {
                return __field;
            }
            set
            {
                __field = value;
                var operators = FilterOperator.GetOperatorByDataType(dataType);
                Operator = operators.FirstOrDefault();
                __operator = Operator;
                Value = null;
                OperatorChanged.InvokeAsync(Operator);
                ValueChanged.InvokeAsync(Value);
                FieldChanged.InvokeAsync(__field);
                StateHasChanged();
            }
        }
        private string _valueString;
        private double? _valueNumber;
        private Enum _valueEnum = null;
        private Enum _valueEnumFlags = null;
        private bool? _valueBool;
        private DateTime? _valueDate;
        private TimeSpan? _valueTime;

        #region Computed Properties and Functions

        private Type dataType
        {
            get
            {
                if (__field == null) return typeof(object);

                var t = typeof(T).GetProperty(__field).PropertyType;
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

        private bool IsEnumFlags
        {
            get
            {
                return FilterOperator.IsEnumFlags(dataType);
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            loadFilterUI();
        }

        protected override void OnParametersSet()
        {
            if (Field != null && __field != Field)
                loadFilterUI();
        }

        private void loadFilterUI()
        {
            __operator = Operator;
            __field = Field;

            if (dataType == typeof(string))
                _valueString = Value == null ? null : Value.ToString();
            else if (isNumber)
                _valueNumber = Value == null ? null : Convert.ToDouble(Value);
            else if (IsEnumFlags)
                _valueEnumFlags = Value == null ? null : (Enum)Value;
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

        internal void EnumFlagsValueChanged(IEnumerable<Enum> values)
        {
            var count = values.Count();
            if (count == 0)
                _valueEnumFlags = null;
            else
            {
                _valueEnumFlags = values.First();
                for (int i = 1; i < count; i++)
                    _valueEnumFlags = _valueEnumFlags.Or(values.ElementAt(i));
            }
            ValueChanged.InvokeAsync(_valueEnumFlags);
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

public static class Extensions
{
    public static Enum Or(this Enum a, Enum b)
    {
        // consider adding argument validation here
        if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
            return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) | Convert.ToInt64(b));
        else
            return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) | Convert.ToUInt64(b));
    }

    public static Enum And(this Enum a, Enum b)
    {
        // consider adding argument validation here
        if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
            return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) & Convert.ToInt64(b));
        else
            return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) & Convert.ToUInt64(b));
    }
    public static Enum Not(this Enum a)
    {
        // consider adding argument validation here
        return (Enum)Enum.ToObject(a.GetType(), ~Convert.ToInt64(a));
    }
}

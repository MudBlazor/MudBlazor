// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
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
        private string _valueString;
        private double _valueNumber;

        #region Computed Properties and Functions

        private Type dataType
        {
            get
            {
                return Field == null ? typeof(object) : typeof(T).GetProperty(Field).PropertyType;
            }
        }

        private bool isNumber
        {
            get
            {
                return FilterDefinition<T>.NumericTypes.Contains(dataType);
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            __operator = Operator;

            if (dataType == typeof(string))
                _valueString = Value == null ? null : Value.ToString();

            if (isNumber && Value != null)
                _valueNumber = Value == null ? 0 : Convert.ToDouble(Value);
        }

        internal void StringValueChanged(string value)
        {
            _valueString = value;
            ValueChanged.InvokeAsync(value);
        }

        internal void NumberValueChanged(double value)
        {
            _valueNumber = value;
            ValueChanged.InvokeAsync(value);
        }

        internal void RemoveFilter()
        {
            DataGrid.RemoveFilter(Id);
        }
    }
}

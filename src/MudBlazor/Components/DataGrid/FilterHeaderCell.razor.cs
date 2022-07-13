// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class FilterHeaderCell<T> : MudComponentBase
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public Column<T> Column { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private string _classname =>
            new CssBuilder(Column?.HeaderClass)
                .AddClass(Column?.headerClassname)
                .AddClass(Class)
                .AddClass("filter-header-cell")
            .Build();

        private string _style =>
            new StyleBuilder()
                .AddStyle(Column?.HeaderStyle)
                .AddStyle(Style)
            .Build();

        private string _valueString;
        private double? _valueNumber;
        private Enum _valueEnum = null;
        private bool? _valueBool;
        private DateTime? _valueDate;
        private TimeSpan? _valueTime;

        #region Computed Properties and Functions

        private bool hasFilter
        {
            get
            {
                if (DataGrid == null)
                    return false;

                return DataGrid.FilterDefinitions.Any(x => x.Field == Column.Field && x.Operator != null && x.Value != null);
            }
        }

        private Type dataType
        {
            get
            {
                return Column?.dataType;
            }
        }

        private string[] operators
        {
            get
            {
                return FilterOperator.GetOperatorByDataType(dataType);
            }
        }

        private string _operator;

        private string chosenOperatorStyle(string o)
        {
            return o == _operator ? "color:var(--mud-palette-primary-text);background-color:var(--mud-palette-primary)" : "";
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
            _operator = operators.FirstOrDefault();
        }

        #region Events

        private void ChangeOperator(string o)
        {
            _operator = o;
            Column.filterContext.FilterDefinition.Operator = _operator;
            ApplyFilter(Column.filterContext.FilterDefinition);
        }

        internal void StringValueChanged(string value)
        {
            _valueString = value;
            Column.filterContext.FilterDefinition.Operator = _operator;
            Column.filterContext.FilterDefinition.Value = value;
            ApplyFilter(Column.filterContext.FilterDefinition);
        }

        internal void NumberValueChanged(double? value)
        {
            _valueNumber = value;
            Column.filterContext.FilterDefinition.Operator = _operator;
            Column.filterContext.FilterDefinition.Value = value;
            ApplyFilter(Column.filterContext.FilterDefinition);
        }

        internal void EnumValueChanged(Enum value)
        {
            _valueEnum = value;
            Column.filterContext.FilterDefinition.Operator = _operator;
            Column.filterContext.FilterDefinition.Value = value;
            ApplyFilter(Column.filterContext.FilterDefinition);
        }

        internal void BoolValueChanged(bool? value)
        {
            _valueBool = value;
            Column.filterContext.FilterDefinition.Operator = _operator;
            Column.filterContext.FilterDefinition.Value = value;
            ApplyFilter(Column.filterContext.FilterDefinition);
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

                Column.filterContext.FilterDefinition.Operator = _operator;
                Column.filterContext.FilterDefinition.Value = date;
                ApplyFilter(Column.filterContext.FilterDefinition);
            }
            else
            {
                Column.filterContext.FilterDefinition.Operator = _operator;
                Column.filterContext.FilterDefinition.Value = value;
                ApplyFilter(Column.filterContext.FilterDefinition);
            }
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

                Column.filterContext.FilterDefinition.Operator = _operator;
                Column.filterContext.FilterDefinition.Value = date;
                ApplyFilter(Column.filterContext.FilterDefinition);
            }
        }

        internal void ApplyFilter(FilterDefinition<T> filterDefinition)
        {
            if (!DataGrid.FilterDefinitions.Any(x => x.Id == filterDefinition.Id))
                DataGrid.FilterDefinitions.Add(filterDefinition);

            DataGrid.GroupItems();
            DataGrid.ExternalStateHasChanged();
        }

        private void ClearFilter()
        {
            ClearFilter(Column.filterContext.FilterDefinition);

            if (dataType == typeof(string))
                _valueString = null;
            else if (isNumber)
                _valueNumber = null;
            else if (isEnum)
                _valueEnum = null;
            else if (dataType == typeof(bool))
                _valueBool = null;
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                _valueDate = null;
                _valueTime = null;
            }
        }

        internal void ClearFilter(FilterDefinition<T> filterDefinition)
        {
            DataGrid.RemoveFilter(filterDefinition.Id);
        }

        #endregion
    }
}

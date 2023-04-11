// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
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

                return false;
                //return DataGrid.FilterDefinitions.Any(x => x.Field == Column.Field && x.Operator != null && x.Value != null);
            }
        }

        private Type dataType
        {
            get
            {
                return Column?.PropertyType;
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
                return TypeIdentifier.IsNumber(dataType);
            }
        }

        private bool isEnum
        {
            get
            {
                return TypeIdentifier.IsEnum(dataType);
            }
        }

        #endregion

        protected override void OnInitialized()
        {
            _operator = operators.FirstOrDefault();
        }

        #region Events

        private async Task ChangeOperatorAsync(string o)
        {
            _operator = o;
            Column.FilterContext.FilterDefinition.Operator = _operator;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task StringValueChangedAsync(string value)
        {
            _valueString = value;
            Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task NumberValueChangedAsync(double? value)
        {
            _valueNumber = value;
            Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task EnumValueChangedAsync(Enum value)
        {
            _valueEnum = value;
            Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task BoolValueChangedAsync(bool? value)
        {
            _valueBool = value;
            Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task DateValueChangedAsync(DateTime? value)
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

                Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = date;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
            else
            {
                Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = value;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
        }

        internal async Task TimeValueChangedAsync(TimeSpan? value)
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

                Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = date;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
        }

        internal async Task ApplyFilterAsync(FilterDefinition<T> filterDefinition)
        {
            if (!DataGrid.FilterDefinitions.Any(x => x.Id == filterDefinition.Id))
                DataGrid.FilterDefinitions.Add(filterDefinition);
            if (DataGrid.ServerData is not null) await DataGrid.ReloadServerData();

            DataGrid.GroupItems();
            ((IMudStateHasChanged)DataGrid).StateHasChanged();
        }

        private async Task ClearFilterAsync()
        {
            await ClearFilterAsync(Column.FilterContext.FilterDefinition);

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

        internal async Task ClearFilterAsync(FilterDefinition<T> filterDefinition)
        {
            await DataGrid.RemoveFilterAsync(filterDefinition.Id);
        }

        #endregion
    }
}

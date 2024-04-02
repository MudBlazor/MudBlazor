// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
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

        #region Computed Properties and Functions

        private Type dataType
        {
            get
            {
                return Column?.PropertyType;
            }
        }

        private FieldType fieldType => FieldType.Identify(dataType);

        private string[] operators
        {
            get
            {
                return FilterOperator.GetOperatorByDataType(dataType);
            }
        }

        private string valueString => fieldType.IsString && Column.FilterContext.FilterDefinition.Value is not null ? (string)Column.FilterContext.FilterDefinition.Value : default;
        private double? valueNumber => fieldType.IsNumber ? (double?)Column.FilterContext.FilterDefinition.Value : default;
        private bool? valueBool => fieldType.IsBoolean && Column.FilterContext.FilterDefinition.Value is not null ? (bool?)Column.FilterContext.FilterDefinition.Value : default;
        private Enum valueEnum => fieldType.IsEnum && Column.FilterContext.FilterDefinition.Value is not null ? (Enum)Column.FilterContext.FilterDefinition.Value : default;
        private DateTime? valueDate => fieldType.IsDateTime ? (DateTime?)Column.FilterContext.FilterDefinition.Value : default;
        private TimeSpan? valueTime => fieldType.IsDateTime && Column.FilterContext.FilterDefinition.Value is not null ? ((DateTime?)Column.FilterContext.FilterDefinition.Value).Value.TimeOfDay : null;
        private string @operator => Column.FilterContext.FilterDefinition.Operator ?? operators.FirstOrDefault();

        public FilterHeaderCell()
        {
            // Update _operator if underlying column had changed position
            //_columnState = RegisterParameter(
            //    nameof(Column),
            //    () => Column,
            //    IsColumnChangedHandlerAsync
            //);
        }

        //private async Task IsColumnChangedHandlerAsync(ParameterChangedEventArgs<Column<T>> args)
        //{
        //    if (Column.FilterContext.FilterDefinition.Operator != _operator)
        //    {
        //        _operator = Column.FilterContext.FilterDefinition.Operator;
        //        await _columnState.SetValueAsync(args.Value);
        //        StateHasChanged();
        //    }
        //}

        private string chosenOperatorStyle(string o)
        {
            return o == @operator ? "color:var(--mud-palette-primary-text);background-color:var(--mud-palette-primary)" : "";
        }

        //private bool isNumber
        //{
        //    get
        //    {
        //        return TypeIdentifier.IsNumber(dataType);
        //    }
        //}

        //private bool isEnum
        //{
        //    get
        //    {
        //        return TypeIdentifier.IsEnum(dataType);
        //    }
        //}

        #endregion

        //protected override void OnInitialized()
        //{
        //    _operator = operators.FirstOrDefault();
        //}

        #region Events

        private async Task ChangeOperatorAsync(string o)
        {
            //_operator = o;
            Column.FilterContext.FilterDefinition.Operator = o;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task StringValueChangedAsync(string value)
        {
            //_valueString = value;
            //Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task NumberValueChangedAsync(double? value)
        {
            //_valueNumber = value;
            //Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task EnumValueChangedAsync(Enum value)
        {
            //_valueEnum = value;
            //Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task BoolValueChangedAsync(bool? value)
        {
            //_valueBool = value;
            //Column.FilterContext.FilterDefinition.Operator = _operator;
            Column.FilterContext.FilterDefinition.Value = value;
            await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
        }

        internal async Task DateValueChangedAsync(DateTime? value)
        {
            //_valueDate = value;

            if (value != null)
            {
                var date = value.Value.Date;

                // get the time component and add it to the date.
                if (valueTime != null)
                {
                    date.Add(valueTime.Value);
                }

                //Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = date;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
            else
            {
                //Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = value;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
        }

        internal async Task TimeValueChangedAsync(TimeSpan? value)
        {
            //_valueTime = value;

            if (valueDate != null)
            {
                var date = valueDate.Value.Date;

                // get the time component and add it to the date.
                if (valueTime != null)
                {
                    date = date.Add(valueTime.Value);
                }

                //Column.FilterContext.FilterDefinition.Operator = _operator;
                Column.FilterContext.FilterDefinition.Value = date;
                await ApplyFilterAsync(Column.FilterContext.FilterDefinition);
            }
        }

        internal async Task ApplyFilterAsync(IFilterDefinition<T> filterDefinition)
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

            //if (dataType == typeof(string))
            //_valueString = null;
            //if (isNumber)
            //    _valueNumber = null;
            //else if (isEnum)
            //    _valueEnum = null;
            //else if (dataType == typeof(bool))
            //    _valueBool = null;
            //else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            //{
            //    _valueDate = null;
            //    _valueTime = null;
            //}
        }

        internal async Task ClearFilterAsync(IFilterDefinition<T> filterDefinition)
        {
            await DataGrid.RemoveFilterAsync(filterDefinition.Id);
        }

        #endregion
    }
}

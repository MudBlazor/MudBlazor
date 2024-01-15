// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Components.DataGrid
{
#nullable enable   
    public class ColumnState<T>
    {
        private readonly Column<T> _column;
        private readonly StateHandler<bool> _hidden;

        internal ColumnState(Column<T> column) 
        {
            _column = column;
            _hidden = StateHandler.Create(column.Hidden, column.HiddenChanged);
        }

        /// <summary>
        /// Update state where external parameters have changed. Usually called in OnParametersSetAsync.
        /// </summary>
        internal void SyncParameters()
        {
            _hidden.SyncParameter(_column.Hidden);
        }

        public bool Hidden { get => _hidden.Value; set => _hidden.Value = value; }
    }
}

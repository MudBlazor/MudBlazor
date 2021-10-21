// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class FooterCell<T> : MudComponentBase
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }
        [CascadingParameter(Name = "IsOnlyFooter")] public bool IsOnlyFooter { get; set; } = false;

        [Parameter] public int ColSpan { get; set; }
        [Parameter] public ColumnType ColumnType { get; set; } = ColumnType.Text;
        [Parameter] public RenderFragment FooterTemplate { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private void CheckedChanged(bool value)
        {
            DataGrid.SetSelectAll(value);
        }
    }
}

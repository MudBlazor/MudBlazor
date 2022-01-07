// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public class DataGridRowClickEventArgs<T> : EventArgs
    {
        public MouseEventArgs MouseEventArgs { get; set; }
        public T Item { get; set; }
        public int RowIndex { get; set; }

    }
}

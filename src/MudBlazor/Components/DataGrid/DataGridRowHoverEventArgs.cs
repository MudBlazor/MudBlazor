// Copyright (c) MudBlazor 2024
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public class DataGridRowHoverEventArgs<T> : EventArgs
    {
        public MouseEventArgs MouseEventArgs { get; }

        public T Item { get; }

        public int RowIndex { get; }

        public DataGridRowHoverEventArgs(MouseEventArgs mouseEventArgs, T item, int rowIndex)
        {
            MouseEventArgs = mouseEventArgs;
            Item = item;
            RowIndex = rowIndex;
        }
    }
}

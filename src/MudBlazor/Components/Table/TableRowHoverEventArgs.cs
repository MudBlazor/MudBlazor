using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public class TableRowHoverEventArgs<T> : EventArgs
    {
        public MouseEventArgs MouseEventArgs { get; }

        public MudTr Row { get; }

        public T? Item { get; }

        public TableRowHoverEventArgs(MouseEventArgs mouseEventArgs, MudTr row, T? item)
        {
            MouseEventArgs = mouseEventArgs;
            Row = row;
            Item = item;
        }
    }
}

using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public class TableRowClickEventArgs<T> : EventArgs
    {
        public MouseEventArgs MouseEventArgs { get; }

        public MudTr Row { get; }

        public T? Item { get; }

        public TableRowClickEventArgs(MouseEventArgs mouseEventArgs, MudTr row, T? item)
        {
            MouseEventArgs = mouseEventArgs;
            Row = row;
            Item = item;
        }
    }
}

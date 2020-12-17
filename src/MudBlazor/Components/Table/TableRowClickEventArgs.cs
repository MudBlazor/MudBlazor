using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Components.Table
{
    public class TableRowClickEventArgs<T> : EventArgs
    {
        public MouseEventArgs MouseEventArgs { get; set; }
        public T Item { get; set; }
        public MudTr Row { get; set; }
    }
}

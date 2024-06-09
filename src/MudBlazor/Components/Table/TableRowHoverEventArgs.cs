using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable
public class TableRowHoverEventArgs<T> : EventArgs
{
    public PointerEventArgs PointerEventArgs { get; }

    public MudTr Row { get; }

    public T? Item { get; }

    public TableRowHoverEventArgs(PointerEventArgs pointerEventArgs, MudTr row, T? item)
    {
        PointerEventArgs = pointerEventArgs;
        Row = row;
        Item = item;
    }
}

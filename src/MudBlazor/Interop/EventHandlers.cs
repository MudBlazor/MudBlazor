using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// see https://github.com/dotnet/aspnetcore/issues/13104
    /// </summary>
    /// 

    //due to these event handlers are not implemented in Blazor, we can't pass MouseEventArgs,
    //because that produces an error of casting, not being possible to convert EventArgs into MouseEventArgs.
    //Change this when they are implemented natively in Blazor
    [EventHandler("onmouseenter", typeof(EventArgs), true, true)]
    [EventHandler("onmouseleave", typeof(EventArgs), true, true)]
    public static class EventHandlers
    {

    }
}

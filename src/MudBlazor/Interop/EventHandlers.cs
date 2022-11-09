using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    // used in MudCollapse
    [EventHandler("onanimationend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
#if !NET7_0_OR_GREATER
    // see https://github.com/dotnet/aspnetcore/issues/13104
    // due to these event handlers are not implemented in Blazor before net7, we can't pass MouseEventArgs,
    // because that produces an error of casting, not being possible to convert EventArgs into MouseEventArgs.
    [EventHandler("onmouseenter", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
    [EventHandler("onmouseleave", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
#endif
    public static class EventHandlers
    {

    }

}

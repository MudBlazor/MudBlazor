using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    // used in MudCollapse
    [EventHandler("onanimationend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
#if !NET7_0_OR_GREATER
    [EventHandler("onmouseenter", typeof(MouseEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
    [EventHandler("onmouseleave", typeof(MouseEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
#endif
    public static class EventHandlers
    {

    }

}

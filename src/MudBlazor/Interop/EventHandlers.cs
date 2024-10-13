using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

// used in MudCollapse
[EventHandler("onanimationend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
public static class EventHandlers;

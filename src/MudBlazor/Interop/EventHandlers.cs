using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
// used in MudCollapse
[EventHandler("ontransitionend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
public static class EventHandlers;

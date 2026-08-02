using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

// used in MudCollapse
/// <summary>
/// Registers the <c>ontransitionend</c> DOM event with Blazor's event system.
/// </summary>
[EventHandler("ontransitionend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
public static class EventHandlers;

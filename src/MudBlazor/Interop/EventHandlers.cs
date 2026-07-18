using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

// used in MudCollapse
/// <summary>
/// Registers the <c>ontransitionend</c> DOM event with Blazor's event system so components such as <see cref="MudCollapse"/> can respond when a CSS transition finishes.
/// </summary>
[EventHandler("ontransitionend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: false)]
public static class EventHandlers;

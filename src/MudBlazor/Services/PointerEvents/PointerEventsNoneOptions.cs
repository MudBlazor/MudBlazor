// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Options for configuring the pointer-events-none interop service.
/// </summary>
/// <remarks>
/// Use these options when you need to disable pointer events on elements while still tracking pointer up/down notifications for cleanup or state transitions.
/// </remarks>
public class PointerEventsNoneOptions
{
    /// <summary>
    /// Output event and debug information to the browser's console.
    /// </summary>
    public bool EnableLogging { get; init; }

    /// <summary>
    /// Subscribe to pointer down events.
    /// </summary>
    public bool SubscribeDown { get; init; }

    /// <summary>
    /// Subscribe to pointer up events.
    /// </summary>
    public bool SubscribeUp { get; init; }
}

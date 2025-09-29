// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Represents options for <see cref="IPointerEventsNoneService"/>.
/// </summary>
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

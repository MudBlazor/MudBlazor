// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Defines the contract for a select component that manages selection items.
/// </summary>
internal interface IMudSelect
{
    /// <summary>
    /// Gets the context that manages communication between the select and its items.
    /// </summary>
    /// <remarks>
    /// Items use this context to:
    /// <list type="bullet">
    /// <item>Register and unregister themselves</item>
    /// <item>Observe selection state changes</item>
    /// <item>Query current selection state</item>
    /// </list>
    /// </remarks>
    object SelectContext { get; }
}

/// <summary>
/// Marker interface for shadow select items.
/// </summary>
/// <remarks>
/// Shadow items are used for value-to-RenderFragment lookups for items
/// that are not visible in the dropdown (HideContent=true).
/// </remarks>
internal interface IMudShadowSelect
{
    /// <summary>
    /// Gets the context that manages shadow item registration.
    /// </summary>
    object SelectContext { get; }
}

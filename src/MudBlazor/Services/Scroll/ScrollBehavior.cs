// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies the scroll behavior for scrolling operations.
/// </summary>
public enum ScrollBehavior
{
    /// <summary>
    /// Scrolls in a smooth fashion.
    /// </summary>
    [Description("smooth")]
    Smooth,

    /// <summary>
    /// Scrolls immediately.
    /// </summary>
    [Description("auto")]
    Auto
}

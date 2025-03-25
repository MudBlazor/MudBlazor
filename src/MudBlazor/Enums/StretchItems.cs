// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies how items in a flex container are stretched along the main axis.
/// </summary>
public enum StretchItems
{
    /// <summary>
    /// No stretching is applied.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// The first item is stretched.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// The last item is stretched.
    /// </summary>
    [Description("end")]
    End,

    /// <summary>
    /// The first and last items are stretched.
    /// </summary>
    [Description("start-and-end")]
    StartAndEnd,

    /// <summary>
    /// All items except for the first and last are stretched.
    /// </summary>
    [Description("middle")]
    Middle,

    /// <summary>
    /// All items are stretched evenly.
    /// </summary>
    [Description("all")]
    All,
}

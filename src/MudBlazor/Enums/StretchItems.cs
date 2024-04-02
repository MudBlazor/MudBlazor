// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies how children of a flex container are stretched along the main axis.
/// </summary>
public enum StretchItems
{
    /// <summary>
    /// No stretching is applied to children.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// The first child is stretched.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// The last child is stretched.
    /// </summary>
    [Description("end")]
    End,

    /// <summary>
    /// The first and last children are stretched.
    /// </summary>
    [Description("start-and-end")]
    StartAndEnd,

    /// <summary>
    /// All children except for the first and last are stretched.
    /// </summary>
    [Description("middle")]
    Middle,

    /// <summary>
    /// All children are stretched.
    /// </summary>
    [Description("all")]
    All,
}

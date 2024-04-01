// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies how children of a flex container are stretched along the main axis.
/// </summary>
public enum StretchChildren
{
    /// <summary>
    /// No stretching is applied to children.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// The first child is stretched.
    /// </summary>
    [Description("first-child")]
    FirstChild,

    /// <summary>
    /// The last child is stretched.
    /// </summary>
    [Description("last-child")]
    LastChild,

    /// <summary>
    /// All children except for the first and last are stretched.
    /// </summary>
    [Description("middle-children")]
    MiddleChildren,

    /// <summary>
    /// All children are stretched.
    /// </summary>
    [Description("all-children")]
    AllChildren,
}

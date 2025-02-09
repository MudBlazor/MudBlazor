// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the position of an object relative to its container.
/// </summary>
public enum ObjectPosition
{
    /// <summary>
    /// The object is centered vertically and horizontally.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// The top of the object is aligned with the top of its container.
    /// </summary>
    [Description("top")]
    Top,

    /// <summary>
    /// The bottom of the object is aligned with the bottom of its container.
    /// </summary>
    [Description("bottom")]
    Bottom,

    /// <summary>
    /// The left of the object is aligned with the left of its container.
    /// </summary>
    [Description("left")]
    Left,

    /// <summary>
    /// The left and top of the object is aligned with the left and top of its container.
    /// </summary>
    [Description("left-top")]
    LeftTop,

    /// <summary>
    /// The left and bottom of the object is aligned with the left and bottom of its container.
    /// </summary>
    [Description("left-bottom")]
    LeftBottom,

    /// <summary>
    /// The right of the object is aligned with the right of its container.
    /// </summary>
    [Description("right")]
    Right,

    /// <summary>
    /// The right and top of the object is aligned with the right and top of its container.
    /// </summary>
    [Description("right-top")]
    RightTop,

    /// <summary>
    /// The right and bottom of the object is aligned with the right and bottom of its container.
    /// </summary>
    [Description("right-bottom")]
    RightBottom,
}

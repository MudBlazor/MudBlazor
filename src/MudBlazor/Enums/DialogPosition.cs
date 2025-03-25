// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The location of a <see cref="MudDialog"/> when it is opened.
/// </summary>
public enum DialogPosition
{
    /// <summary>
    /// The dialog will appear in the center of the screen.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// The dialog will appear on the left side of the screen, centered vertically.
    /// </summary>
    [Description("centerleft")]
    CenterLeft,

    /// <summary>
    /// The dialog will appear on the left side of the screen, centered vertically.
    /// </summary>
    [Description("centerright")]
    CenterRight,

    /// <summary>
    /// The dialog will appear on the top of the screen, centered horizontally.
    /// </summary>
    [Description("topcenter")]
    TopCenter,

    /// <summary>
    /// The dialog will appear on the upper-left corner of the screen.
    /// </summary>
    [Description("topleft")]
    TopLeft,

    /// <summary>
    /// The dialog will appear on the upper-right corner of the screen.
    /// </summary>
    [Description("topright")]
    TopRight,

    /// <summary>
    /// The dialog will appear on the bottom of the screen, centered horizontally.
    /// </summary>
    [Description("bottomcenter")]
    BottomCenter,

    /// <summary>
    /// The dialog will appear on the lower-left corner of the screen.
    /// </summary>
    [Description("bottomleft")]
    BottomLeft,

    /// <summary>
    /// The dialog will appear on the lower-right corner of the screen.
    /// </summary>
    [Description("bottomright")]
    BottomRight,

    /// <summary>
    /// The dialog will appear at a custom position.
    /// </summary>
    [Description("custom")]
    Custom
}

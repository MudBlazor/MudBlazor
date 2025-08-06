// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

public enum LinkTarget
{
    /// <summary>
    /// Opens the linked document in the same tab as it was clicked
    /// </summary>
    [Description("_self")]
    Self,

    /// <summary>
    /// Opens the linked document in the full body of the window
    /// </summary>
    [Description("_top")]
    Top,

    /// <summary>
    /// Opens the linked document in the parent frame
    /// </summary>
    [Description("_parent")]
    Parent,

    /// <summary>
    /// Opens the linked document in a new window or tab
    /// </summary>
    [Description("_blank")]
    Blank,

    /// <summary>
    /// Microsoft specified link type
    /// </summary>
    [Description("_external")]
    External,

    /// <summary>
    /// Opens the linked document in an iframe. Requires FrameTarget
    /// </summary>
    [Description("iframe")]
    Iframe
}

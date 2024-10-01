// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates how an object is sized relative to its container.
/// </summary>
public enum ObjectFit
{
    /// <summary>
    /// The image is cropped to fit within its container.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// The content is sized to maintain its aspect ratio while filling the container.
    /// </summary>
    /// <remarks>
    /// If the object's aspect ratio does not match the aspect ratio of its container, then the object will be clipped to fit.
    /// </remarks>
    [Description("cover")]
    Cover,

    /// <summary>
    /// The content is scaled to maintain its aspect ratio while filling the container.
    /// </summary>
    /// <remarks>
    /// The object is scaled to fill the container, while preserving its aspect ratio, which may cause a "letterbox" if its aspect ratio does not match the aspect ratio of its container.
    /// </remarks>
    [Description("contain")]
    Contain,

    /// <summary>
    /// The content is sized to fill the container. 
    /// </summary>
    /// <remarks>
    /// The object will completely fill the container. If the object's aspect ratio does not match the aspect ratio of its container, then the object will be stretched to fit.
    /// </remarks>
    [Description("fill")]
    Fill,

    /// <summary>
    /// The content is sized as if <see cref="ObjectFit.None"/> or <see cref="ObjectFit.Contain"/> were specified, whichever would result in a smaller object size.
    /// </summary>
    [Description("scale-down")]
    ScaleDown
}

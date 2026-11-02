// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Indicates the initial mode used by a <see cref="MudColorPicker"/>.
/// </summary>
[EnumExtensions]
public enum ColorPickerMode
{
    /// <summary>
    /// Red, Green, and Blue color values are used.
    /// </summary>
    RGB,

    /// <summary>
    /// Hue, Saturation, and Lightness color values are used.
    /// </summary>
    HSL,

    /// <summary>
    /// Hexadecimal values are used.
    /// </summary>
    HEX
}

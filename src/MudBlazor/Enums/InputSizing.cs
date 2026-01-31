// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// The strategy for how the input calculates its height.
/// </summary>
[EnumExtensions]
public enum InputSizing
{
    /// <summary>
    /// The height is fixed based on the Lines property.
    /// </summary>
    [Description("fixed")]
    Fixed,

    /// <summary>
    /// The height grows and shrinks dynamically to fit the text content.
    /// Uses Lines as minimum and MaxLines as maximum.
    /// </summary>
    /// <remarks>
    /// Previously known as "AutoGrow".
    /// </remarks>
    [Description("auto")]
    Auto,

    /// <summary>
    /// The input fills the available vertical space in its parent container.
    /// Uses Lines as minimum height and MaxLines as maximum height (optional).
    /// </summary>
    /// <remarks>
    /// Requires the parent container to have a constrained height (e.g., a flex column with bounded height, or a dialog body).
    /// When the parent does not constrain height, this behaves like <see cref="Fixed"/> with respect to Lines/MaxLines.
    /// Only applies to multiline inputs. For non-multiline inputs, this behaves like <see cref="Fixed"/>.
    /// </remarks>
    [Description("fill")]
    Fill,
}

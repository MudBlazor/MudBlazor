// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The strategy for how the input calculates its height.
/// </summary>
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
}

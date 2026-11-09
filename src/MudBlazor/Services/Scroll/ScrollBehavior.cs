// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Specifies whether a scroll animates smoothly or jumps immediately, matching the CSS <c>scroll-behavior</c> property.
/// </summary>
[EnumExtensions]
public enum ScrollBehavior
{
    /// <summary>
    /// Scrolls in a smooth fashion.
    /// </summary>
    [Description("smooth")]
    Smooth,

    /// <summary>
    /// Scrolls immediately.
    /// </summary>
    [Description("auto")]
    Auto
}

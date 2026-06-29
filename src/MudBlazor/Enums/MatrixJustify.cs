// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// The alignment of rows or columns within a <see cref="MudMatrix"/> component.
/// </summary>
[EnumExtensions]
public enum MatrixJustify
{
    /// <summary>
    /// Items are aligned to the start.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// Items are centered.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// Items are aligned to the end.
    /// </summary>
    [Description("end")]
    End,

    /// <summary>
    /// Space is applied between each item, with items aligned against the start and end.
    /// </summary>
    [Description("space-between")]
    SpaceBetween,

    /// <summary>
    /// Space is applied between each item, with additional spacing for the first and last item.
    /// </summary>
    [Description("space-around")]
    SpaceAround,

    /// <summary>
    /// Space is applied evenly between each item, including the edges of the first and last item.
    /// </summary>
    [Description("space-evenly")]
    SpaceEvenly,
}

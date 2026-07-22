// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;

namespace MudBlazor;

public enum ObjectPosition
{
    [Description("center")]
    Center,
    [Description("top")]
    Top,
    [Description("bottom")]
    Bottom,
    [Description("left")]
    Left,
    [Description("left-top")]
    LeftTop,
    [Description("left-bottom")]
    LeftBottom,
    [Description("right")]
    Right,
    [Description("right-top")]
    RightTop,
    [Description("right-bottom")]
    RightBottom,
}

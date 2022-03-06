// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;

namespace MudBlazor;

public enum ObjectFit
{
    [Description("none")]
    None,
    [Description("cover")]
    Cover,
    [Description("contain")]
    Contain,
    [Description("fill")]
    Fill,
    [Description("scale-down")]
    ScaleDown
}

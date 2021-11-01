// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace MudBlazor.Enums
{
    [Flags]
    public enum TablePagerPosition
    {
        [Description("bottom")]
        Bottom = 1,
        [Description("top")]
        Top = 2
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Smooth: scrolls in a smooth fashion;
/// Auto: is immediate
/// </summary>
public enum ScrollBehavior
{
    [Description("smooth")]
    Smooth,

    [Description("auto")]
    Auto
}

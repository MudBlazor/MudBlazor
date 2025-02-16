// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor.Enums;

public enum PaddingDensity
{
    /// <summary>
    /// Minimal spacing is applied.
    /// </summary>
    [Description("low")]
    Low,

    /// <summary>
    /// Normal  spacing is applied.
    /// </summary>
    [Description("normal")]
    Normal,

    /// <summary>
    /// Ample spacing is applied.
    /// </summary>
    [Description("high")]
    High,

    /// <summary>
    /// Maximum spacing is applied
    /// </summary>
    [Description("max")]
    Max
}

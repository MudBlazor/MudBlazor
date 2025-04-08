// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Components.Chart;

public class BarChartOptions : DefaultAxisChartOptions
{
    /// <summary>
    /// Controls how bar groups are distributed across the available space
    /// </summary>
    public Justify Justify { get; set; } = Justify.SpaceEvenly;
}

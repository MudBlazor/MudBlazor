// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace MudBlazor
{
    /// <summary>
    /// Indicates the position of the Y axis labels as either left or right in a <see cref="ChartType.HeatMap"/>
    /// </summary>
    public enum YAxisLabelPosition
    {
        /// <summary>
        /// The Y axis labels are displayed at the left of the chart
        /// </summary>
        Left,
        /// <summary>
        /// The Y Axis labels are displayed at the right of the chart
        /// </summary>
        Right,
        /// <summary>
        /// Do not include Y axis labels
        /// </summary>
        None,
    }
}

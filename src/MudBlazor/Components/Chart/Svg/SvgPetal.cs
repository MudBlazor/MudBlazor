// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Represents a petal shape along an <see cref="SvgPath"/>.
/// </summary>
public sealed class SvgPetal : SvgPath
{
    /// <summary>
    /// The radius of the segment.
    /// </summary>
    public double SegmentRadius { get; set; }
    /// <summary>
    /// The angle in radians of the segment.
    /// </summary>
    public double AngleRadians { get; set; }
    /// <summary>
    /// The offset distance for the label from the center of the petal.
    /// </summary>
    public double LabelOffset { get; set; }
}

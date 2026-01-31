// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;
#nullable enable

/// <summary>
/// Represents a specific point along an <see cref="SvgPath"/>.
/// </summary>
public sealed class SvgPathPoint : SvgPath
{
    /// <summary>
    /// The position of the point along a <see cref="SvgPath"/>
    /// </summary>
    public int PointIndex { get; set; }
}

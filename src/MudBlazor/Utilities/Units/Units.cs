// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// CSS length values for defining sizes of components.
/// </summary>
public static class Units
{
    /// <summary>
    /// Takes up a fraction of the available space.
    /// </summary>
    /// <param name="value">The number of fractions to take up.</param>
    public static TrackBreadth Fr(double value = 1) => new Fr(value);

    /// <summary>
    /// A fixed size in pixels.
    /// </summary>
    /// <param name="value">The size in pixels.</param>
    public static LengthPercentage Px(double value) => new Px(value);

    /// <summary>
    /// A fixed size in rem units.
    /// </summary>
    /// <param name="value">The size in rem units.</param>
    public static LengthPercentage Rem(double value) => new Rem(value);

    /// <summary>
    /// A percentage of the size of the parent element.
    /// </summary>
    /// <remarks>
    /// Relies on parent having a defined size on the axis that the percentage is
    /// in reference to. Otherwise it will devolve into auto.
    /// </remarks>
    /// <param name="value">The percentage, from 0 to 100.</param>
    public static LengthPercentage Pct(double value) => new Pct(value);

    /// <summary>
    /// A fixed size relative to the font size of the element.
    /// </summary>
    /// <param name="value">The size in em units.</param>
    public static LengthPercentage Em(double value) => new Em(value);

    /// <summary>
    /// A fixed size relative to 1% of the viewport's width.
    /// </summary>
    /// <param name="value">The size in vw units.</param>
    public static LengthPercentage Vw(double value) => new Vw(value);

    /// <summary>
    /// A fixed size relative to 1% of the viewport's height.
    /// </summary>
    /// <param name="value">The size in vh units.</param>
    public static LengthPercentage Vh(double value) => new Vh(value);

    /// <summary>
    /// A fixed size relative to 1% of the smaller of the viewport's width and height.
    /// </summary>
    /// <param name="value">The size in vmin units.</param>
    public static LengthPercentage VMin(double value) => new VMin(value);

    /// <summary>
    /// A fixed size relative to 1% of the larger of the viewport's width and height.
    /// </summary>
    /// <param name="value">The size in vmax units.</param>
    public static LengthPercentage VMax(double value) => new VMax(value);

    /// <summary>
    /// Sizes automatically based on the content of the element.
    /// </summary>
    public static InflexibleBreadth Auto() => new Auto();

    /// <summary>
    /// Sizes to the smallest possible size that doesn't cause overflow.
    /// </summary>
    public static InflexibleBreadth MinContent() => new MinContent();

    /// <summary>
    /// Sizes to the size the content would take up with no wrapping at all.
    /// </summary>
    public static InflexibleBreadth MaxContent() => new MaxContent();

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static TrackBreadth MinMax(InflexibleBreadth min, TrackBreadth max) => new MinMax(min, max);

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static FixedSize MinMax(LengthPercentage min, TrackBreadth max) => new FixedMinMax(min, max);

    /// <summary>
    /// Uses the smaller size between the two sizes.
    /// </summary>
    public static LengthPercentage Min(LengthPercentage a, LengthPercentage b) => new Min(a, b);

    /// <summary>
    /// Uses the larger size between two sizes.
    /// </summary>
    public static LengthPercentage Max(LengthPercentage a, LengthPercentage b) => new Max(a, b);
}

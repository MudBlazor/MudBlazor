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
    public static ITrackBreadth Fr(double value = 1) => new Fr(value);

    /// <summary>
    /// A fixed size in pixels.
    /// </summary>
    /// <param name="value">The size in pixels.</param>
    public static LengthPercentage Px(double value) => new Px(value);

    /// <summary>
    /// A fixed size relative to the font size of the root element.
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
    /// A fixed size relative to the used cap height of the element's font.
    /// </summary>
    /// <param name="value">The size in cap units.</param>
    public static LengthPercentage Cap(double value) => new Cap(value);

    /// <summary>
    /// A fixed size relative to the used cap height of the root element's font.
    /// </summary>
    /// <param name="value">The size in rcap units.</param>
    public static LengthPercentage Rcap(double value) => new Rcap(value);

    /// <summary>
    /// A fixed size relative to the advance measure of the "0" glyph of the element's font.
    /// </summary>
    /// <param name="value">The size in ch units.</param>
    public static LengthPercentage Ch(double value) => new Ch(value);

    /// <summary>
    /// A fixed size relative to the advance measure of the "0" glyph of the root element's font.
    /// </summary>
    /// <param name="value">The size in rch units.</param>
    public static LengthPercentage Rch(double value) => new Rch(value);

    /// <summary>
    /// A fixed size relative to the average advance measure of a full-width glyph of the element's font.
    /// </summary>
    /// <param name="value">The size in ic units.</param>
    public static LengthPercentage Ic(double value) => new Ic(value);

    /// <summary>
    /// A fixed size relative to the average advance measure of a full-width glyph of the root element's font.
    /// </summary>
    /// <param name="value">The size in ric units.</param>
    public static LengthPercentage Ric(double value) => new Ric(value);

    /// <summary>
    /// A fixed size in inches.
    /// </summary>
    /// <param name="value">The size in inches.</param>
    public static LengthPercentage In(double value) => new In(value);

    /// <summary>
    /// A fixed size relative to the x-height of the element's font.
    /// </summary>
    /// <param name="value">The size in ex units.</param>
    public static LengthPercentage Ex(double value) => new Ex(value);

    /// <summary>
    /// A fixed size relative to the x-height of the root element's font.
    /// </summary>
    /// <param name="value">The size in rex units.</param>
    public static LengthPercentage Rex(double value) => new Rex(value);

    /// <summary>
    /// A fixed size in centimeters.
    /// </summary>
    /// <param name="value">The size in centimeters.</param>
    public static LengthPercentage Cm(double value) => new Cm(value);

    /// <summary>
    /// A fixed size in quarter-millimeters.
    /// </summary>
    /// <param name="value">The size in q units.</param>
    public static LengthPercentage Q(double value) => new Q(value);

    /// <summary>
    /// A fixed size relative to 1% of the viewport's size in the inline axis.
    /// </summary>
    /// <param name="value">The size in vi units.</param>
    public static LengthPercentage Vi(double value) => new Vi(value);

    /// <summary>
    /// A fixed size relative to 1% of the viewport's size in the block axis.
    /// </summary>
    /// <param name="value">The size in vb units.</param>
    public static LengthPercentage Vb(double value) => new Vb(value);

    /// <summary>
    /// A fixed size in points.
    /// </summary>
    /// <param name="value">The size in points.</param>
    public static LengthPercentage Pt(double value) => new Pt(value);

    /// <summary>
    /// A fixed size in picas.
    /// </summary>
    /// <param name="value">The size in picas.</param>
    public static LengthPercentage Pc(double value) => new Pc(value);

    /// <summary>
    /// A fixed size in millimeters.
    /// </summary>
    /// <param name="value">The size in millimeters.</param>
    public static LengthPercentage Mm(double value) => new Mm(value);

    /// <summary>
    /// A fixed size relative to the computed line height of the element.
    /// </summary>
    /// <param name="value">The size in lh units.</param>
    public static LengthPercentage Lh(double value) => new Lh(value);

    /// <summary>
    /// A fixed size relative to the computed line height of the root element.
    /// </summary>
    /// <param name="value">The size in rlh units.</param>
    public static LengthPercentage Rlh(double value) => new Rlh(value);

    /// <summary>
    /// Sizes automatically based on the content of the element.
    /// </summary>
    public static IInflexibleBreadth Auto() => new Auto();

    /// <summary>
    /// Sizes to the smallest possible size that doesn't cause overflow.
    /// </summary>
    public static IInflexibleBreadth MinContent() => new MinContent();

    /// <summary>
    /// Sizes to the size the content would take up with no wrapping at all.
    /// </summary>
    public static IInflexibleBreadth MaxContent() => new MaxContent();

    /// <summary>
    /// Uses the smaller size between the two sizes.
    /// </summary>
    public static LengthPercentage Min(LengthPercentage a, LengthPercentage b) => new Min(a, b);

    /// <summary>
    /// Minimum value for a minmax unit. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Minmax clamps a size between a minimum and maximum value. 
    /// </para>
    /// <para>
    /// see <see cref="FixedMinBuilder.Max(ITrackBreadth)"/> to finish the unit.
    /// </para>
    /// </remarks>
    /// <param name="min">Minimum value for a minmax unit.</param>
    public static FixedMinBuilder Min(LengthPercentage min) => new(min);

    /// <summary>
    /// Minimum value for a minmax unit. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Minmax clamps a size between a minimum and maximum value. 
    /// </para>
    /// <para>
    /// see <see cref="InflexibleMinBuilder.Max(ITrackBreadth)"/> to finish the unit.
    /// </para>
    /// </remarks>
    public static InflexibleMinBuilder Min(IInflexibleBreadth min) => new(min);

    /// <summary>
    /// Uses the larger size between two sizes.
    /// </summary>
    public static LengthPercentage Max(LengthPercentage a, LengthPercentage b) => new Max(a, b);
}

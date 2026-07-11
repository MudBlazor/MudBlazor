// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using MudBlazor.Utilities;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

public static class Units
{
    /// <summary>
    /// Takes up a fraction of the available space.
    /// </summary>
    /// <param name="value">The number of fractions to take up.</param>
    public static TrackUnit Fr(double value = 1) => new Fr(value);

    /// <summary>
    /// A fixed size in pixels.
    /// </summary>
    /// <param name="value">The size in pixels.</param>
    public static CalcUnit Px(double value) => new Px(value);

    /// <summary>
    /// A fixed size in rem units.
    /// </summary>
    /// <param name="value">The size in rem units.</param>
    public static CalcUnit Rem(double value) => new Rem(value);

    /// <summary>
    /// A percentage of the size of the parent element.
    /// </summary>
    public static CalcUnit Pct(double value) => new Pct(value);

    /// <summary>
    /// Sizes automatically based on the content of the element.
    /// </summary>
    public static InflexibleTrackUnit Auto() => new Auto();

    /// <summary>
    /// Sizes to the smallest possible size that doesn't cause overflow.
    /// </summary>
    public static InflexibleTrackUnit MinContent() => new MinContent();

    /// <summary>
    /// Sizes to the size the content would take up with no wrapping at all.
    /// </summary>
    public static InflexibleTrackUnit MaxContent() => new MaxContent();

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>NOT</c> guaranteed to be valid inside <see cref="ExplicitMatrix.Fill(IAutoRepeatable)"/> or <see cref="ExplicitMatrix.Fill(IAutoRepeatable)"/>.
    /// </para>
    /// </remarks>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static TrackUnit MinMax(InflexibleTrackUnit min, TrackUnit max) => new MinMax(min, max);

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Valid inside <see cref="ExplicitMatrix.Fill(IAutoRepeatable)"/> or <see cref="ExplicitMatrix.Fill(IAutoRepeatable)"/>.
    /// </para>
    /// </remarks>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static IAutoRepeatable MinMax(CalcUnit min, TrackUnit max) => new MinMax(min, max);

    /// <summary>
    /// Uses the smaller size between the two sizes.
    /// </summary>
    public static CalcUnit Min(CalcUnit a, CalcUnit b) => new Min(a, b);

    /// <summary>
    /// Uses the larger size between two sizes.
    /// </summary>
    public static CalcUnit Max(CalcUnit a, CalcUnit b) => new Max(a, b);
}

/// <summary>
/// Anything valid as a grid track size, or as the <c>max</c> argument of <c>minmax()</c>.
/// </summary>
public abstract class TrackUnit : CssStringBuilder { }

internal sealed class Fr : TrackUnit
{
    public Fr(double value = 1) => Value = $"{value}fr";
}

/// <summary>
/// Anything valid as either argument of <c>minmax()</c> (i.e. everything except <see cref="Fr"/>).
/// </summary>
public abstract class InflexibleTrackUnit : TrackUnit { }

internal sealed class Auto : InflexibleTrackUnit
{
    public Auto() => Value = "auto";
}

internal sealed class MinContent : InflexibleTrackUnit
{
    public MinContent() => Value = "min-content";
}

internal sealed class MaxContent : InflexibleTrackUnit
{
    public MaxContent() => Value = "max-content";
}



/// <summary>
/// A CSS &lt;calc-value&gt; length, percentage, or nested min()/max()/calc().
/// </summary>
/// <remarks>
/// <para>
/// Valid inside calc(), min(), max(), and the +, -, *, / operators. 
/// </para>
/// <para>
/// Excludes fr, auto, min-content, max-content (see <see cref="InflexibleTrackUnit"/>).
/// </para>
/// </remarks>
public abstract class CalcUnit : InflexibleTrackUnit, IAutoRepeatable
{
    /// <summary>
    /// Adds two units together using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator +(CalcUnit a, CalcUnit b) => new CalcSum(a, SumOperator.Add, b);

    /// <summary>
    /// Subtracts two units using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator -(CalcUnit a, CalcUnit b) => new CalcSum(a, SumOperator.Subtract, b);

    /// <summary>
    /// Multiplies unit using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator *(CalcUnit a, double b) => new CalcProduct(a, ProductOperator.Multiply, b);

    /// <summary>
    /// Divides unit using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator /(CalcUnit a, double b) => new CalcProduct(a, ProductOperator.Divide, b);
}

[EnumExtensions]
internal enum SumOperator
{
    [Description("+")]
    Add,
    [Description("-")]
    Subtract,
}
internal sealed class CalcSum : CalcUnit
{
    public CalcSum(CalcUnit a, SumOperator op, CalcUnit b) => Value = $"calc({a} {op.ToStringFast(true)} {b})";

}

[EnumExtensions]
internal enum ProductOperator
{

    [Description("*")]
    Multiply,
    [Description("/")]
    Divide
}
internal sealed class CalcProduct : CalcUnit
{
    public CalcProduct(CalcUnit a, ProductOperator op, double b) => Value = $"calc({a} {op.ToStringFast(true)} {b})";
}

internal sealed class Px : CalcUnit
{
    public Px(double value) => Value = $"{value}px";
}

internal sealed class Rem : CalcUnit
{
    public Rem(double value) => Value = $"{value}rem";
}

internal sealed class Pct : CalcUnit
{
    public Pct(double value) => Value = $"{value}%";
}

internal sealed class Min : CalcUnit
{
    public Min(CalcUnit a, CalcUnit b) => Value = $"min({a}, {b})";
}

internal sealed class Max : CalcUnit
{
    public Max(CalcUnit a, CalcUnit b) => Value = $"max({a}, {b})";
}

/// <summary>
/// Valid for repeat(auto-fill/auto-fit) needs a definite length
/// somewhere. Excludes fr and bare auto/min-content/max-content.
/// </summary>
public interface IAutoRepeatable { }
internal sealed class MinMax : TrackUnit, IAutoRepeatable
{
    public MinMax(InflexibleTrackUnit min, TrackUnit max) => Value = $"minmax({min}, {max})";
}


// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
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
    public static FixedTrackUnit Auto() => new Auto();

    public static FixedTrackUnit MinContent() => new MinContent();

    public static FixedTrackUnit MaxContent() => new MaxContent();

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static TrackUnit MinMax(FixedTrackUnit min, TrackUnit max) => new MinMax(min, max);

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
/// Base type for anything that evaluates to a CSS size/track value.
/// </summary>
public abstract class CssUnitBuilder
{
    protected string _value = "";

    public override string ToString() => _value;

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public static bool operator ==(CssUnitBuilder a, CssUnitBuilder b) => a._value == b._value;

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are different.
    /// </summary>
    public static bool operator !=(CssUnitBuilder a, CssUnitBuilder b) => !(a == b);

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public override bool Equals(object? obj) => obj is CssUnitBuilder other && _value == other._value;

    /// <summary>
    /// Returns hash code of the CSS representation.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();
}

/// <summary>
/// Anything valid as a grid track size, or as the <c>max</c> argument of <c>minmax()</c>.
/// </summary>
public abstract class TrackUnit : CssUnitBuilder { }

/// <summary>
/// A flex fraction. Valid as a track size and as <c>minmax()</c>'s max argument,
/// but NOT valid as <c>minmax()</c>'s min argument or inside calc()/min()/max()
/// </summary>
internal sealed class Fr : TrackUnit
{
    public Fr(double value = 1) => _value = $"{value}fr";
}

/// <summary>
/// Anything valid as either argument of <c>minmax()</c> (i.e. everything except <see cref="Fr"/>).
/// </summary>
public abstract class FixedTrackUnit : TrackUnit { }

internal sealed class Auto : FixedTrackUnit
{
    public Auto() => _value = "auto";
}

internal sealed class MinContent : FixedTrackUnit
{
    public MinContent() => _value = "min-content";
}

internal sealed class MaxContent : FixedTrackUnit
{
    public MaxContent() => _value = "max-content";
}

[EnumExtensions]
internal enum Operator
{
    [Description("+")]
    Add,
    [Description("-")]
    Subtract,
    [Description("*")]
    Multiply,
    [Description("/")]
    Divide
}

/// <summary>
/// Anything valid inside <c>calc()</c>, <c>min()</c>, or <c>max()</c>,
/// percentages, and nested min()/max() expressions.
/// </summary>
/// <remarks>
/// excludes <see cref="Fr"/>
/// and <see cref="Auto"/>, which the math functions' grammar doesn't accept.
/// </remarks>
public abstract class CalcUnit : FixedTrackUnit
{
    /// <summary>
    /// Adds two units together using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator +(CalcUnit a, CalcUnit b) => new Calc(a, Operator.Add, b);

    /// <summary>
    /// Subtracts two units using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator -(CalcUnit a, CalcUnit b) => new Calc(a, Operator.Subtract, b);

    /// <summary>
    /// Multiplies unit using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator *(CalcUnit a, double b) => new Calc(a, Operator.Multiply, b);

    /// <summary>
    /// Divides unit using CSS <c>calc()</c>.
    /// </summary>
    public static CalcUnit operator /(CalcUnit a, double b) => new Calc(a, Operator.Divide, b);
}

internal sealed class Calc : CalcUnit
{
    public Calc(CalcUnit a, Operator op, CalcUnit b) => _value = $"calc({a} {op.ToStringFast(true)} {b})";
    public Calc(CalcUnit a, Operator op, double b) => _value = $"calc({a} {op.ToStringFast(true)} {b})";
}

public sealed class Px : CalcUnit
{
    public Px(double value) => _value = $"{value}px";
}

public sealed class Rem : CalcUnit
{
    public Rem(double value) => _value = $"{value}rem";
}

public sealed class Pct : CalcUnit
{
    public Pct(double value) => _value = $"{value}%";
}

public sealed class Min : CalcUnit
{
    public Min(CalcUnit a, CalcUnit b) => _value = $"min({a}, {b})";
}

public sealed class Max : CalcUnit
{
    public Max(CalcUnit a, CalcUnit b) => _value = $"max({a}, {b})";
}

internal sealed class MinMax : TrackUnit
{
    public MinMax(FixedTrackUnit min, TrackUnit max) => _value = $"minmax({min}, {max})";
}


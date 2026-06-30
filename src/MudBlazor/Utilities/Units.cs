// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor;

/// <summary>
/// Defines size using css units.
/// </summary>
public class Units
{
    private string _value = "100px";

    /// <summary>
    /// Takes up a fraction of the available space.
    /// </summary>
    /// <param name="value">The number of fractions to take up.</param>
    public static Units Fr(double value = 1)
    {
        return new() { _value = $"{value}fr" };
    }

    /// <summary>
    /// A fixed size in pixels.
    /// </summary>
    /// <param name="value">The size in pixels.</param>
    public static Units Px(int value)
    {
        return new() { _value = $"{value}px" };
    }

    /// <summary>
    /// A fixed size in rem units.
    /// </summary>
    /// <param name="value">The size in rem units.</param>
    public static Units Rem(double value)
    {
        return new() { _value = $"{value}rem" };
    }

    /// <summary>
    /// A percentage of the size of the parent element.
    /// </summary>
    public static Units Pct(double value)
    {
        return new() { _value = $"{value}%" };
    }

    /// <summary>
    /// Sizes automatically based on the content of the element.
    /// </summary>
    public static Units Auto()
    {
        return new() { _value = "auto" };
    }

    /// <summary>
    /// Clamps a size between a minimum and maximum value.
    /// </summary>
    /// <param name="min">The minimum size.</param>
    /// <param name="max">The maximum size.</param>
    public static Units MinMax(Units min, Units max)
    {
        return new() { _value = $"minmax({min}, {max})" };
    }

    /// <summary>
    /// Uses the smaller size between the two sizes.
    /// </summary>
    public static Units Min(Units size1, Units size2)
    {
        return new() { _value = $"min({size1}, {size2})" };
    }

    /// <summary>
    /// Uses the larger size between two sizes.
    /// </summary>
    public static Units Max(Units size1, Units size2)
    {
        return new() { _value = $"max({size1}, {size2})" };
    }

    /// <summary>
    /// Adds two units together using CSS <c>calc()</c>.
    /// </summary>
    public static Units operator +(Units a, Units b)
    {
        return new() { _value = $"calc({a} + {b})" };
    }

    /// <summary>
    /// Subtracts two units using CSS <c>calc()</c>.
    /// </summary>
    public static Units operator -(Units a, Units b)
    {
        return new() { _value = $"calc({a} - {b})" };
    }

    /// <summary>
    /// Multiplies two units using CSS <c>calc()</c>.
    /// </summary>
    public static Units operator *(Units a, Units b)
    {
        return new() { _value = $"calc({a} * {b})" };
    }

    /// <summary>
    /// Divides two units using CSS <c>calc()</c>.
    /// </summary>
    public static Units operator /(Units a, Units b)
    {
        return new() { _value = $"calc({a} / {b})" };
    }

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    /// <returns></returns>
    public static bool operator ==(Units a, Units b)
    {
        return a._value == b._value;
    }

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are different.
    /// </summary>
    public static bool operator !=(Units a, Units b)
    {
        return a._value != b._value;
    }

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Units units && _value == units._value;
    }

    /// <summary>
    /// Returns hash code of the CSS representation.
    /// </summary>
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <summary>
    /// Returns CSS representation.
    /// </summary>
    public override string ToString()
    {
        return _value;
    }
}

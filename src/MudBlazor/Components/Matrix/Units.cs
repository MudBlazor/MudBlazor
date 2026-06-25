// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor;

/// <summary>
/// Defines size using css units.
/// </summary>
/// <seealso cref="ExplicitMatrix"/>
public class Units
{
    private string _value = "100px";

    /// <summary>
    /// Takes up a fraction of the available space.
    /// </summary>
    /// <param name="value">The number of fractions to take up.</param>
    public static Units Fr(double value = 1) 
    {
        return new() 
        { 
            _value = $"{value}fr" 
        };
    }

    /// <summary>
    /// A fixed size in pixels.
    /// </summary>
    /// <param name="value">The size in pixels.</param>
    public static Units Px(int value) 
    {
        return new() 
        { 
            _value = $"{value}px" 
        };
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
    public static Units Percent(double value)
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

    public override string ToString() => _value;
}

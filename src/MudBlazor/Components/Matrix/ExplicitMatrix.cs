// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Defines the template for a row or column within a <see cref="MudMatrix"/>.
/// </summary>
/// <seealso cref="MudMatrix"/>
/// <seealso cref="Units"/>
public class ExplicitMatrix
{
    private string _value = "none";

    /// <summary>
    /// Defines the sizing pattern for columns or rows in the matrix.
    /// </summary>
    /// <param name="items">The size of each column or row in order.</param>
    public static ExplicitMatrix Pattern(params TrackUnit[] items)
    {
        return new()
        {
            _value = string.Join(" ", items.Select(i => i.ToString()))
        };
    }

    /// <summary>
    /// Defines the sizing pattern for columns or rows in the matrix.
    /// </summary>
    /// <param name="count">The number of times to repeat the columns or row sizes.</param>
    /// <param name="items">The size of each columns or row in order to repeat.</param>
    public static ExplicitMatrix Pattern(int count, params TrackUnit[] items)
    {
        return new()
        {
            _value = $"repeat({count}, {string.Join(" ", items.Select(i => i.ToString()))})"
        };
    }

    /// <summary>
    /// Creates as many columns or rows as can fit in the available space, preserving empty tracks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="Fit(TrackUnit)"/>, if there arent enough items to fill the column or row, there will be empty space left.
    /// </para>
    /// <para>
    /// Maps to CSS <c>repeat(auto-fill, size)</c>.
    /// </para>
    /// <para>
    /// <c>Fr is not a valid parameter!</c> It must be a unit of fixed size.
    /// </para>
    /// </remarks>
    /// <param name="size">The fixed size of each <see cref="MudMatrixItem"/> to repeat.</param>
    public static ExplicitMatrix Fill(TrackUnit size)
    {
        return new()
        {
            _value = $"repeat(auto-fill, {size})"
        };
    }

    /// <summary>
    /// Creates as many columns or rows as can fit in the available space.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="Fill(TrackUnit)"/>, if there arent enough items to fill the row or column, the remaining ones will stretch to fill the empty space.
    /// </para>
    /// <para>
    /// Maps to CSS <c>repeat(auto-fit, size)</c>.
    /// </para>
    /// <para>
    /// <c>Fr is not a valid parameter!!</c> It must be a unit of fixed size
    /// </para>
    /// </remarks>
    /// <param name="size">The fixed size of <see cref="MudMatrixItem"/> to repeat.</param>
    public static ExplicitMatrix Fit(TrackUnit size)
    {
        return new()
        {
            _value = $"repeat(auto-fit, {size})"
        };
    }

    /// <summary>
    /// Returns CSS representation.
    /// </summary>
    public override string ToString()
    {
        return _value;
    }
}


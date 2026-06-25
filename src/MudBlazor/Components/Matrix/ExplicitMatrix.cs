// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Defines the template for a row or column within a <see cref="MudMatrix"/>.
/// </summary>
/// <seealso cref="Units"/>
public class ExplicitMatrix
{
    private string _value = "auto";

    /// <summary>
    /// Defines the sizing pattern for columns or rows in the matrix.
    /// </summary>
    /// <param name="items">The size of each column or row in order.</param>
    public static ExplicitMatrix Pattern(params Units[] items)
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
    public static ExplicitMatrix Pattern(int count, params Units[] items)
    {
        return new()
        {
            _value = $"repeat({count}, {string.Join(" ", items.Select(i => i.ToString()))})"
        };
    }

    /// <summary>
    /// Creates as many columns or rows as can fit in the available space.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If there are not enough items to fill the row or column, there will be an empty space.
    /// </para>
    /// </remarks>
    /// <param name="items">The size of each <see cref="MudMatrixItem"/> in order to repeat.</param>
    public static ExplicitMatrix Fill(params Units[] items) 
    {
        return new() 
        {
            _value = $"repeat(auto-fill,  {string.Join(" ", items.Select(i => i.ToString()))})" 
        };
    }

    /// <summary>
    /// Creates as many columns or rows as can fit in the available space.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If there are not enough items to fill the row or column, the remaining ones will stretch to fill the empty space.
    /// </para>
    /// </remarks>
    /// <param name="items">The size of each <see cref="MudMatrixItem"/> in order to repeat.</param>
    public static ExplicitMatrix Fit(params Units[] items)
    {
        return new()
        {
            _value = $"repeat(auto-fit, {string.Join(" ", items.Select(i => i.ToString()))})"
        };
    }

    public override string ToString() => _value;
}


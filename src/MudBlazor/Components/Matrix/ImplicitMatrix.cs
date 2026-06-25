// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Defines the overflow for a row or column within a <see cref="MudMatrix"/>.
/// </summary>
/// <seealso cref="Units"/>
public class ImplicitMatrix
{
    private string _value = "auto";

    /// <summary>
    /// Defines the track sizes to be repeated.
    /// </summary>
    /// <param name="items">The size of each track in order.</param>
    public static ImplicitMatrix Pattern(params Units[] items)
    {
        return new()
        {
            _value = string.Join(" ", items.Select(i => i.ToString()))
        };
    }

    /// <summary>
    /// Defines the repeated track sizes by repeating a set of tracks a set number of times.
    /// </summary>
    /// <param name="count">The number of times to repeat the track sizes.</param>
    /// <param name="items">The size of each track in order to repeat.</param>
    public static ImplicitMatrix Pattern(int count, params Units[] items)
    {
        return new()
        {
            _value = $"repeat({count}, {string.Join(" ", items.Select(i => i.ToString()))})"
        };
    }

    public override string ToString() => _value;
}


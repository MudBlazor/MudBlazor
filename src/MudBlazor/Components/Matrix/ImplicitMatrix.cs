// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Defines the overflow for a row or column within a <see cref="MudMatrix"/>.
/// </summary>
/// <seealso cref="MudMatrix"/>
/// <seealso cref="Units"/>
public class ImplicitMatrix : CssStringBuilder
{
    protected override string DefaultValue() => "auto";

    /// <summary>
    /// Defines the track breadths to be repeated.
    /// </summary>
    /// <param name="items">The size of each breadth in order.</param>
    public static ImplicitMatrix Pattern(params ITrackBreadth[] items)
    {
        return new()
        {
            Value = string.Join(" ", items.Select(i => i.ToString()))
        };
    }
}


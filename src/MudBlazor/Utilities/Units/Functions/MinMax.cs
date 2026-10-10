// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class MinMax : CssStringBuilder, ITrackBreadth
{
    public MinMax(IInflexibleBreadth min, ITrackBreadth max) => Value = $"minmax({min}, {max})";
}

internal sealed class MinMaxFixedMin : CssStringBuilder, ITrackBreadth, IFixedSize
{
    public MinMaxFixedMin(LengthPercentage min, ITrackBreadth max) => Value = $"minmax({min}, {max})";
}

internal sealed class MinMaxFixedMax : CssStringBuilder, ITrackBreadth, IFixedSize
{
    public MinMaxFixedMax(ITrackBreadth min, LengthPercentage max) => Value = $"minmax({min}, {max})";
}

public class FixedMinBuilder
{
    private readonly LengthPercentage _min;
    internal FixedMinBuilder(LengthPercentage min) => _min = min;

    /// <summary>
    /// Maximum value for a minmax unit. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Minmax clamps a size between a minimum and maximum value. 
    /// </para>
    /// </remarks>
    public IFixedSize Max(ITrackBreadth max) => new MinMaxFixedMin(_min, max);
}

public class InflexibleMinBuilder
{
    private readonly IInflexibleBreadth _min;
    internal InflexibleMinBuilder(IInflexibleBreadth min) => _min = min;

    /// <summary>
    /// Maximum value for a minmax unit. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Minmax clamps a size between a minimum and maximum value. 
    /// </para>
    /// <para>
    /// <c>NOT</c> valid inside of <see cref="ExplicitMatrix.Fit(IFixedSize)"/> or 
    /// <see cref="ExplicitMatrix.Fill(IFixedSize)"/>. Use <see cref="Max(LengthPercentage)"/>
    /// or <see cref="Units.Min(LengthPercentage)"/> in that case.
    /// </para>
    /// </remarks>
    public ITrackBreadth Max(ITrackBreadth max) => new MinMax(_min, max);

    /// <summary>
    /// Maximum value for a minmax unit. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Minmax clamps a size between a minimum and maximum value. 
    /// </para>
    /// </remarks>
    public IFixedSize Max(LengthPercentage max) => new MinMaxFixedMax(_min, max);
}

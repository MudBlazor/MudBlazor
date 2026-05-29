using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MudBlazor;

/// <summary>
/// Provides a comparer for <see cref="double"/> values with an epsilon tolerance.
/// </summary>
[DebuggerDisplay("Epsilon = {_epsilon}")]
public class DoubleEpsilonEqualityComparer : IEqualityComparer<double>
{
    /// <summary>
    /// A constant holding the smallest positive normal value of type double.
    /// </summary>
    private const double DoubleMinNormal = (1L << 52) * double.Epsilon;
    private readonly double _epsilon;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleEpsilonEqualityComparer"/> class with the default epsilon value.
    /// </summary>
    public DoubleEpsilonEqualityComparer() : this(DoubleMinNormal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleEpsilonEqualityComparer"/> class with the specified epsilon value.
    /// </summary>
    /// <param name="epsilon">The tolerance value for comparison.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="epsilon"/> is not within the valid range.</exception>
    public DoubleEpsilonEqualityComparer(double epsilon)
    {
        if (epsilon is <= 0 or >= 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(epsilon), @"Epsilon must be greater than 0 and less than 1.");
        }

        _epsilon = epsilon;
    }

    /// <inheritdoc/>
    public bool Equals(double x, double y)
    {
        // Copyright (c) Michael Borgwardt
        var absA = Math.Abs(x);
        var absB = Math.Abs(y);
        var diff = Math.Abs(x - y);

        if (x.Equals(y))
        {
            // shortcut, handles infinities
            return true;
        }

        if (x == 0 || y == 0 || absA + absB < DoubleMinNormal)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < _epsilon * DoubleMinNormal;
        }

        // use relative error
        return diff / Math.Min(absA + absB, double.MaxValue) < _epsilon;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Same as <see cref="double.GetHashCode"/>.
    /// </remarks>
    public int GetHashCode(double obj) => obj.GetHashCode();

    /// <summary>
    /// Gets the default instance of <see cref="DoubleEpsilonEqualityComparer"/>.
    /// </summary>
    public static readonly DoubleEpsilonEqualityComparer Default = new();
}

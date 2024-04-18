// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor.UnitTests.Comparer;

#nullable enable
public class DoubleArrayEpsilonEqualityComparer : IEqualityComparer<double[]>
{
    private readonly IEqualityComparer<double> _elementComparer;

    public DoubleArrayEpsilonEqualityComparer(IEqualityComparer<double> elementComparer)
    {
        _elementComparer = elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
    }

    public bool Equals(double[]? x, double[]? y)
    {
        if (x == null || y == null)
        {
            return x == y;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (var i = 0; i < x.Length; i++)
        {
            if (!_elementComparer.Equals(x[i], y[i]))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(double[] obj)
    {
        unchecked
        {
            var hash = 17;
            foreach (var item in obj)
            {
                hash = hash * 31 + _elementComparer.GetHashCode(item);
            }

            return hash;
        }
    }
}

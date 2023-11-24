// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor.UnitTests.TestComponents;

public class DelegateEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> _equals;
    private readonly Func<T, int> _getHashCode;

    public DelegateEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
    {
        _equals = equals ?? throw new ArgumentNullException(nameof(equals));
        _getHashCode = getHashCode ?? throw new ArgumentNullException(nameof(getHashCode));
    }

    public bool Equals(T x, T y)
    {
        return _equals(x, y);
    }

    public int GetHashCode(T obj)
    {
        return _getHashCode(obj);
    }
}

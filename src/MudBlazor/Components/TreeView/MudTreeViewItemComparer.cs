// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable

using System.Collections.Generic;

namespace MudBlazor;

public class MudTreeViewItemComparer<T> : IEqualityComparer<MudTreeViewItem<T>>
{
    private readonly IEqualityComparer<T?> _valueComparer;

    public MudTreeViewItemComparer(IEqualityComparer<T?> valueComparer)
    {
        _valueComparer = valueComparer;
    }

    public bool Equals(MudTreeViewItem<T>? x, MudTreeViewItem<T>? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return _valueComparer.Equals(x.Value, y.Value);
    }

    public int GetHashCode(MudTreeViewItem<T> obj)
    {
        return obj.Value != null ? _valueComparer.GetHashCode(obj.Value) : 0;
    }
}


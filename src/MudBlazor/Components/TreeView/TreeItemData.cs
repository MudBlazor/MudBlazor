// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor;
#nullable enable

public class TreeItemData<T> : IEquatable<TreeItemData<T>>
{
    public TreeItemData(T? value)
    {
        Value = value;
    }

    public string? Text { get; set; }

    public string? Icon { get; set; }

    public T? Value { get; init; }

    public bool Expanded { get; set; }

    public bool Expandable { get; set; } = true;

    public bool Selected { get; set; }

    public virtual List<TreeItemData<T>>? Children { get; set; }

    public virtual bool HasChildren => Children is not null && Children.Count > 0;

    public virtual bool Equals(TreeItemData<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        return Equals((TreeItemData<T>)obj);
    }

    public override int GetHashCode()
    {
        if (Value is null)
            return 0;
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}

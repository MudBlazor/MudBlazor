// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities;

/// <summary>
/// Base type for anything that evaluates to a CSS size/track value.
/// </summary>
public abstract class CssStringBuilder : IEquatable<CssStringBuilder>
{
    protected virtual string DefaultValue() => "";

    protected string Value
    {
        get => field ??= DefaultValue();
        init;
    }

    protected CssStringBuilder() => Value = DefaultValue();

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public bool Equals(CssStringBuilder? other) => other is not null && Value == other.Value;

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public override bool Equals(object? obj) => obj is CssStringBuilder other && Value == other.Value;

    /// <summary>
    /// Returns hash code of the CSS representation.
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns CSS representation.
    /// </summary>
    public override string ToString() => Value;

}

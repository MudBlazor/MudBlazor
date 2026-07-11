// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    protected string _value;

    protected CssStringBuilder() => _value = DefaultValue();

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public bool Equals(CssStringBuilder? other) => other is not null && _value == other._value;

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public override bool Equals(object? obj) => obj is CssStringBuilder other && _value == other._value;

    /// <summary>
    /// Returns hash code of the CSS representation.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Returns CSS representation.
    /// </summary>
    public override string ToString() => _value;

}

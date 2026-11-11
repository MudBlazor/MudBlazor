// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities;

/// <summary>
/// Base type for constructing CSS input.
/// </summary>
public abstract class CssStringBuilder : IEquatable<CssStringBuilder>
{
    protected virtual string DefaultValue() => "";

    protected string Value
    {
        get => field ??= DefaultValue();
        init;
    }

    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public bool Equals(CssStringBuilder? other) => string.Equals(other?.Value, Value);
    /// <summary>
    /// <c>Returns true</c> when the CSS representation of the Units are the same.
    /// </summary>
    public override bool Equals(object? obj) => obj is CssStringBuilder other && string.Equals(Value, other.Value);

    /// <summary>
    /// Returns hash code of the CSS representation.
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns CSS representation.
    /// </summary>
    public override string ToString() => Value;


}

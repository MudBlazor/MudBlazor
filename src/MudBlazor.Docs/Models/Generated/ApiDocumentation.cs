// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents a set of XML documentation for MudBlazor types.
/// </summary>
public static partial class ApiDocumentation
{
    /// <summary>
    /// The types which have documentation.
    /// </summary>
    public static FrozenDictionary<string, DocumentedType> Types { get; private set; }

    /// <summary>
    /// Gets a documented type by its name.
    /// </summary>
    /// <param name="name">The name of the type to find.</param>
    public static DocumentedType GetType(string name)
    {
        // First, try an exact match
        if (Types.TryGetValue(name, out var match))
        {
            return match;
        }
        if (Types.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        return null;
    }
}

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
    /// Converts pages to their actual .NET types.
    /// </summary>
    public static Dictionary<string, string> PagesToTypes { get; private set; } = new()
    {
        { "alert", "MudAlert" },
        { "appbar", "MudAppBar" },
        { "avatar", "MudAvatar" },
        { "badge", "MudBadge" }
    };

    /// <summary>
    /// Gets a documented type by its name.
    /// </summary>
    /// <param name="name"></param>
    public static DocumentedType GetType(string name)
    {
        // Was the type found with the literal name?
        if (Types.TryGetValue(name, out var type))
        {
            return type;
        }
        // Are we mapping a page to a type?  (e.g. "alert" to "MudAlert")
        if (PagesToTypes.TryGetValue(name, out var typeName) && Types.TryGetValue(typeName, out var resolvedType))
        {
            return resolvedType;
        }
        // Nothing was found
        return null;
    }
}

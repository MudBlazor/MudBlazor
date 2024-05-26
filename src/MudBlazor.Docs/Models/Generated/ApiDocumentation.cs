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
    /// The generated documentation for events.
    /// </summary>
    public static Dictionary<string, DocumentedEvent> Events { get; private set; }

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public static Dictionary<string, DocumentedField> Fields { get; private set; }

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public static Dictionary<string, DocumentedType> Types { get; private set; }

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public static Dictionary<string, DocumentedProperty> Properties { get; private set; }

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public static Dictionary<string, DocumentedMethod> Methods { get; private set; }

    /// <summary>
    /// Gets a documented type by its name.
    /// </summary>
    /// <param name="name">The name of the type to find.</param>
    public static DocumentedType GetType(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Types.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Types.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Types.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }
}

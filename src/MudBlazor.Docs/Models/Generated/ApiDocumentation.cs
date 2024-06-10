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
    public static Dictionary<string, DocumentedEvent> Events { get; private set; } = [];

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public static Dictionary<string, DocumentedField> Fields { get; private set; } = [];

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public static Dictionary<string, DocumentedType> Types { get; private set; } = [];

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public static Dictionary<string, DocumentedProperty> Properties { get; private set; } = [];

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public static Dictionary<string, DocumentedMethod> Methods { get; private set; } = [];

    /// <summary>
    /// Gets an event, field, method, or property by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static DocumentedMember GetMember(string name)
    {
        DocumentedMember result = GetProperty(name);
        result ??= GetField(name);
        result ??= GetMethod(name);
        result ??= GetEvent(name);
        return result;
    }

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

    /// <summary>
    /// Gets a documented property by its name.
    /// </summary>
    /// <param name="name">The name of the property to find.</param>
    public static DocumentedProperty GetProperty(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Properties.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Properties.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Properties.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented property by its name.
    /// </summary>
    /// <param name="name">The name of the field to find.</param>
    public static DocumentedField GetField(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Fields.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Fields.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Fields.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented method by its name.
    /// </summary>
    /// <param name="name">The name of the method to find.</param>
    public static DocumentedMethod GetMethod(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Methods.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Methods.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Methods.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented event by its name.
    /// </summary>
    /// <param name="name">The name of the event to find.</param>
    public static DocumentedEvent GetEvent(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Events.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Events.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Events.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }
}

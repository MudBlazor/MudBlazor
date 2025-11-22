// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public class DocumentedType
{
    /// <summary>
    /// The name of the type.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The unique key for this type.
    /// </summary>
    public required string? Key { get; init; }

    /// <summary>
    /// The XML documentation summary for this type.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// The XML documentation remarks for this type.
    /// </summary>
    public string? Remarks { get; init; }

    /// <summary>
    /// The type can be seen externally.
    /// </summary>
    public bool IsPublic { get; init; }

    /// <summary>
    /// The type is a base class.
    /// </summary>
    public bool IsAbstract { get; init; }

    /// <summary>
    /// The .NET Type related to this type.
    /// </summary>
    public required Type Type { get; init; }

    /// <summary>
    /// The .NET type this inherits from.
    /// </summary>
    public Type? BaseType { get; init; }

    /// <summary>
    /// The properties in this type.
    /// </summary>
    public Dictionary<string, DocumentedProperty> Properties { get; } = [];

    /// <summary>
    /// The methods in this type.
    /// </summary>
    public Dictionary<string, DocumentedMethod> Methods { get; } = [];

    /// <summary>
    /// The events in this type.
    /// </summary>
    public Dictionary<string, DocumentedEvent> Events { get; } = [];

    /// <summary>
    /// The fields in this type.
    /// </summary>
    public Dictionary<string, DocumentedField> Fields { get; } = [];

    /// <summary>
    /// The types declared within this type.
    /// </summary>
    public Dictionary<string, DocumentedType> NestedTypes { get; } = [];

    /// <summary>
    /// The global settings related to this type.
    /// </summary>
    public Dictionary<string, DocumentedProperty> GlobalSettings { get; } = [];

    /// <summary>
    /// The see-also links for this type.
    /// </summary>
    public List<DocumentedLink> Links { get; init; } = [];
}

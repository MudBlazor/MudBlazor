// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public sealed class DocumentedType
{
    /// <summary>
    /// The Reflection name of this type.
    /// </summary>
    public string Name { get; init; } = "";

    /// <summary>
    /// The user-facing name of this type.
    /// </summary>
    public string NameFriendly { get; init; } = "";

    /// <summary>
    /// The relative URL to this type's documentation.
    /// </summary>
    public string ApiUrl => "/api/" + Name;

    /// <summary>
    /// The link to examples related to this type.
    /// </summary>
    public string ComponentUrl => "/components/" + Name;

    /// <summary>
    /// Whether this type is a Blazor component.
    /// </summary>
    public bool IsComponent { get; init; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string Summary { get; init; } = "";

    /// <summary>
    /// The brief summary of this member as plain text.
    /// </summary>
    public string? SummaryPlain => Summary == null ? null : DocumentedMember.GetPlaintext(Summary);

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string Remarks { get; init; } = "";

    /// <summary>
    /// The Reflection name of this type's base type.
    /// </summary>
    public string BaseTypeName { get; init; } = "";

    /// <summary>
    /// The type this type inherits from.
    /// </summary>
    public DocumentedType? BaseType => ApiDocumentation.GetType(BaseTypeName);

    /// <summary>
    /// The documented types inheriting from this class.
    /// </summary>
    public List<DocumentedType> Children => ApiDocumentation.Types.Values.Where(type => type.BaseTypeName == Name).ToList();

    /// <summary>
    /// The properties in this type (including inherited properties).
    /// </summary>
    public Dictionary<string, DocumentedProperty> Properties { get; init; } = [];

    /// <summary>
    /// The methods in this type (including inherited methods).
    /// </summary>
    public Dictionary<string, DocumentedMethod> Methods { get; init; } = [];

    /// <summary>
    /// The fields in this type (including inherited fields).
    /// </summary>
    public Dictionary<string, DocumentedField> Fields { get; init; } = [];

    /// <summary>
    /// The events in this type.
    /// </summary>
    public Dictionary<string, DocumentedEvent> Events { get; init; } = [];

    /// <summary>
    /// The properties in this type (including inherited properties).
    /// </summary>
    public Dictionary<string, DocumentedProperty> GlobalSettings { get; init; } = [];

    /// <summary>
    /// The see-also links for this type.
    /// </summary>
    public List<DocumentedLink> Links { get; init; } = [];
}

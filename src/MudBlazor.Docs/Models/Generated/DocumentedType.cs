// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public class DocumentedType : IComparable<DocumentedType>
{
    /// <summary>
    /// The Reflection name of this type.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The user-facing name of this type.
    /// </summary>
    public string NameFriendly { get; set; } = "";

    /// <summary>
    /// The relative URL to this type's documentation.
    /// </summary>
    public string ApiUrl => "/api/" + Name;

    /// <summary>
    /// The link to examples related to this type.
    /// </summary>
    public string? ComponentUrl => "/components/" + Name;

    /// <summary>
    /// Whether this type is a Blazor component.
    /// </summary>
    public bool IsComponent { get; set; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The brief summary of this member as plain text.
    /// </summary>
    public string? SummaryPlain => Summary == null ? null : DocumentedMember.GetPlaintext(Summary);

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// The Reflection name of this type's base type.
    /// </summary>
    public string? BaseTypeName { get; set; }

    /// <summary>
    /// The documentation for the base class.
    /// </summary>
    public DocumentedType BaseType => ApiDocumentation.GetType(BaseTypeName);

    /// <summary>
    /// The documented types inheriting from this class.
    /// </summary>
    public List<DocumentedType> Children => ApiDocumentation.Types.Values.Where(type => type.BaseTypeName == Name).ToList();

    /// <summary>
    /// The properties in this type (including inherited properties).
    /// </summary>
    public Dictionary<string, DocumentedProperty> Properties { get; set; } = [];

    /// <summary>
    /// The methods in this type (including inherited methods).
    /// </summary>
    public Dictionary<string, DocumentedMethod> Methods { get; set; } = [];

    /// <summary>
    /// The fields in this type (including inherited fields).
    /// </summary>
    public Dictionary<string, DocumentedField> Fields { get; set; } = [];

    /// <summary>
    /// The events in this type.
    /// </summary>
    public Dictionary<string, DocumentedEvent> Events { get; set; } = [];

    /// <summary>
    /// The properties in this type (including inherited properties).
    /// </summary>
    public Dictionary<string, DocumentedProperty> GlobalSettings { get; set; } = [];

    public int CompareTo(DocumentedType? other)
    {
        if (other == null)
        {
            return -1;
        }
        return Name.CompareTo(other.Name);
    }
}

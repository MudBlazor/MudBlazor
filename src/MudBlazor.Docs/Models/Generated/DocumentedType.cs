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
    /// The unique key for this type, such as <c>MudBlazor.MudAlert</c>.
    /// </summary>
    /// <remarks>
    /// Used to build this type's members on first use.  See <see cref="ApiDocumentationMembers"/>.
    /// </remarks>
    public string Key { get; init; } = "";

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

    // The member collections are filled by this type's loader in ApiDocumentationMembers, which runs the first time something reads them.
    // The generator fills them through the internal views, which do not trigger it.

    private readonly Dictionary<string, DocumentedProperty> _properties = [];
    private readonly Dictionary<string, DocumentedMethod> _methods = [];
    private readonly Dictionary<string, DocumentedField> _fields = [];
    private readonly Dictionary<string, DocumentedEvent> _events = [];
    private readonly Dictionary<string, DocumentedProperty> _globalSettings = [];

    /// <summary>
    /// The properties in this type (including inherited properties).
    /// </summary>
    public Dictionary<string, DocumentedProperty> Properties
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _properties;
        }
    }

    /// <summary>
    /// The methods in this type (including inherited methods).
    /// </summary>
    public Dictionary<string, DocumentedMethod> Methods
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _methods;
        }
    }

    /// <summary>
    /// The fields in this type (including inherited fields).
    /// </summary>
    public Dictionary<string, DocumentedField> Fields
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _fields;
        }
    }

    /// <summary>
    /// The events in this type.
    /// </summary>
    public Dictionary<string, DocumentedEvent> Events
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _events;
        }
    }

    /// <summary>
    /// The properties in this type which have a global setting.
    /// </summary>
    public Dictionary<string, DocumentedProperty> GlobalSettings
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _globalSettings;
        }
    }

    /// <summary>
    /// The properties in this type, without building the member documentation first.
    /// </summary>
    internal Dictionary<string, DocumentedProperty> PropertiesInternal => _properties;

    /// <summary>
    /// The methods in this type, without building the member documentation first.
    /// </summary>
    internal Dictionary<string, DocumentedMethod> MethodsInternal => _methods;

    /// <summary>
    /// The fields in this type, without building the member documentation first.
    /// </summary>
    internal Dictionary<string, DocumentedField> FieldsInternal => _fields;

    /// <summary>
    /// The events in this type, without building the member documentation first.
    /// </summary>
    internal Dictionary<string, DocumentedEvent> EventsInternal => _events;

    /// <summary>
    /// The global settings in this type, without building the member documentation first.
    /// </summary>
    internal Dictionary<string, DocumentedProperty> GlobalSettingsInternal => _globalSettings;

    private readonly List<DocumentedLink> _links = [];

    /// <summary>
    /// The see-also links for this type.
    /// </summary>
    /// <remarks>
    /// See-also links can point at a member, so they are built with the member documentation.
    /// </remarks>
    public List<DocumentedLink> Links
    {
        get
        {
            ApiDocumentationMembers.EnsureType(Key);
            return _links;
        }
    }

    /// <summary>
    /// The see-also links for this type, without building the member documentation first.
    /// </summary>
    internal List<DocumentedLink> LinksInternal => _links;
}

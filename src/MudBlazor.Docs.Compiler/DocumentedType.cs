// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
    public string Name { get; set; }

    /// <summary>
    /// The unique key for this type.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The key for this type for XML documentation.
    /// </summary>
    public string XmlKey { get; set; }

    /// <summary>
    /// The XML documentation summary for this type.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// The XML documentation remarks for this type.
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// The type can be seen externally.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The type is a base class.
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// The .NET Type related to this type.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// The .NET type this inherits from.
    /// </summary>
    public Type BaseType { get; set; }

    /// <summary>
    /// The properties in this type.
    /// </summary>
    public Dictionary<string, DocumentedProperty> Properties { get; set; } = [];

    /// <summary>
    /// The methods in this type.
    /// </summary>
    public Dictionary<string, DocumentedMethod> Methods { get; set; } = [];

    /// <summary>
    /// The events in this type.
    /// </summary>
    public Dictionary<string, DocumentedEvent> Events { get; set; } = [];

    /// <summary>
    /// The fields in this type.
    /// </summary>
    public Dictionary<string, DocumentedField> Fields { get; set; } = [];

    /// <summary>
    /// The types declared within this type.
    /// </summary>
    public Dictionary<string, DocumentedType> NestedTypes { get; set; } = [];

    /// <summary>
    /// The global settings related to this type.
    /// </summary>
    public Dictionary<string, DocumentedProperty> GlobalSettings { get; set; } = [];
}

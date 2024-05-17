// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public class DocumentedType
{
    public string Name { get; set; }
    public string Key { get; set; }
    public string XmlKey { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAbstract { get; set; }
    public Type Type { get; set; }
    public Type BaseType { get; set; }
    public Dictionary<string, DocumentedProperty> Properties { get; set; } = [];
    public Dictionary<string, DocumentedMethod> Methods { get; set; } = [];
    public Dictionary<string, DocumentedEvent> Events { get; set; } = [];
    public Dictionary<string, DocumentedField> Fields { get; set; } = [];
}

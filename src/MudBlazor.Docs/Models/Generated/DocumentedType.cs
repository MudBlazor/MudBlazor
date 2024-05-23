// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public class DocumentedType
{
    public string Name { get; set; }
    public string LegacyApiUrl => "/api/" + (IsComponent ? Name.Replace("Mud", "") : Name);
    public string ApiUrl => "/api/" + Name;
    public string ElementName => (IsComponent ? $"<{Name}>" : null);
    public string ComponentUrl => (IsComponent ? "/components/" + Name.Replace("Mud", "") : null);
    public bool IsComponent { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public string BaseTypeName { get; set; }
    public DocumentedType BaseType => ApiDocumentation.GetType(BaseTypeName);
    public List<DocumentedType> Children => ApiDocumentation.Types.Values.Where(type => type.BaseTypeName == Name).ToList();
    public Dictionary<string, DocumentedProperty> Properties { get; set; } = [];
    public Dictionary<string, DocumentedMethod> Methods { get; set; } = [];
    public Dictionary<string, DocumentedField> Fields { get; set; } = [];
    public Dictionary<string, DocumentedEvent> Events { get; set; } = [];
    public Dictionary<string, DocumentedProperty> GlobalSettings { get; set; } = [];
}

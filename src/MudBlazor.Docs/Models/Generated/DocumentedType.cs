// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents documentation for a type.
/// </summary>
[DebuggerDisplay("{Name}: Summary={Summary}")]
public class DocumentedType
{
    public string Name { get; set; }
    public string Key { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAbstract { get; set; }
    public List<DocumentedProperty> Properties { get; set; } = [];
    public List<DocumentedMethod> Methods { get; set; } = [];
    public List<DocumentedEvent> Events { get; set; } = [];
}

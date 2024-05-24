// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents documentation for a method.
/// </summary>
[DebuggerDisplay("({ReturnType}) {Name}: {Summary}")]
public class DocumentedMethod
{
    public string ApiUrl => "/api/" + Name;
    public string Category { get; set; } = "General";
    public string? DeclaringTypeFriendlyName { get; set; }
    public string? DeclaringTypeName { get; set; }
    public DocumentedType? DeclaringType => string.IsNullOrEmpty(DeclaringTypeName) ? null : ApiDocumentation.Types[DeclaringTypeName];
    public string? DeclaringTypeApiLink => $"/api/{DeclaringTypeName}";
    public bool IsPublic { get; set; }
    public bool IsProtected { get; set; }
    public string? Name { get; set; } = "";
    public int? Order { get; set; }
    public string? Remarks { get; set; }
    public string ReturnTypeFriendlyName { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string? Summary { get; set; }
    public Dictionary<string, DocumentedParameter> Parameters { get; set; } = [];
}

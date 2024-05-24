// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents documentation for a property.
/// </summary>
[DebuggerDisplay("({Type}) {Name}: {Summary}")]
public sealed class DocumentedProperty
{
    public string Category { get; set; } = "General";
    public string? DeclaringTypeName { get; set; }
    public string? DeclaringTypeFriendlyName { get; set; }
    public DocumentedType? DeclaringType => string.IsNullOrEmpty(DeclaringTypeName) ? null : ApiDocumentation.Types[DeclaringTypeName];
    public string? DeclaringTypeApiUrl => $"/api/{DeclaringTypeName}";
    public string Name { get; set; } = "";
    public int? Order { get; set; }
    public string? Remarks { get; set; }
    public string? Summary { get; set; }
    public string Type { get; set; } = "";
    public string? TypeFriendlyName { get; set; }
    public bool IsPublic { get; set; }
    public bool IsProtected { get; set; }
    public bool IsParameter { get; set; }
}

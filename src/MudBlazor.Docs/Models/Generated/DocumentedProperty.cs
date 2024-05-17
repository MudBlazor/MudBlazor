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
[DebuggerDisplay("({PropertyTypeName}) {Name}: {Summary}")]
public sealed class DocumentedProperty
{
    public string ApiUrl => "/api/" + Name;
    public string Category { get; set; } = "General";
    public string? DeclaringType { get; set; }
    public string? DeclaringTypeApiLink => $"/api/{DeclaringType}";
    public string Name { get; set; } = "";
    public int? Order { get; set; }
    public string? Remarks { get; set; }
    public string? Summary { get; set; }
    public string Type { get; set; } = "";
    public string TypeCSharp => Type.Replace("Boolean", "bool").Replace("Int32", "int").Replace("Int64", "long").Replace("String", "string").Replace("Double", "double").Replace("Single", "float").Replace("Object", "object");
    public bool IsPublic { get; set; }
    public bool IsProtected { get; set; }
    public bool IsParameter { get; set; }
}

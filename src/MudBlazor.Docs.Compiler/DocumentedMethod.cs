// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents documentation for a method.
/// </summary>
[DebuggerDisplay("({ReturnTypeName}) {Name}: {Summary}")]
public class DocumentedMethod
{
    public string Name { get; set; }
    public string Key { get; set; }
    public Dictionary<string, DocumentedParameter> Parameters { get; set; } = [];
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public bool IsPublic { get; set; }
    public bool IsProtected { get; set; }
    public Type ReturnType { get; set; }
    public string ReturnTypeName { get; set; }
    public string ReturnTypeFullName { get; set; }
    public bool IsEmpty => string.IsNullOrEmpty(Summary) && string.IsNullOrEmpty(Remarks);
}

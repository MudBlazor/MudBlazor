// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a documented parameter for a method.
/// </summary>
[DebuggerDisplay("({TypeName}) {Name}: {Summary}")]
public sealed class DocumentedField
{
    public string Key { get; set; }
    public string Name { get; set; }
    public Type Type { get; set; }
    public string TypeFullName { get; set; }
    public string TypeName { get; set; }
    public string Summary { get; set; }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

#nullable enable

/// <summary>
/// Represents documentation for a method.
/// </summary>
public class DocumentedMethod : DocumentedMember
{
    /// <summary>
    /// The parameters for this method.
    /// </summary>
    public Dictionary<string, DocumentedParameter> Parameters { get; set; } = [];

    /// <summary>
    /// The XML documentation for what this method returns.
    /// </summary>
    public string? Returns { get; set; }
}

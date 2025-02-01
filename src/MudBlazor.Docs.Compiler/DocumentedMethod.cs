// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents documentation for a method.
/// </summary>
public class DocumentedMethod : DocumentedMember
{
    /// <summary>
    /// The parameters for this method.
    /// </summary>
    public List<DocumentedParameter> Parameters { get; set; } = [];

    /// <summary>
    /// The XML documentation for what this method returns.
    /// </summary>
    public string? Returns { get; set; }
}

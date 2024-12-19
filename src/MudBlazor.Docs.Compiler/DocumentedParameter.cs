// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// A parameter for a method.
/// </summary>
[DebuggerDisplay("({Type.Name}) {Name}: {Summary}")]
public sealed class DocumentedParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    public required string? Name { get; init; }

    /// <summary>
    /// The type of this member.
    /// </summary>
    public required Type Type { get; init; }

    /// <summary>
    /// The XML documentation for this parameter.
    /// </summary>
    public required string Summary { get; init; } = string.Empty;
}

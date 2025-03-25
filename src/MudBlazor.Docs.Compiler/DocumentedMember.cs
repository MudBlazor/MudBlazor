// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a base class for designing documented members.
/// </summary>
[DebuggerDisplay("({Type?.Name}) {Name}: {Summary}")]
public abstract class DocumentedMember
{
    /// <summary>
    /// The category of the member.
    /// </summary>
    /// <remarks>
    /// This value comes from the <see cref="CategoryAttribute"/> applied to the member.
    /// </remarks>
    public string? Category { get; init; }

    /// <summary>
    /// The type which defines this member.
    /// </summary>
    public Type? DeclaringType { get; init; }

    /// <summary>
    /// The type which defines this member.
    /// </summary>
    public DocumentedType? DeclaringDocumentedType { get; set; }

    /// <summary>
    /// Whether this member is only visible to inheritors.
    /// </summary>
    public bool IsProtected { get; init; }

    /// <summary>
    /// The name of this member.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The order of this member relative to other members.
    /// </summary>
    public int Order { get; init; } = int.MaxValue;

    /// <summary>
    /// The unique key for this member in dictionaries.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string? Remarks { get; init; }

    /// <summary>
    /// The type of this member.
    /// </summary>
    public Type? Type { get; init; }
}

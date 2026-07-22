// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

#nullable enable

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
    public string? Category { get; set; }

    /// <summary>
    /// The type which defines this member.
    /// </summary>
    public Type? DeclaringType { get; set; }

    /// <summary>
    /// The name of the type which defines this member.
    /// </summary>
    public string? DeclaringTypeFullName { get; set; }

    /// <summary>
    /// Whether this member is only visible to inheritors.
    /// </summary>
    public bool IsProtected { get; set; }

    /// <summary>
    /// The name of this member.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The order of this member relative to other members.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// The unique key for this member in dictionaries.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The unique key for this member in XML documentation.
    /// </summary>
    public string? XmlKey { get; set; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// The type of this member.
    /// </summary>
    public Type? Type { get; set; }
}

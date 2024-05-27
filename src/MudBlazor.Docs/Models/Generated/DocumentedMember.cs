// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents a documented member of a type.
/// </summary>
[DebuggerDisplay("({ReturnType}) {Name}: {Summary}")]
public abstract class DocumentedMember
{
    /// <summary>
    /// The category of the member.
    /// </summary>
    /// <remarks>
    /// This value comes from the <see cref="CategoryAttribute"/> applied to the member.
    /// </remarks>
    public string Category { get; set; } = "General";

    /// <summary>
    /// The type which defines this member.
    /// </summary>
    public string? DeclaringTypeName { get; set; }

    /// <summary>
    /// The user-facing name of the declaring type.
    /// </summary>
    public string? DeclaringTypeFriendlyName { get; set; }

    /// <summary>
    /// The declaring type for this member.
    /// </summary>
    public DocumentedType? DeclaringType
    {
        get
        {
            if (string.IsNullOrEmpty(DeclaringTypeName))
            {
                return null;
            }
            var key = DeclaringTypeName;
            var genericsStart = DeclaringTypeName.IndexOf('[');
            if (genericsStart != -1)
            {
                key = DeclaringTypeName.Substring(0, genericsStart);
            }
            if (ApiDocumentation.Types.TryGetValue(key, out var type))
            {
                return type;
            }
            return null;
        }
    }

    /// <summary>
    /// The link to the docs for the declaring type.
    /// </summary>
    public string? DeclaringTypeApiUrl => $"/api/{DeclaringTypeName}";

    /// <summary>
    /// Whether this member is only visible to inheritors.
    /// </summary>
    public bool IsProtected { get; set; }

    /// <summary>
    /// The name of this member.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The order of this member relative to other members.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The type of this member.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// The user-facing name of this member's type.
    /// </summary>
    public string? TypeFriendlyName { get; set; }
}

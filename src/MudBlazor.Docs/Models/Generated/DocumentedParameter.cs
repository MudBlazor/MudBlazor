// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents a documented parameter for a method.
/// </summary>
public sealed class DocumentedParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The name of the type of this member.
    /// </summary>
    public string TypeName { get; set; } = "";

    /// <summary>
    /// The user-facing name of this member's type.
    /// </summary>
    public string TypeFriendlyName { get; set; } = "";

    /// <summary>
    /// The type of this member.
    /// </summary>
    public DocumentedType Type => ApiDocumentation.GetType(TypeName);

    /// <summary>
    /// The XML documentation for this parameter.
    /// </summary>
    public string Summary { get; set; } = "";
}

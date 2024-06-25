// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents documentation for a property.
/// </summary>
public sealed class DocumentedProperty : DocumentedMember
{
    /// <summary>
    /// Whether this property is a parameter.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the <see cref="ParameterAttribute"/> is applied to this property.
    /// </remarks>
    public bool IsParameter { get; set; }
}

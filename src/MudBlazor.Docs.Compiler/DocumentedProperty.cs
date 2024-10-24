﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

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

    /// <summary>
    /// The <see cref="EventHandler"/> called when this property changes.
    /// </summary>
    /// <remarks>
    /// When set, this property can be bound via <c>@bind-[Property]</c> in Razor.
    /// </remarks>
    public DocumentedEvent? ChangeEvent { get; set; }
}

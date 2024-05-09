// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents a set of XML documentation for MudBlazor types.
/// </summary>
public static partial class ApiDocumentation
{
    /// <summary>
    /// The types which have documentation.
    /// </summary>
    public static FrozenDictionary<string, DocumentedType> Types { get; private set; }
}

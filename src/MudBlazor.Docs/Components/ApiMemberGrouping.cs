// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Components;

/// <summary>
/// Indicates the method that properties for a type are grouped.
/// </summary>
public enum ApiMemberGrouping
{
    /// <summary>
    /// Properties are grouped by category.
    /// </summary>
    Categories,

    /// <summary>
    /// Properties are grouped by their declaring type.
    /// </summary>
    Inheritance,

    /// <summary>
    /// Properties are not grouped.
    /// </summary>
    None
}

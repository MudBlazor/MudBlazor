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
/// The kind of members to display in a member table.
/// </summary>
public enum ApiMemberTableMode
{
    /// <summary>
    /// No items will be displayed.
    /// </summary>
    None,

    /// <summary>
    /// Only properties will be displayed.
    /// </summary>
    Properties,

    /// <summary>
    /// Only methods will be displayed.
    /// </summary>
    Methods,

    /// <summary>
    /// Only fields will be displayed.
    /// </summary>
    Fields,

    /// <summary>
    /// Only events will be displayed.
    /// </summary>
    Events,
}

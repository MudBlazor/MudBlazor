// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the types of selections allowed.
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// One selection is allowed at a time.
    /// </summary>
    [Description("single-selection")]
    SingleSelection,

    /// <summary>
    /// More than one selection is allowed.
    /// </summary>
    [Description("multi-selection")]
    MultiSelection,

    /// <summary>
    /// One selection is toggled.
    /// </summary>
    [Description("toggle-selection")]
    ToggleSelection
}

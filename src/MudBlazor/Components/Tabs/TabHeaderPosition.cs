// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

#nullable enable
namespace MudBlazor;

/// <summary>
/// The location of the <see cref="MudTabs.Header"/>
/// </summary>
public enum TabHeaderPosition
{
    /// <summary>
    /// Additional content is placed after the first tab.
    /// </summary>
    [Description("after")]
    After,

    /// <summary>
    /// Additional content is placed before the first tab.
    /// </summary>
    [Description("before")]
    Before,

    /// <summary>
    /// No additional content is shown.
    /// </summary>
    [Description("none")]
    None,
}

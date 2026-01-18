// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// The information about a requested activity when <see cref="MudTabs.OnPreviewInteraction"/> occurs.
/// </summary>
public class TabInteractionEventArgs
{
    /// <summary>
    /// The index of the panel being activated.
    /// </summary>
    public int PanelIndex { get; set; }

    /// <summary>
    /// The type of interaction which occurred.
    /// </summary>
    public TabInteractionType InteractionType { get; set; }

    /// <summary>
    /// Prevents the tab from being activated.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>. When <c>true</c>, the attempt to activate the tab is prevented.
    /// </remarks>
    public bool Cancel { get; set; }
}

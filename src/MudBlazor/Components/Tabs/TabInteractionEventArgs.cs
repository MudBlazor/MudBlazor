// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class TabInteractionEventArgs
{
    public int PanelIndex { get; set; }
    public TabInteractionType InteractionType { get; set; }
    public bool Cancel { get; set; }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MudBlazor;

internal interface IExpansionPanelManager
{
    Task AddPanelAsync(MudExpansionPanel panel);

    void RemovePanel(MudExpansionPanel panel);

    Task NotifyPanelsChanged(MudExpansionPanel panel);
}

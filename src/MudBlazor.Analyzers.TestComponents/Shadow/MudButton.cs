// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Analyzers.TestComponents.Shadow
{
    /// <summary>
    /// Shares its short name with <see cref="MudBlazor.MudButton"/> so the analyzer tests can prove that
    /// migration hints are matched on the component's type identity, not on its display tag name.
    /// </summary>
    public class MudButton : MudComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}

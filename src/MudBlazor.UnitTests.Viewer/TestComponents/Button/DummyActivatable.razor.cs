// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;

namespace MudBlazor.UnitTests;

public partial class DummyActivatable : ComponentBase, IActivatable
{
#nullable enable
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public int ActivationCount { get; private set; }

    public void Activate(object activator, MouseEventArgs args) => ActivationCount++;
}

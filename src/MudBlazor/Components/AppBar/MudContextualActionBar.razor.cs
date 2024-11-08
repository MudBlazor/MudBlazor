// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor;

#nullable enable

public partial class MudContextualActionBar : MudAppBar
{
    private readonly ParameterState<bool> _visibleState;

    private new bool Contextual { get; set; }

    private RenderFragment ContextualContent => base.BuildRenderTree;

    /// <summary>
    /// Determines if the action bar is visible.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Overlay.Behavior)]
    public bool Visible { get; set; }

    /// <summary>
    /// Occurs when <see cref="Visible"/> changes.
    /// </summary>
    /// <remarks>
    /// This event is triggered when the visibility of the action bar changes.
    /// </remarks>
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    public MudContextualActionBar()
    {
        using var registerScope = CreateRegisterScope();
        _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
            .WithParameter(() => Visible)
            .WithEventCallback(() => VisibleChanged);
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudBlazor;

/// <summary>
/// Renders its child content only when <see cref="UpdateCount"/> changes, so a host's internal state changes do not rebuild that content.
/// </summary>
/// <remarks>
/// <see cref="MudTooltip"/> and <see cref="MudSelect{T}"/> increment the count when their own parent renders them.
/// </remarks>
public class MudTooltipChildContainer : IComponent
{
    private int _lastUpdateCount;
    private RenderHandle _renderHandle;

    /// <summary>
    /// The child content to render.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The update count of the parent component.
    /// </summary>
    [Parameter, EditorRequired]
    public int UpdateCount { get; set; }

    /// <inheritdoc />
    void IComponent.Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    /// <inheritdoc />
    Task IComponent.SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        var changed = UpdateCount != _lastUpdateCount;
        _lastUpdateCount = UpdateCount;
        // Only recalculate if changed
        if (changed)
        {
            _renderHandle.Render(BuildRenderTree);
        }

        return Task.CompletedTask;
    }

    private void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
}

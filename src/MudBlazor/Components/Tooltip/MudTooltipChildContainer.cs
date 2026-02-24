// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudBlazor;

/// <summary>
/// This component is used to prevent re-rendering of the child content when the tooltip's internal state changes.
/// It only re-renders when the parent component of the tooltip re-renders (signaled by UpdateCount).
/// </summary>
public class MudTooltipChildContainer : ComponentBase
{
    private int _lastUpdateCount;

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
    protected override void OnInitialized() => _lastUpdateCount = UpdateCount;

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        var changed = UpdateCount != _lastUpdateCount;
        _lastUpdateCount = UpdateCount;
        return changed;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
}

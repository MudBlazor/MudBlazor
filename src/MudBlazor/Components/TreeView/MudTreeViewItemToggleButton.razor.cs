// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor;

/// <summary>
/// Toggles the expansion state of a <see cref="MudTreeViewItem{T}"/>.
/// </summary>
/// <seealso cref="MudTreeView{T}"/>
/// <seealso cref="MudTreeViewItem{T}"/>
public partial class MudTreeViewItemToggleButton : MudComponentBase
{
    private readonly ParameterState<bool> _expandedState;

    public MudTreeViewItemToggleButton()
    {
        using var registerScope = CreateRegisterScope();
        _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
            .WithParameter(() => Expanded)
            .WithEventCallback(() => ExpandedChanged);
    }

    protected string Classname =>
        new CssBuilder(Class)
            .AddClass("mud-treeview-item-expand-button")
            .AddClass("mud-treeview-item-arrow-expand", !Loading)
            .AddClass("mud-transform", _expandedState.Value && !Loading)
            .AddClass("mud-treeview-item-arrow-load", Loading)
            .Build();

    /// <summary>
    /// Shows this button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Visible { get; set; }

    /// <summary>
    /// Prevents the user from interacting with this button.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether this button is in the "expanded" state.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Expanded { get; set; }

    /// <summary>
    /// Displays the loading icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>. Typically used when time is required to load child items.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Loading { get; set; }

    /// <summary>
    /// Occurs when <see cref="Expanded"/>.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// The icon shown when in the "loading" state.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.Loop"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

    /// <summary>
    /// The color of the loading icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public Color LoadingIconColor { get; set; } = Color.Default;

    /// <summary>
    /// The expand/collapse icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public string ExpandedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The color of the expand/collapse icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public Color ExpandedIconColor { get; set; } = Color.Default;

    private Task ToggleAsync()
    {
        return _expandedState.SetValueAsync(!_expandedState.Value);
    }

    private void OnDoubleClick()
    {
        /* Don't do anything on purpose. Fixes #9419 */
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor;

public partial class MudTreeViewItemToggleButton : MudComponentBase
{

    protected string Classname =>
        new CssBuilder(Class)
            .AddClass("mud-treeview-item-arrow-expand", !Loading)
            .AddClass("mud-transform", Expanded && !Loading)
            .AddClass("mud-treeview-item-arrow-load", Loading)
            .Build();

    /// <summary>
    /// If true, displays the button.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Visible { get; set; }

    /// <summary>
    /// Propagate disabled state to icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Determines when to flip the expanded icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Expanded { get; set; }

    /// <summary>
    /// If true, displays the loading icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Behavior)]
    public bool Loading { get; set; }

    /// <summary>
    /// Called whenever expanded changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// The loading icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

    /// <summary>
    /// The color of the loading. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public Color LoadingIconColor { get; set; } = Color.Default;

    /// <summary>
    /// The expand/collapse icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public string ExpandedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The color of the expand/collapse. It supports the theme colors.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.TreeView.Appearance)]
    public Color ExpandedIconColor { get; set; } = Color.Default;

    private Task ToggleAsync()
    {
        Expanded = !Expanded;
        return ExpandedChanged.InvokeAsync(Expanded);
    }

    private void OnDoubleClick()
    {
        /* Don't do anything on purpose. Fixes #9419 */
    }
}

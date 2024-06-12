// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents an inheritance tree for a documented type.
/// </summary>
public partial class ApiTypeHierarchy
{
    /// <summary>
    /// The type to show inheritance for.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The root of the type hierarchy.
    /// </summary>
    public IReadOnlyCollection<DocumentedTypeTreeItem>? Root { get; set; }

    /// <summary>
    /// The selected type.
    /// </summary>
    public DocumentedTypeTreeItem? SelectedType { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Type == null)
        {
            return;
        }

        SelectedType = new DocumentedTypeTreeItem()
        {
            ApiUrl = Type.ApiUrl,
            Expanded = false,
            Name = Type.NameFriendly,
        };
        var root = new List<DocumentedTypeTreeItem>() { SelectedType };
        // Walk up the hierarchy to build the tree
        var parent = Type.BaseType;
        while (parent != null)
        {
            root[0] = new DocumentedTypeTreeItem()
            {
                ApiUrl = parent.ApiUrl,
                Children = [root[0]],
                Expanded = true,
                Name = parent.NameFriendly
            };
            if (parent.BaseType != null)
            {
                parent = parent.BaseType;
            }
            else
            {
                root[0] = new DocumentedTypeTreeItem()
                {
                    ApiUrl = null,
                    Children = [root[0]],
                    Expanded = true,
                    Name = parent.BaseTypeName
                };
                break;
            }
        }
        // Now check for types inheriting from this type
        foreach (var descendant in ApiDocumentation.Types.Values.OrderBy(type => type.Name).Where(type => type.BaseTypeName == Type.Name))
        {
            SelectedType.Children.Add(new()
            {
                ApiUrl = descendant.ApiUrl,
                Name = descendant.NameFriendly,
            });
        }
        // Finally, flag the root
        root[0].IsRoot = true;
        // Set the items
        Root = new ReadOnlyCollection<DocumentedTypeTreeItem>(root);
        StateHasChanged();
    }

    [Inject]
    private NavigationManager? Browser { get; set; }

    /// <summary>
    /// Occurs when a type has been clicked.
    /// </summary>
    /// <param name="item"></param>
    public void OnTypeClicked(DocumentedTypeTreeItem item)
    {
        if (!string.IsNullOrEmpty(item.ApiUrl))
        {
            Browser?.NavigateTo(item.ApiUrl);
        }
    }

    /// <summary>
    /// Occurs when a node is expanded or collapsed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="expanded"></param>
    public void OnExpandedChanged(DocumentedTypeTreeItem item, bool expanded)
    {
        item.Expanded = expanded;
        StateHasChanged();
    }

    /// <summary>
    /// Represents a node in a documented type tree view.
    /// </summary>
    [DebuggerDisplay("{Name}={ApiUrl}, Children={Children.Count}")]
    public class DocumentedTypeTreeItem
    {
        public bool IsRoot { get; set; }
        public string? Name { get; set; }
        public string? ApiUrl { get; set; }
        public bool Expanded { get; set; }
        public List<DocumentedTypeTreeItem> Children { get; set; } = [];
    }
}

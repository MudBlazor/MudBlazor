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
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string? TypeName { get; set; }

    /// <summary>
    /// The type to display members for.
    /// </summary>
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The root of the type hierarchy.
    /// </summary>
    public IReadOnlyCollection<TreeItemData<DocumentedType>>? Root { get; set; }

    /// <summary>
    /// The selected type.
    /// </summary>
    public DocumentedType? SelectedType { get; set; }

    protected override void OnParametersSet()
    {
        if (Type == null || Type.Name != TypeName)
        {
            Type = ApiDocumentation.GetType(TypeName);
            SelectedType = Type;

            // Start with the current type
            var primaryItem = new TreeItemData<DocumentedType>
            {
                Text = Type.Name,
                Selected = true,
                Expanded = false,
                Value = Type,
                Children = [],
            };
            var root = new List<TreeItemData<DocumentedType>>() { primaryItem };
            // Walk up the hierarchy to build the tree
            var parent = Type.BaseType;
            while (parent != null)
            {
                root[0] = new TreeItemData<DocumentedType>()
                {
                    Children = [root[0]],
                    Expanded = true,
                    Text = parent.NameFriendly,
                    Value = parent
                };
                if (parent.BaseType != null)
                {
                    parent = parent.BaseType;
                }
                else
                {
                    root[0] = new TreeItemData<DocumentedType>()
                    {
                        Children = [root[0]],
                        Expanded = true,
                        Text = parent.BaseTypeName,
                        Value = new DocumentedType() { Name = "Root" }
                    };
                    break;
                }
            }
            // Now check for types inheriting from this type
            foreach (var descendant in ApiDocumentation.Types.Values.OrderBy(type => type.Name).Where(type => type.BaseTypeName == Type.Name))
            {
                primaryItem?.Children?.Add(new()
                {
                    Children = [],
                    Text = descendant.NameFriendly,
                    Value = descendant
                });
            }
            // Set the items
            Root = new ReadOnlyCollection<TreeItemData<DocumentedType>>(root);
        }
    }

    [Inject]
    private NavigationManager? Browser { get; set; }

    /// <summary>
    /// Occurs when a type has been clicked.
    /// </summary>
    /// <param name="item"></param>
    public void OnTypeClicked(TreeItemData<DocumentedType> item)
    {
        if (item.Value != null && !string.IsNullOrEmpty(item.Value.ApiUrl))
        {
            Browser?.NavigateTo(item.Value.ApiUrl);
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public List<DocumentedTypeTreeItem> Root { get; set; } = [];

    /// <summary>
    /// The selected type.
    /// </summary>
    public DocumentedTypeTreeItem? SelectedType { get; set; }

    protected override void OnParametersSet()
    {
        if (Type != null)
        {
            SelectedType = new DocumentedTypeTreeItem()
            {
                ApiUrl = Type.ApiUrl,
                Expanded = false,
                Name = Type.Name,
            };
            Root = [SelectedType];
            // Walk up the hierarchy to build the tree
            var parent = Type.BaseType;
            while (parent != null)
            {
                Root[0] = new DocumentedTypeTreeItem()
                {
                    ApiUrl = parent.ApiUrl,
                    Children = [Root[0]],
                    Expanded = true,
                    Name = parent.Name
                };
                if (parent.BaseType != null)
                {
                    parent = parent.BaseType;
                }
                else
                {
                    Root[0] = new DocumentedTypeTreeItem()
                    {
                        ApiUrl = null,
                        Children = [Root[0]],
                        Expanded = true,
                        Name = parent.BaseTypeName
                    };
                    break;
                }
            }
            // Now check for types inheriting from this type
            foreach (var descendant in ApiDocumentation.Types.Values.Where(type => type.BaseTypeName == Type.Name))
            {
                SelectedType.Children.Add(new()
                {
                    ApiUrl = descendant.ApiUrl,
                    Name = descendant.Name,
                });
            }
            StateHasChanged();
        }
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
    /// Represents a node in a documented type tree view.
    /// </summary>
    [DebuggerDisplay("{Name}={ApiUrl}, Children={Children.Count}")]
    public class DocumentedTypeTreeItem
    {
        public string? Name { get; set; }
        public string? ApiUrl { get; set; }
        public bool Expanded { get; set; }
        public bool IsMudBlazorType => Name != null && Name.StartsWith("Mud");
        public string? Icon => IsMudBlazorType ? @Icons.Custom.Brands.MudBlazor : null;
        public Color IconColor => IsMudBlazorType ? Color.Primary : Color.Default;
        public List<DocumentedTypeTreeItem> Children { get; set; } = [];
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Pages.Api;

#nullable enable

/// <summary>
/// Represents a page for viewing the members of a documented type.
/// </summary>
public partial class Api
{
    /// <summary>
    /// The service for managing menus.
    /// </summary>
    [Inject]
    public IMenuService? MenuService { get; set; }

    /// <summary>
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    public string? TypeName { get; set; }

    /// <summary>
    /// The name of the component associated with this type.
    /// </summary>
    /// <remarks>
    /// The friendly name for the type, if it is a MudBlazor component.  For example: the type
    /// <c>StackedBar</c> will return <c>Stacked Bar Chart</c>.
    /// </remarks>
    public string? ComponentName { get; set; }

    /// <summary>
    /// The name at the top of the page.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Shows the inheritance hierarchy.
    /// </summary>
    public static bool ShowInheritance => true;

    /// <summary>
    /// The type being displayed.
    /// </summary>
    public DocumentedType? DocumentedType { get; set; }

    protected override void OnParametersSet()
    {
        if (DocumentedType == null || DocumentedType.Name != TypeName)
        {
            DocumentedType = ApiDocumentation.GetType(TypeName);
            ComponentName = MenuService!.GetComponentName(TypeName!) ?? DocumentedType?.NameFriendly;
            if (DocumentedType == null)
            {
                Title = TypeName + " Not Found";
            }
            else if (DocumentedType.IsComponent)
            {
                Title = DocumentedType.NameFriendly + " Component";
            }
            else if (DocumentedType.BaseTypeName == "Enum")
            {
                Title = DocumentedType.NameFriendly + " Enumeration";
            }
            else
            {
                Title = DocumentedType.NameFriendly + " Class";
            }
        }
    }
}

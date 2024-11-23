// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Components;

/// <summary>
/// A title and description for a docs page.
/// </summary>
public sealed partial class DocsPageHeader
{
    /// <summary>
    /// The parent documentation page.
    /// </summary>
    [CascadingParameter]
    public DocsPage Page { get; set; }

    /// <summary>
    /// The service for navigating to other pages.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// The service for building menus.
    /// </summary>
    [Inject]
    public IMenuService MenuService { get; set; }

    /// <summary>
    /// The title of this page.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// The subtitle of this page.
    /// </summary>
    [Parameter]
    public string SubTitle { get; set; }

    /// <summary>
    /// The description of this page.
    /// </summary>
    [Parameter]
    public RenderFragment Description { get; set; }

    /// <summary>
    /// The special headers, if any, for this page.
    /// </summary>
    [Parameter]
    public RenderFragment SpecialHeaderContent { get; set; }

    // Will be replaced by DocumentedType
    public MudComponent _component;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // If there is no subtitle set, but we have a component summary, use the component summary
        if (string.IsNullOrEmpty(SubTitle) && Page.Type != null && !string.IsNullOrEmpty(Page.Type.Summary))
        {
            SubTitle = Page.Type.Summary;
        }
    }

    /// <summary>
    /// Gets the title for this page.
    /// </summary>
    private string GetTitle() => $"{Title} - MudBlazor";

    /// <summary>
    /// Gets the subtitle for this page.
    /// </summary>
    /// <returns></returns>
    private string GetSubTitle()
    {
        if (string.IsNullOrEmpty(SubTitle))
            return "";
        return SubTitle.TrimEnd('.') + ".";
    }

    /// <summary>
    /// Gets the keywords for this page.
    /// </summary>
    /// <returns></returns>
    private string GetKeywords()
    {
        var keywords = new HashSet<string>
        {
            Title,
            "mudblazor",
            "blazor",
            "component",
            "material design"
        };
        if (Page.Type != null)
        {
            keywords.Add(Page.Type.Name);
            keywords.Add(Page.Type.Name?.Replace("Mud", ""));
        }
        return string.Join(", ", keywords);
    }

    /// <summary>
    /// Gets the canonical URL for this page.
    /// </summary>
    /// <returns></returns>
    private string GetCanonicalUri()
    {
        return NavigationManager.Uri.Replace(NavigationManager.BaseUri, "https://mudblazor.com/");
    }

    /// <summary>
    /// Gets whether this page has an associated example page.
    /// </summary>
    /// <returns>When <c>true</c>, a menu item exists for this type.</returns>
    public bool HasExamplePage()
    {
        return MenuService.Components.Any(menu => menu.GroupComponents == null && menu.Link == Page.TypeName)
            || MenuService.Components.Any(menu => menu.GroupComponents != null && menu.GroupComponents.Any(subMenu => subMenu.Link == Page.TypeName));
    }
}

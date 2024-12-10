// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Components;

/// <summary>
/// A title and description for a docs page.
/// </summary>
public sealed partial class DocsPageHeader
{
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
    /// The name of the component associated with this page.
    /// </summary>
    /// <remarks>
    /// Should be the name of a component, such as <c>nameof(MudAlert)</c>.  When set, the
    /// <see cref="DocumentedType"/> property will contain all the documentation for this 
    /// component.
    /// </remarks>
    [Parameter]
    public string Component { get; set; }

    /// <summary>
    /// Whether this page shows API documentation.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool IsApi { get; set; }

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

    /// <summary>
    /// The documentation for this page's component.
    /// </summary>
    public DocumentedType DocumentedType { get; set; }

    /// <summary>
    /// The example page for this type.
    /// </summary>
    public MudComponent Example { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Has the page changed?  Or is this the first render?
        if (Component != null && (DocumentedType == null || DocumentedType.Name != Component))
        {
            // Get the documentation for this component
            DocumentedType = ApiDocumentation.GetType(Component);
            // Look for an example page for this type
            Example = DocumentedType == null ? null : MenuService.GetExample(DocumentedType);
            // If there is no subtitle set, but we have a component summary, use the component summary
            if (string.IsNullOrEmpty(SubTitle) && !string.IsNullOrEmpty(DocumentedType.Summary))
            {
                SubTitle = DocumentedType.Summary;
            }
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
            Component,
            Component?.Replace("Mud", ""),
            "mudblazor",
            "blazor",
            "component",
            "material design"
        };
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
}

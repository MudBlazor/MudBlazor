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
    private string keywords;

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
    /// The keywords for this page.
    /// </summary>
    [Parameter]
    public string Keywords
    {
        get => keywords;
        set
        {
            keywords = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public bool DisableApiHeader { get; set; }

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

    [Parameter] public string ComponentLink { get; set; }

    /// <summary>
    /// The documentation for this page's component.
    /// </summary>
    public DocumentedType DocumentedType { get; set; }

    public Type _componentType;
    public MudComponent _parentComponent;
    public MudComponent _component;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Has the page changed?  Or is this the first render?
        if (Component != null && (DocumentedType == null || DocumentedType.Name != Component))
        {
            // Get the documentation for this component
            DocumentedType = ApiDocumentation.GetType(Component);
            // If there is no subtitle set, but we have a component, use the component's summary
            if (string.IsNullOrEmpty(SubTitle))
            {
                SubTitle = DocumentedType.Summary;
            }
        }
    }

    /// <summary>
    /// Gets the title for this page.
    /// </summary>
    private string GetTitle() => $"{Title} - MudBlazor";

    private string GetSubTitle()
    {
        if (string.IsNullOrEmpty(SubTitle))
            return "";
        return SubTitle.TrimEnd('.') + ".";
    }

    private string GetKeywords()
    {
        var keywords = new HashSet<string>(Regex.Split(Keywords ?? "", @",\s"));
        keywords.Add(Title);
        keywords.Add(Component);
        keywords.Add(Component?.Replace("Mud", ""));
        keywords.Add("mudblazor");
        keywords.Add("blazor");
        keywords.Add("component");
        keywords.Add("material design");
        keywords.Remove("");
        keywords.Remove(null);
        return string.Join(", ", keywords);
    }

    private string GetCanonicalUri()
    {
        return NavigationManager.Uri.Replace(NavigationManager.BaseUri, "https://mudblazor.com/");
    }
}

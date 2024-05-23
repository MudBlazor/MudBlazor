// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Utilities;

namespace MudBlazor.Docs.Components;

public partial class SectionHeader
{
    [CascadingParameter] private DocsPage DocsPage { get; set; }

    [CascadingParameter] private SectionSubGroups SubGroup { get; set; }
    [CascadingParameter] private DocsPageSection Section { get; set; }


    protected string Classname =>
        new CssBuilder("docs-section-header")
            .AddClass("docs-section-anchor", !string.IsNullOrWhiteSpace(Title))
            .AddClass(Class)
            .Build();

    [Parameter] public string Class { get; set; }

    [Parameter] public string Title { get; set; }
    [Parameter] public bool HideTitle { get; set; }
    [Parameter] public RenderFragment SubTitle { get; set; }
    [Parameter] public RenderFragment Description { get; set; }

    public DocsSectionLink SectionInfo { get; set; }

    public ElementReference SectionReference;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (DocsPage == null || string.IsNullOrWhiteSpace(Title))
        {
            return;
        }

        var parentTitle = DocsPage.GetParentTitle(Section) ?? string.Empty;
        if (string.IsNullOrEmpty(parentTitle) == false)
        {
            parentTitle += '-';
        }

        var id = (parentTitle + Title).Replace(" ", "-").ToLowerInvariant();

        SectionInfo = new DocsSectionLink { Id = id, Title = Title, };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && DocsPage != null && !string.IsNullOrWhiteSpace(Title))
        {
            await DocsPage.AddSectionAsync(SectionInfo, Section);
        }
    }

    private string GetSectionId() => SectionInfo?.Id ?? Guid.NewGuid().ToString();

    private Typo GetTitleTypo()
    {
        if (Section.Level >= 1)
        {
            return Typo.h6;
        }

        return Typo.h5;
    }
}

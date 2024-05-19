// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared;

#nullable enable
public partial class Appbar
{
    private bool _searchDialogOpen;
    private bool _searchDialogAutocompleteOpen;
    private int _searchDialogReturnedItemsCount;
    private string _badgeTextSoon = "coming soon";
    private MudAutocomplete<ApiLinkServiceEntry> _searchAutocomplete = null!;
    private DialogOptions _dialogOptions = new() { Position = DialogPosition.TopCenter, NoHeader = true };
    private readonly List<ApiLinkServiceEntry> _apiLinkServiceEntries =
    [
        new ApiLinkServiceEntry
        {
            Title = "Installation",
            Link = "getting-started/installation",
            SubTitle = "Get started with MudBlazor fast and easy."
        },

        new ApiLinkServiceEntry
        {
            Title = "Wireframes",
            Link = "getting-started/wireframes",
            SubTitle = "These small templates can be copied directly or just be used for inspiration."
        },

        new ApiLinkServiceEntry
        {
            Title = "Table",
            Link = "components/table",
            ComponentType = typeof(MudTable<T>),
            SubTitle = "A sortable, filterable table with multiselection and pagination."
        },

        new ApiLinkServiceEntry
        {
            Title = "Grid",
            Link = "components/grid",
            ComponentType = typeof(MudGrid),
            SubTitle = "The grid component helps keeping layout consistent across various screen resolutions and sizes."
        },

        new ApiLinkServiceEntry
        {
            Title = "Button",
            Link = "components/button",
            ComponentType = typeof(MudGrid),
            SubTitle = "A Material Design button for triggering an action or navigating to a link."
        },

        new ApiLinkServiceEntry
        {
            Title = "Card",
            Link = "components/card",
            ComponentType = typeof(MudCard),
            SubTitle = "Cards can contain actions, text, or media like images or graphics."
        },

        new ApiLinkServiceEntry
        {
            Title = "Dialog",
            Link = "components/dialog",
            ComponentType = typeof(MudDialog),
            SubTitle = "A dialog will overlay your current app content, providing the user with either information, a choice, or other tasks."
        },

        new ApiLinkServiceEntry
        {
            Title = "App Bar",
            Link = "components/appbar",
            ComponentType = typeof(MudAppBar),
            SubTitle = "App bar is used to display actions, branding, navigation and screen titles."
        },

        new ApiLinkServiceEntry
        {
            Title = "Navigation Menu",
            Link = "components/navmenu",
            ComponentType = typeof(MudNavMenu),
            SubTitle = "Nav menu provides a tree-like menu linking to the content on your site."
        }
    ];

    public bool IsSearchDialogOpen
    {
        get => _searchDialogOpen;
        set
        {
            _searchDialogAutocompleteOpen = default;
            _searchDialogReturnedItemsCount = default;
            _searchDialogOpen = value;
        }
    }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IApiLinkService ApiLinkService { get; set; } = null!;

    [Inject]
    private LayoutService LayoutService { get; set; } = null!;

    [Parameter]
    public EventCallback<MouseEventArgs> DrawerToggleCallback { get; set; }

    [Parameter]
    public bool DisplaySearchBar { get; set; } = true;

    private async void OnSearchResult(ApiLinkServiceEntry entry)
    {
        NavigationManager.NavigateTo(entry.Link);
        await Task.Delay(1000);
        await _searchAutocomplete.ClearAsync();
    }

    private string GetActiveClass(DocsBasePage page)
    {
        return page == LayoutService.GetDocsBasePage(NavigationManager.Uri) ? "mud-chip-text mud-chip-color-primary mx-1 px-3" : "mx-1 px-3";
    }

    private Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            // The user just opened the popover so show the most popular pages according to our analytics data as search results.
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(_apiLinkServiceEntries);
        }

        return ApiLinkService.Search(text);
    }

    private void OpenSearchDialog() => IsSearchDialogOpen = true;
}

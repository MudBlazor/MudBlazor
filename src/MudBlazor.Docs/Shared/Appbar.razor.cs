﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Shared;

public partial class Appbar
{
    [CascadingParameter] private MainLayout MainData { get; set; }
    [Parameter] public bool DisplaySearchBar { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IApiLinkService ApiLinkService { get; set; }

    [Inject] private INotificationService NotificationService { get; set; }
    
    MudAutocomplete<ApiLinkServiceEntry> _searchAutocomplete;

    private IDictionary<NotificationMessage,bool> _messages = null;
    private bool _newNotificationsAvailable = false;
    protected override async Task OnInitializedAsync()
    {
        _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
        _messages = await NotificationService.GetNotifications();
        await base.OnInitializedAsync();
    }

    private async Task MarkNotificationAsRead()
    {
        await NotificationService.MarkNotificationsAsRead();
        _newNotificationsAvailable = false;
    }

    private Task<IEnumerable<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // the user just clicked the autocomplete open, show the most popular pages as search result according to our analytics data
                // ordered by popularity
                return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(new[]
                {
                    new ApiLinkServiceEntry
                    {
                        Title = "Installation",
                        Link = "getting-started/installation",
                        SubTitle = "Getting started with MudBlazor fast and easy."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Wireframes",
                        Link = "getting-started/wireframes",
                        SubTitle =
                            "These small templates can be copied directly or just be used for inspiration."
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
                        SubTitle =
                            "The grid component helps keeping layout consistent across various screen resolutions and sizes."
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
                        SubTitle =
                            "A dialog will overlay your current app content, providing the user with either information, a choice, or other tasks."
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
                    },
                });
            }
            return ApiLinkService.Search(text);
    }

    private async void OnSearchResult(ApiLinkServiceEntry entry)
    {
        NavigationManager.NavigateTo(entry.Link);
        await Task.Delay(1000);
        await _searchAutocomplete.Clear();
    }

    private string GetActiveClass(DocsBasePage page)
    {
        string activeClass = "mud-chip-text mud-chip-color-primary mx-1 px-3";
        
        if ((NavigationManager.Uri.Contains("/api/") || NavigationManager.Uri.Contains("/components/")) && page == DocsBasePage.Docs)
        {
            return activeClass;
        }
        else if (NavigationManager.Uri.Contains("/getting-started/") && page == DocsBasePage.GettingStarted)
        {
            return activeClass;
        }
        else if (NavigationManager.Uri.Contains("/mud/") && page == DocsBasePage.DiscoverMore)
        {
            return activeClass;
        }
        else
        {
            return "mx-1 px-3";
        }
    }

    private enum DocsBasePage
    {
        Docs,
        GettingStarted,
        DiscoverMore
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Utilities;

namespace MudBlazor.Docs.Shared;

#nullable enable
public partial class Appbar : IDisposable
{
    private DocsBasePage _activePage;
    private bool _searchDialogOpen;
    private bool _searchDialogAutocompleteOpen;
    private int _searchDialogReturnedItemsCount;
    private MudAutocomplete<ApiLinkServiceEntry>? _searchBarAutocomplete;
    private MudAutocomplete<ApiLinkServiceEntry>? _searchDialogAutocomplete;
    private DialogOptions _dialogOptions = new() { Position = DialogPosition.TopCenter, NoHeader = true, CloseOnEscapeKey = true };
    private static readonly JsKeyModifier[] CtrlLeftKeyModifiers = [JsKeyModifier.ControlLeft];
    private static readonly JsKeyModifier[] CtrlRightKeyModifiers = [JsKeyModifier.ControlRight];

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

    protected override void OnInitialized()
    {
        UpdateActivePage(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
        base.OnInitialized();
    }

    private async Task OnSearchResult(ApiLinkServiceEntry? entry)
    {
        if (entry is null)
        {
            return;
        }

        NavigationManager.NavigateTo(entry.Link);
        await Task.Delay(1000);
        if (_searchBarAutocomplete is not null)
        {
            await _searchBarAutocomplete.ClearAsync();
        }

        if (_searchDialogAutocomplete is not null)
        {
            await _searchDialogAutocomplete.ClearAsync();
        }
    }

    private string GetActiveClass(DocsBasePage page)
    {
        return page == _activePage ? "mud-chip-text mud-chip-color-primary mx-1 px-3" : "mx-1 px-3";
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        if (UpdateActivePage(args.Location))
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private bool UpdateActivePage(string location)
    {
        var activePage = LayoutService.GetDocsBasePage(location);
        if (_activePage == activePage)
        {
            return false;
        }

        _activePage = activePage;
        return true;
    }

    private Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            // The user just opened the popover so show the most popular pages according to our analytics data as search results.
            return Task.FromResult(ApiLinkService.GetFeaturedEntries());
        }

        return ApiLinkService.Search(text);
    }

    private void OpenSearchDialog() => IsSearchDialogOpen = true;

    private async Task HandleSearchHotkeyAsync()
    {
        if (DisplaySearchBar && _searchBarAutocomplete is not null)
        {
            await _searchBarAutocomplete.FocusAsync();
            return;
        }

        OpenSearchDialog();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}

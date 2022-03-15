// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Services;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences.UserPreferences _userPreferences;

    public bool IsRTL { get; private  set; } = false;
    public bool IsDarkMode { get; private set; } = false;

    public MudTheme CurrentTheme { get; private set; }

    
    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }
    
    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkTheme;
            IsRTL = _userPreferences.RightToLeft;
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences {DarkTheme = IsDarkMode};
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
    }
    
    public event EventHandler MajorUpdateOccured;

    private  void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this,EventArgs.Empty);
    
    public async Task ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        _userPreferences.DarkTheme = IsDarkMode;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        _userPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();

    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccured();
    }
    
    public DocsBasePage GetDocsBasePage(string uri)
    {
        if (uri.Contains("/docs/") || uri.Contains("/api/") || uri.Contains("/components/") ||
            uri.Contains("/features/") || uri.Contains("/customization/") || uri.Contains("/utilities/"))
        {
            return DocsBasePage.Docs;
        }
        else if (uri.Contains("/getting-started/"))
        {
            return DocsBasePage.GettingStarted;
        }
        else if (uri.Contains("/mud/"))
        {
            return DocsBasePage.DiscoverMore;
        }
        else
        {
            return DocsBasePage.None;
        }
    }
}

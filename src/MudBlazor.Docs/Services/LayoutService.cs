﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Services;

using MudBlazor.Docs.Enums;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences.UserPreferences _userPreferences;
    private bool _systemPreferences;

    public bool IsRTL { get; private set; }
    public DarkLightMode DarkModeToggle = DarkLightMode.System;

    public bool IsDarkMode { get; private set; }

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
        _systemPreferences = isDarkModeDefaultTheme;
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkLightTheme switch
            {
                DarkLightMode.Dark => true,
                DarkLightMode.Light => false,
                DarkLightMode.System => isDarkModeDefaultTheme,
                _ => IsDarkMode
            };
            IsRTL = _userPreferences.RightToLeft;
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences { DarkLightTheme = DarkLightMode.System };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
    }

    public Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccurred();
        }
        return Task.CompletedTask;
    }

    public event EventHandler MajorUpdateOccurred;

    private void OnMajorUpdateOccurred() => MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);

    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = _systemPreferences;
                break;
        }

        _userPreferences.DarkLightTheme = DarkModeToggle;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccurred();
    }

    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        _userPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccurred();
    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccurred();
    }

    public DocsBasePage GetDocsBasePage(string uri)
    {
        if (uri.Contains("/docs/") || uri.Contains("/api/") || uri.Contains("/components/") ||
            uri.Contains("/features/") || uri.Contains("/customization/") || uri.Contains("/utilities/"))
        {
            return DocsBasePage.Docs;
        }

        if (uri.Contains("/getting-started/"))
        {
            return DocsBasePage.GettingStarted;
        }

        if (uri.Contains("/mud/"))
        {
            return DocsBasePage.DiscoverMore;
        }

        return DocsBasePage.None;
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Enums;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Services;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences.UserPreferences _userPreferences;
    private bool _systemDarkMode;

    /// <summary>
    /// Displays the layout right to left.
    /// </summary>
    public bool IsRTL { get; private set; }

    /// <summary>
    /// The user's preference that indirectly sets the mode through <see cref="IsDarkMode"/>.
    /// </summary>
    public DarkLightMode CurrentDarkLightMode { get; private set; }

    /// <summary>
    /// Determined in <see cref="UpdateDarkMode"/> and is what the UI actually displays out of the user preference and system preference.
    /// </summary>
    public bool IsDarkMode { get; private set; }

    /// <summary>
    /// Enables observation of the system theme change so we can update the dark/light mode.
    /// </summary>
    public bool ObserveSystemThemeChange { get; private set; }

    /// <summary>
    /// The MudBlazor theme that will be used.
    /// </summary>
    public MudTheme CurrentTheme { get; private set; }

    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    /// <summary>
    /// Occurs when a change happens that needs a UI refresh to be properly displayed.
    /// </summary>
    public event EventHandler MajorUpdateOccurred;

    private void OnMajorUpdateOccurred() => MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Updates the state of the date mode.
    /// </summary>
    /// <param name="systemMode">The known system mode which is used in <see cref="DarkLightMode.System"/>.</param>
    public void UpdateDarkMode(bool? systemMode = null)
    {
        if (systemMode.HasValue)
        {
            _systemDarkMode = systemMode.Value;
        }

        IsDarkMode = CurrentDarkLightMode switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            _ => _systemDarkMode,
        };
    }

    public async Task ApplyUserPreferencesAsync()
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();

        if (_userPreferences is null)
        {
            _userPreferences = new()
            {
                DarkLightTheme = DarkLightMode.System,
            };

            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
        else
        {
            IsRTL = _userPreferences.RightToLeft;
            CurrentDarkLightMode = _userPreferences.DarkLightTheme;
            UpdateDarkMode();
        }
    }

    public Task OnSystemModeChanged(bool newValue)
    {
        _systemDarkMode = newValue;
        OnMajorUpdateOccurred();
        return Task.CompletedTask;
    }

    public async Task CycleDarkLightModeAsync()
    {
        switch (CurrentDarkLightMode)
        {
            case DarkLightMode.System:
                CurrentDarkLightMode = DarkLightMode.Light;
                ObserveSystemThemeChange = false;
                break;

            case DarkLightMode.Light:
                CurrentDarkLightMode = DarkLightMode.Dark;
                ObserveSystemThemeChange = false;
                break;

            case DarkLightMode.Dark:
                CurrentDarkLightMode = DarkLightMode.System;
                ObserveSystemThemeChange = true;
                break;
        }

        UpdateDarkMode();

        _userPreferences.DarkLightTheme = CurrentDarkLightMode;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccurred();
    }

    public async Task ToggleRightToLeftAsync()
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

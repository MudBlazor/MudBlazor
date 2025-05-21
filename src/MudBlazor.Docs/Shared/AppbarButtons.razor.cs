// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Enums;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Shared;

/// <summary>
/// Code-behind for the AppbarButtons component, handling UI logic for theme and layout toggles, and notifications.
/// </summary>
public partial class AppbarButtons : IDisposable
{
    private IDictionary<NotificationMessage, bool> _messages = new Dictionary<NotificationMessage, bool>();
    private bool _newNotificationsAvailable;

    [Inject]
    private INotificationService NotificationService { get; set; } = null!;

    [Inject]
    private LayoutService LayoutService { get; set; } = null!;

    /// <summary>
    /// Gets the text for the RTL toggle button, indicating the next state.
    /// </summary>
    public string RtlButtonText => LayoutService.IsRTL ? "Switch to Left-to-right" : "Switch to Right-to-left";

    /// <summary>
    /// Gets the icon for the RTL toggle button.
    /// </summary>
    public string RtlButtonIcon => LayoutService.IsRTL ? @Icons.Material.Filled.FormatTextdirectionLToR : @Icons.Material.Filled.FormatTextdirectionRToL;

    /// <summary>
    /// Gets the text for the dark/light mode toggle button, indicating the next mode.
    /// </summary>
    public string DarkLightModeButtonText => LayoutService.CurrentDarkLightMode switch
    {
        DarkLightMode.Dark => "Switch to System mode",
        DarkLightMode.Light => "Switch to Dark mode",
        _ => "Switch to Light mode"
    };

    /// <summary>
    /// Gets the icon for the dark/light mode toggle button.
    /// </summary>
    public string DarkLightModeButtonIcon => LayoutService.CurrentDarkLightMode switch
    {
        DarkLightMode.Dark => Icons.Material.Rounded.AutoMode,
        DarkLightMode.Light => Icons.Material.Outlined.DarkMode,
        _ => Icons.Material.Filled.LightMode
    };

    private async Task MarkNotificationAsReadAsync()
    {
        await NotificationService.MarkNotificationsAsRead();
        _newNotificationsAvailable = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadNotificationsAsync();
        LayoutService.MajorUpdateOccurred += OnMajorLayoutUpdateOccurred;
        await base.OnInitializedAsync();
    }

    private async Task LoadNotificationsAsync()
    {
        _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
        _messages = await NotificationService.GetNotifications();
    }

    private void OnMajorLayoutUpdateOccurred(object sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    // It's good practice to unsubscribe from events to prevent memory leaks.
    public void Dispose()
    {
        LayoutService.MajorUpdateOccurred -= OnMajorLayoutUpdateOccurred;
        GC.SuppressFinalize(this);
    }
}

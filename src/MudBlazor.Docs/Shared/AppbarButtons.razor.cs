// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Enums;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Shared;

public partial class AppbarButtons
{
    private IDictionary<NotificationMessage, bool> _messages = null;
    private bool _newNotificationsAvailable;

    [Inject]
    private INotificationService NotificationService { get; set; } = null!;

    [Inject]
    private LayoutService LayoutService { get; set; } = null!;


    /// <summary>
    /// Gets the text for the RTL toggle button, indicating the next state.
    /// </summary>
    public string RtlButtonText => LayoutService.IsRTL ? "Left-to-right" : "Right-to-left";

    /// <summary>
    /// Gets the icon for the RTL toggle button.
    /// </summary>
    public string RtlButtonIcon => LayoutService.IsRTL ? @Icons.Material.Filled.FormatTextdirectionLToR : @Icons.Material.Filled.FormatTextdirectionRToL;

    /// <summary>
    /// Gets the text for the dark/light mode toggle button, indicating the next mode.
    /// </summary>
    public string DarkLightModeButtonText => LayoutService.CurrentDarkLightMode switch
    {
        DarkLightMode.Dark => "Auto mode",
        DarkLightMode.Light => "Dark mode",
        _ => "Light mode"
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
            _messages = await NotificationService.GetNotifications();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}

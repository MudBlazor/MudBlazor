// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Pages.Mud.Announcements;

public partial class AnnouncementOverviewPage
{
    private IDictionary<NotificationMessage, bool> _messages;

    [Inject] public INotificationService NotificationService { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _messages = await NotificationService.GetNotifications();
            await NotificationService.MarkNotificationsAsRead();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}

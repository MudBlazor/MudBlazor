// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Pages.Mud.Announcements;

public partial class AnnouncementOverviewPage
{
    private IDictionary<NotificationMessage, bool> _messages = null;

    [Inject] public INotificationService NotificationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _messages = await NotificationService.GetNotifications();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await NotificationService.MarkNotificationsAsRead();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}

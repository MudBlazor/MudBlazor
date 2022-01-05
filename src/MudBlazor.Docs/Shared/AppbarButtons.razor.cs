﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

namespace MudBlazor.Docs.Shared;

public partial class AppbarButtons
{
    [CascadingParameter] private MainLayout MainData { get; set; }

    [Inject] private INotificationService NotificationService { get; set; }
    
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
}

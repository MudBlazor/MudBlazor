// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Services.Notifications;

public interface INotificationService
{
    Task<bool> AreNewNotificationsAvailable();
    Task MarkNotificationsAsRead();
    Task MarkNotificationsAsRead(string id);

    Task<NotificationMessage> GetMessageById(string id);
    Task<IDictionary<NotificationMessage, bool>> GetNotifications();
    Task AddNotification(NotificationMessage message);
}

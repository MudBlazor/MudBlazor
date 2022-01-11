// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using MudBlazor.Docs.NotificationContent;

namespace MudBlazor.Docs.Services.Notifications;

public class InMemoryNotificationService : INotificationService
{
    private const string LocalStorageKey = "__notficationTimestamp";
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<InMemoryNotificationService> _logger;

    private readonly List<NotificationMessage> _messages;

    public InMemoryNotificationService(ILocalStorageService localStorageService,
        ILogger<InMemoryNotificationService> logger)
    {
        _localStorageService = localStorageService;
        _logger = logger;

        _messages = new List<NotificationMessage>();
    }

    private async Task<DateTime> GetLastReadTimestamp()
    {
        if (await _localStorageService.ContainKeyAsync(LocalStorageKey) == false)
        {
            return DateTime.MinValue;
        }
        else
        {
            var timestamp = await _localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
            return timestamp;
        }
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        var timestamp = await GetLastReadTimestamp();
        var entriesFound = _messages.Any(x => x.PublishDate > timestamp);

        return entriesFound;
    }

    public async Task MarkNotificationsAsRead()
    {
        await _localStorageService.SetItemAsync(LocalStorageKey, DateTime.UtcNow.Date);
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        var message = await GetMessageById(id);
        if (message == null) { return; }
        
        var timestamp = await _localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
        if (message.PublishDate > timestamp)
        {
            await _localStorageService.SetItemAsync(LocalStorageKey, message.PublishDate);
        }
        
    }

    public Task<NotificationMessage> GetMessageById(string id) =>
        Task.FromResult(_messages.FirstOrDefault((x => x.Id == id)));

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        var lastReadTimestamp = await GetLastReadTimestamp();
        var items = _messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }


    public void Preload()
    {
        _messages.Add(new NotificationMessage(
            "new-docs",
            "New Docs Page for you",
            "Here goes content to explain something",
            "Announcement",
            new DateTime(2021, 12, 31),
            "https://media.discordapp.net/attachments/814877828638179358/926959380115640330/Lory1-MERGED-WEB.jpg?width=491&height=687",
            new[]
            {
                new NotificationAuthor("The MudBlazor Team",
                    "https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png")
            }, typeof(NewDocsPageNotificationContent)));

        _messages.Add(new NotificationMessage(
            "cool-stuff",
            "cooler than yours",
            "Here goes content to explain something",
            "Announcement",
            new DateTime(2022, 01, 02),
            "https://media.discordapp.net/attachments/814877828638179358/926959380115640330/Lory1-MERGED-WEB.jpg?width=491&height=687",
            new[]
            {
                new NotificationAuthor("The MudBlazor Team",
                    "https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png")
            }, typeof(NewDocsPageNotificationContent)));
    }
}

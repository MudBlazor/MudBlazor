// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Services.KeyInterceptor.Mocks;

public class KeyInterceptorObserverMock : IKeyInterceptorObserver
{
    public string ElementId { get; }

    public List<(string elemendId, KeyboardEventArgs keyboardEventArgs)> Notifications { get; } = new();

    public KeyInterceptorObserverMock(string elementId)
    {
        ElementId = elementId;
    }

    public Task NotifyOnKeyDownAsync(KeyboardEventArgs args)
    {
        Notifications.Add((ElementId, args));

        return Task.CompletedTask;
    }

    public Task NotifyOnKeyUpAsync(KeyboardEventArgs args)
    {
        Notifications.Add((ElementId, args));

        return Task.CompletedTask;
    }
}

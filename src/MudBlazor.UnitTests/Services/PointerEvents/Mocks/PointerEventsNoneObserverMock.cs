// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Services.PointerEvents.Mocks;

public class PointerEventsNoneObserverMock : IPointerEventsNoneObserver
{
    public string ElementId { get; }

    public List<(string elemendId, EventArgs eventArgs)> Notifications { get; } = new();

    public PointerEventsNoneObserverMock(string elementId)
    {
        ElementId = elementId;
    }

    public Task NotifyOnPointerDownAsync(EventArgs args)
    {
        Notifications.Add((ElementId, args));
        return Task.CompletedTask;
    }

    public Task NotifyOnPointerUpAsync(EventArgs args)
    {
        Notifications.Add((ElementId, args));
        return Task.CompletedTask;
    }
}

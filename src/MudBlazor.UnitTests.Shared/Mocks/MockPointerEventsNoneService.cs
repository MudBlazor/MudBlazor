// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Shared.Mocks;

#nullable enable

public class MockPointerEventsNoneService : IPointerEventsNoneService
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task SubscribeAsync(IPointerEventsNoneObserver observer, PointerEventsNoneOptions options) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, PointerEventsNoneOptions options, IPointerDownObserver? pointerDown = null, IPointerUpObserver? pointerUp = null) => Task.CompletedTask;

    public Task UnsubscribeAsync(IPointerEventsNoneObserver observer) => Task.CompletedTask;

    public Task UnsubscribeAsync(string elementId) => Task.CompletedTask;
}

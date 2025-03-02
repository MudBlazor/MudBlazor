// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Shared.Mocks;

#nullable enable

public class MockOverlayPointerDownListenerFactory : IOverlayPointerDownListenerFactory
{
    public IOverlayPointerDownListener Create(string elementId) => new MockOverlayPointerDownListener(null);
}

public class MockOverlayPointerDownListener : IOverlayPointerDownListener
{
    public event EventHandler? OnPointerDown;

    public MockOverlayPointerDownListener(EventHandler? onPointerDown)
    {
        OnPointerDown += OnPointerDown;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public ValueTask<bool> StartAsync() => ValueTask.FromResult(true);

    public ValueTask<bool> StopAsync() => ValueTask.FromResult(true);
}

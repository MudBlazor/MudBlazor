// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudBlazor.UnitTests.Shared.Mocks;

public class MockJsEventFactory : IJsEventFactory
{
    private readonly IJsEvent? _jsEvent;

    public MockJsEventFactory() : this(null)
    {
    }

    public MockJsEventFactory(IJsEvent? jsEvent)
    {
        _jsEvent = jsEvent;
    }

    public IJsEvent Create() => _jsEvent ?? new MockJsEvent();
}

public class MockJsEvent : IJsEvent
{
#pragma warning disable CS0067
    public event Action<int>? CaretPositionChanged;
    public event Action<string>? Paste;
    public event Action<int, int>? Select;
#pragma warning restore CS0067

    public Task Connect(string element, JsEventOptions options) => Task.CompletedTask;

    public Task Disconnect() => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

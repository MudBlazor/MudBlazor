// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Services.KeyInterceptor.Mocks;

#nullable enable
public class KeyInterceptorObserverMock : IKeyInterceptorObserver, IEquatable<KeyInterceptorObserverMock>
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

    public bool Equals(KeyInterceptorObserverMock? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ElementId == other.ElementId;
    }

    public override bool Equals(object? obj) => obj is KeyInterceptorObserverMock keyObserver && Equals(keyObserver);

    public override int GetHashCode() => ElementId.GetHashCode();
}

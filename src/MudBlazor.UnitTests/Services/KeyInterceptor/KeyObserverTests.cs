// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.KeyInterceptor;

#nullable enable
[TestFixture]
public class KeyObserverTests
{
    [Test]
    public async Task KeyObserver_Ignore()
    {
        // Arrange
        var keyboardDownEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" };
        var keyboardUpEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" };
        var keyNotification = new List<(string elemendId, KeyboardEventArgs keyboardEventArgs)>();
        IKeyInterceptorObserver observer1 = new KeyObserver("observer1", KeyObserver.KeyDown(args => keyNotification.Add(("observer1", args))), KeyObserver.KeyUp(null));
        IKeyInterceptorObserver observer2 = new KeyObserver("observer2", KeyObserver.KeyDown(null), KeyObserver.KeyUp(args => keyNotification.Add(("observer2", args))));
        IKeyInterceptorObserver observer3 = new KeyObserver("observer3", KeyObserver.KeyDown(args => keyNotification.Add(("observer3", args))), KeyObserver.KeyUp((Action<KeyboardEventArgs>?)null));
        IKeyInterceptorObserver observer4 = new KeyObserver("observer4", KeyObserver.KeyDown((Action<KeyboardEventArgs>?)null), KeyObserver.KeyUp(args => keyNotification.Add(("observer4", args))));
        IKeyInterceptorObserver observer5 = new KeyObserver("observer5", KeyObserver.KeyDown(args => keyNotification.Add(("observer5", args))), null);
        IKeyInterceptorObserver observer6 = new KeyObserver("observer6", null, KeyObserver.KeyUp(args => keyNotification.Add(("observer6", args))));
        var observers = new List<IKeyInterceptorObserver> { observer1, observer2, observer3, observer4, observer5, observer6 };

        // Act
        foreach (var observer in observers)
        {
            await observer.NotifyOnKeyDownAsync(keyboardDownEventArgsExpected);
            await observer.NotifyOnKeyUpAsync(keyboardUpEventArgsExpected);
        }

        // Assert
        keyNotification.Count.Should().Be(6);
        keyNotification.Should().BeEquivalentTo(new List<(string elemendId, KeyboardEventArgs keyboardEventArgs)>
        {
            ("observer1", keyboardDownEventArgsExpected),
            ("observer2", keyboardUpEventArgsExpected),
            ("observer3", keyboardDownEventArgsExpected),
            ("observer4", keyboardUpEventArgsExpected),
            ("observer5", keyboardDownEventArgsExpected),
            ("observer6", keyboardUpEventArgsExpected)
        });
    }

    [Test]
    public async Task KeyDownObserverTask_ShouldNotify()
    {
        // Arrange
        var keyboardDownEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" };
        var keyNotification = new List<KeyboardEventArgs>();
        var observer = new KeyObserver("observer1", KeyObserver.KeyDown(KeyDownNotifyAsync), KeyObserver.KeyUpIgnore());

        Task KeyDownNotifyAsync(KeyboardEventArgs args)
        {
            keyNotification.Add(args);

            return Task.CompletedTask;
        }

        // Act
        await ((IKeyDownObserver)observer).NotifyOnKeyDownAsync(keyboardDownEventArgsExpected);
        // Should be ignored
        await ((IKeyUpObserver)observer).NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" });

        // Assert
        keyNotification.Count.Should().Be(1);
        keyNotification.Should().ContainSingle().Which.Should().BeEquivalentTo(keyboardDownEventArgsExpected);
    }

    [Test]
    public async Task KeyUpObserverTask_ShouldNotify()
    {
        // Arrange
        var keyboardUpEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" };
        var keyNotification = new List<KeyboardEventArgs>();
        var observer = new KeyObserver("observer1", KeyObserver.KeyDownIgnore(), KeyObserver.KeyUp(KeyUpNotifyAsync));

        Task KeyUpNotifyAsync(KeyboardEventArgs args)
        {
            keyNotification.Add(args);

            return Task.CompletedTask;
        }

        // Act
        await ((IKeyUpObserver)observer).NotifyOnKeyUpAsync(keyboardUpEventArgsExpected);
        // Should be ignored
        await ((IKeyDownObserver)observer).NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" });

        // Assert
        keyNotification.Count.Should().Be(1);
        keyNotification.Should().ContainSingle().Which.Should().BeEquivalentTo(keyboardUpEventArgsExpected);
    }

    [Test]
    public async Task KeyDownObserver_ShouldNotify()
    {
        // Arrange
        var keyboardDownEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" };
        var keyNotification = new List<KeyboardEventArgs>();
        var observer = new KeyObserver("observer1", KeyObserver.KeyDown(KeyDownNotify), KeyObserver.KeyUpIgnore());

        void KeyDownNotify(KeyboardEventArgs args) => keyNotification.Add(args);

        // Act
        await ((IKeyDownObserver)observer).NotifyOnKeyDownAsync(keyboardDownEventArgsExpected);
        // Should be ignored
        await ((IKeyUpObserver)observer).NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" });

        // Assert
        keyNotification.Count.Should().Be(1);
        keyNotification.Should().ContainSingle().Which.Should().BeEquivalentTo(keyboardDownEventArgsExpected);
    }

    [Test]
    public async Task KeyUpObserver_ShouldNotify()
    {
        // Arrange
        var keyboardUpEventArgsExpected = new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" };
        var keyNotification = new List<KeyboardEventArgs>();
        var observer = new KeyObserver("observer1", KeyObserver.KeyDownIgnore(), KeyObserver.KeyUp(KeyUpNotify));

        void KeyUpNotify(KeyboardEventArgs args) => keyNotification.Add(args);

        // Act
        await ((IKeyUpObserver)observer).NotifyOnKeyUpAsync(keyboardUpEventArgsExpected);
        // Should be ignored
        await ((IKeyDownObserver)observer).NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" });

        // Assert
        keyNotification.Count.Should().Be(1);
        keyNotification.Should().ContainSingle().Which.Should().BeEquivalentTo(keyboardUpEventArgsExpected);
    }

    [Test]
    public void Equals_ReturnsTrueForSameInstance()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);

        // Act
        var result = observer1.Equals(observer1);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        var observer2 = new KeyObserver("observer1", null, null);

        // Act
        var result = observer1.Equals(observer2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        var observer2 = new KeyObserver("observer2", null, null);

        // Act
        var result = observer1.Equals(observer2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        object obj = new KeyObserver("observer1", null, null);

        // Act
        var result = observer1.Equals(obj);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        var observer2 = new KeyObserver("observer2", null, null);
        object obj = observer2;

        // Act
        var result = observer1.Equals(obj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsFalseForObjectIsNull()
    {
        // Arrange
        var observer = new KeyObserver("observer1", null, null);
        object? obj = null;

        // Act
        var result = observer.Equals(obj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var observer = new KeyObserver("observer1", null, null);
        KeyObserver? other = null;

        // Act
        var result = observer.Equals(other);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        var observer2 = new KeyObserver("observer1", null, null);

        // Act
        var hashCode1 = observer1.GetHashCode();
        var hashCode2 = observer2.GetHashCode();

        // Assert
        hashCode2.Should().Be(hashCode1);
    }

    [Test]
    public void GetHashCode_ReturnsDifferentValueForDifferentObjects()
    {
        // Arrange
        var observer1 = new KeyObserver("observer1", null, null);
        var observer2 = new KeyObserver("observer2", null, null);

        // Act
        var hashCode1 = observer1.GetHashCode();
        var hashCode2 = observer2.GetHashCode();

        // Assert
        hashCode2.Should().NotBe(hashCode1);
    }
}

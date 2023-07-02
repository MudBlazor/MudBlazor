// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Browser;

#nullable enable
[TestFixture]
public class BrowserViewportSubscriptionTests
{
    [Test]
    public void Equals_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var subscription1 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        var subscription2 = new BrowserViewportSubscription(subscription1.JavaScriptListenerId, subscription1.ObserverId);

        // Act
        var result = subscription1.Equals(subscription2);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var subscription1 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        var subscription2 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = subscription1.Equals(subscription2);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var subscription = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        object obj = new BrowserViewportSubscription(subscription.JavaScriptListenerId, subscription.ObserverId);

        // Act
        var result = subscription.Equals(obj);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var subscription1 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        var subscription2 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        object obj = subscription2;

        // Act
        var result = subscription1.Equals(obj);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_ObjectOverload_ReturnsFalseForObjectIsNull()
    {
        // Arrange
        var subscription = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        object? obj = null;

        // Act
        var result = subscription.Equals(obj);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var subscription = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        BrowserViewportSubscription? other = null;

        // Act
        var result = subscription.Equals(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var subscription1 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        var subscription2 = new BrowserViewportSubscription(subscription1.JavaScriptListenerId, subscription1.ObserverId);

        // Act
        var hashCode1 = subscription1.GetHashCode();
        var hashCode2 = subscription2.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [Test]
    public void GetHashCode_ReturnsDifferentValueForDifferentObjects()
    {
        // Arrange
        var subscription1 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());
        var subscription2 = new BrowserViewportSubscription(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var hashCode1 = subscription1.GetHashCode();
        var hashCode2 = subscription2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hashCode1, hashCode2);
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Services;
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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
        result.Should().BeFalse();
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
        result.Should().BeFalse();
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
        hashCode2.Should().Be(hashCode1);
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
        hashCode2.Should().NotBe(hashCode1);
    }

    [Test]
    public void Equality_IgnoresOptions()
    {
        // Equality is keyed on the listener/observer ids only; the service consults Options separately.
        // Arrange
        var javaScriptListenerId = Guid.NewGuid();
        var observerId = Guid.NewGuid();
        var subscription1 = new BrowserViewportSubscription(javaScriptListenerId, observerId, new ResizeOptions { ReportRate = 100 });
        var subscription2 = new BrowserViewportSubscription(javaScriptListenerId, observerId, new ResizeOptions { ReportRate = 500 });

        // Act & Assert
        subscription1.Equals(subscription2).Should().BeTrue();
        subscription2.GetHashCode().Should().Be(subscription1.GetHashCode());
    }
}

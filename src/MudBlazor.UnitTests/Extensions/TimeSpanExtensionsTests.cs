// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class TimeSpanExtensionsTests
{
    [Test]
    public void ToIsoString_ShouldReturnHoursAndMinutes_WhenSecondsAndMillisecondsAreFalse()
    {
        // Arrange
        var timeSpan = new TimeSpan(10, 30, 45);

        // Act
        var result = timeSpan.ToIsoString(seconds: false, ms: false);

        // Assert
        result.Should().Be("10:30");
    }

    [Test]
    public void ToIsoString_ShouldReturnHoursMinutesAndSeconds_WhenSecondsIsTrueAndMillisecondsAreFalse()
    {
        // Arrange
        var timeSpan = new TimeSpan(10, 30, 45);

        // Act
        var result = timeSpan.ToIsoString(seconds: true, ms: false);

        // Assert
        result.Should().Be("10:30-45");
    }

    [Test]
    public void ToIsoString_ShouldReturnHoursMinutesSecondsAndMilliseconds_WhenSecondsAndMillisecondsAreTrue()
    {
        // Arrange
        var timeSpan = new TimeSpan(0, 10, 30, 45, 123);

        // Act
        var result = timeSpan.ToIsoString(seconds: true, ms: true);

        // Assert
        result.Should().Be("10:30-45,123");
    }

    [Test]
    public void ToAmPmHour_ShouldReturnCorrectAmPmHour_WhenTimeIsIn24HourFormat()
    {
        // Arrange
        var timeSpan = new TimeSpan(13, 0, 0);

        // Act
        var result = timeSpan.ToAmPmHour();

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void ToAmPmHour_ShouldReturn12_WhenTimeIsMidnight()
    {
        // Arrange
        var timeSpan = new TimeSpan(0, 0, 0);

        // Act
        var result = timeSpan.ToAmPmHour();

        // Assert
        result.Should().Be(12);
    }

    [Test]
    public void ToAmPmHour_ShouldReturn12_WhenTimeIsNoon()
    {
        // Arrange
        var timeSpan = new TimeSpan(12, 0, 0);

        // Act
        var result = timeSpan.ToAmPmHour();

        // Assert
        result.Should().Be(12);
    }
}

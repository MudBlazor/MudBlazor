// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using FluentAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class DateTimeExtensionTests
{
    [Test]
    public void ToIsoDateString_ShouldReturnFormattedString_WhenDateTimeIsProvided()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 5);

        // Act
        var result = dateTime.ToIsoDateString();

        // Assert
        result.Should().Be("2023-10-05");
    }

    [Test]
    public void ToIsoDateString_ShouldReturnFormattedString_WhenNullableDateTimeIsProvided()
    {
        // Arrange
        DateTime? dateTime = new DateTime(2023, 10, 5);

        // Act
        var result = dateTime.ToIsoDateString();

        // Assert
        result.Should().Be("2023-10-05");
    }

    [Test]
    public void ToIsoDateString_ShouldReturnNull_WhenNullableDateTimeIsNull()
    {
        // Arrange
        DateTime? dateTime = null;

        // Act
        var result = dateTime.ToIsoDateString();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void StartOfMonth_ShouldReturnFirstDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 15);
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.StartOfMonth(culture);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 1));
    }

    [Test]
    public void EndOfMonth_ShouldReturnLastDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 15);
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.EndOfMonth(culture);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 31));
    }

    [Test]
    public void StartOfWeek_ShouldReturnFirstDayOfWeek()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 5); // Thursday
        const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        // Act
        var result = dateTime.StartOfWeek(FirstDayOfWeek);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 2)); // Monday
    }

    [Test]
    public void StartOfWeek_ShouldReturnSameDate_WhenDateIsFirstDayOfWeek()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 2); // Monday
        const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        // Act
        var result = dateTime.StartOfWeek(FirstDayOfWeek);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 2)); // Monday
    }

    [Test]
    public void StartOfWeek_ShouldHandleEdgeCase_WhenDateIsNearStartOfYear()
    {
        // Arrange
        var dateTime = new DateTime(1, 1, 3); // Wednesday
        const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        // Act
        var result = dateTime.StartOfWeek(FirstDayOfWeek);

        // Assert
        result.Should().Be(new DateTime(1, 1, 1)); // Monday
    }
}

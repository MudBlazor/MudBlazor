// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
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
    public void ToIsoDateString_ShouldZeroPad_WhenComponentsAreSingleDigit()
    {
        // Arrange
        var dateTime = new DateTime(7, 1, 9); // Year 7, January 9th.

        // Act
        var result = dateTime.ToIsoDateString();

        // Assert
        result.Should().Be("0007-01-09");
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
    public void EndOfMonth_ShouldReturn29Days_ForFebruaryInLeapYear()
    {
        // Arrange
        var dateTime = new DateTime(2024, 2, 10); // 2024 is a leap year.
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.EndOfMonth(culture);

        // Assert
        result.Should().Be(new DateTime(2024, 2, 29));
    }

    [Test]
    public void EndOfMonth_ShouldReturn28Days_ForFebruaryInNonLeapYear()
    {
        // Arrange
        var dateTime = new DateTime(2023, 2, 10); // 2023 is not a leap year.
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.EndOfMonth(culture);

        // Assert
        result.Should().Be(new DateTime(2023, 2, 28));
    }

    [Test]
    public void StartOfMonth_ShouldUseCultureCalendar_WhenNotGregorian()
    {
        // Arrange
        var culture = new CultureInfo("fa-IR"); // Uses the Persian calendar.
        var dateTime = new DateTime(2021, 2, 14); // Mid-month in the Persian calendar (Bahman 1399).

        // Act
        var result = dateTime.StartOfMonth(culture);

        // Assert: result is the first day of the Persian month, not the Gregorian month.
        culture.Calendar.GetDayOfMonth(result).Should().Be(1);
        result.Should().NotBe(new DateTime(2021, 2, 1)); // A Gregorian start-of-month would land here.
        result.Should().BeBefore(dateTime); // Persian Bahman begins in January, before this date.
    }

    [Test]
    public void EndOfMonth_ShouldUseCultureCalendar_WhenNotGregorian()
    {
        // Arrange
        var culture = new CultureInfo("fa-IR"); // Uses the Persian calendar.
        var dateTime = new DateTime(2021, 2, 14); // Mid-month in the Persian calendar (Bahman 1399).

        // Act
        var result = dateTime.EndOfMonth(culture);

        // Assert: result is the last day of the Persian month, not the Gregorian month.
        var year = culture.Calendar.GetYear(result);
        var month = culture.Calendar.GetMonth(result);
        culture.Calendar.GetDayOfMonth(result).Should().Be(culture.Calendar.GetDaysInMonth(year, month));
        result.Should().NotBe(new DateTime(2021, 2, 28)); // A Gregorian end-of-month would land here.
        result.Should().BeAfter(dateTime); // Persian Bahman ends in February, after this date.
    }

    [Test]
    public void StartOfWeek_ShouldReturnFirstDayOfWeek()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 5); // Thursday
        const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        // Act
        var result = dateTime.StartOfWeek(FirstDayOfWeek, CultureInfo.CurrentCulture);

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
        var result = dateTime.StartOfWeek(FirstDayOfWeek, CultureInfo.CurrentCulture);

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
        var result = dateTime.StartOfWeek(FirstDayOfWeek, CultureInfo.CurrentCulture);

        // Assert
        result.Should().Be(new DateTime(1, 1, 1)); // Monday
    }

    [Test]
    public void LastWeekDayOfMonth_ShouldReturnLastWeekDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 9, 15); // September 15, 2023
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.LastWeekDayOfMonth(DayOfWeek.Friday, culture);

        // Assert
        result.Should().Be(new DateTime(2023, 9, 29)); // September 29, 2023 (Friday)
    }

    [Test]
    public void LastWeekDayOfMonth_ShouldReturnLastDay_WhenItMatchesTargetDay()
    {
        // Arrange: October 2023 ends on Tuesday the 31st, so no loop iterations are needed.
        var dateTime = new DateTime(2023, 10, 15);
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.LastWeekDayOfMonth(DayOfWeek.Tuesday, culture);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 31));
    }

    [Test]
    public void FirstWeekDayOfMonth_ShouldReturnFirstWeekDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 9, 15); // September 15, 2023
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.FirstWeekDayOfMonth(DayOfWeek.Monday, culture);

        // Assert
        result.Should().Be(new DateTime(2023, 9, 4)); // September 4, 2023 (Monday)
    }

    [Test]
    public void FirstWeekDayOfMonth_ShouldReturnFirstDay_WhenItMatchesTargetDay()
    {
        // Arrange: October 2023 starts on Sunday the 1st, so no loop iterations are needed.
        var dateTime = new DateTime(2023, 10, 15);
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = dateTime.FirstWeekDayOfMonth(DayOfWeek.Sunday, culture);

        // Assert
        result.Should().Be(new DateTime(2023, 10, 1));
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using AwesomeAssertions;
using Microsoft.Extensions.Time.Testing;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateWrapperDateOnlyTests
{
    [Test]
    public void Today_UsesInjectedTimeProvider()
    {
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero));
        var sut = new DateWrapper<DateOnly>(new DateOnlyConverter(), timeProvider);

        sut.Today.Should().Be(new DateOnly(2024, 6, 15));
    }

    // Today must reflect the user's local civil date, not the UTC date.
    // Fake "now" is 2024-03-15 23:00 in UTC-5 (== 2024-03-16 04:00 UTC).
    // Civil date is the 15th; UtcNow.Date would have given the 16th.
    [Test]
    public void Today_NearMidnightInNonUtcZone_ReturnsLocalCivilDate()
    {
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("UTC-5", TimeSpan.FromHours(-5), "UTC-5", "UTC-5"));
        timeProvider.SetUtcNow(new DateTimeOffset(2024, 3, 16, 4, 0, 0, TimeSpan.Zero));
        var sut = new DateWrapper<DateOnly>(new DateOnlyConverter(), timeProvider);

        sut.Today.Should().Be(new DateOnly(2024, 3, 15));
    }


    [Test]
    [Theory]
    [TestCaseSource(nameof(EndOfMonthTestData))]
    public void EndOfMonth_ShouldReturnExpectedDate(DateOnly date, CultureInfo culture, DateOnly expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateOnly>(new DateOnlyConverter(), TimeProvider.System);
        dateWrapper.SetCulture(culture);

        // Act
        var result = dateWrapper.EndOfMonth(date);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Theory]
    [TestCaseSource(nameof(StartOfMonthTestData))]
    public void StartOfMonth_ShouldReturnExpectedDate(DateOnly date, CultureInfo culture, DateOnly expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateOnly>(new DateOnlyConverter(), TimeProvider.System);
        dateWrapper.SetCulture(culture);

        // Act
        var result = dateWrapper.StartOfMonth(date);

        // Assert
        result.Should().Be(expected);
    }

    private static object[] EndOfMonthTestData() =>
    [
        new object[]
        {
            new DateOnly(2021, 02, 14), CultureInfo.InvariantCulture,
            new DateOnly(2021, 02, 28)
        },
        new object[]
        {
            new DateOnly(2024, 02, 14), CultureInfo.InvariantCulture,
            new DateOnly(2024, 02, 29)
        },
        new object[]
        {
            new DateOnly(1399, 11, 26, new PersianCalendar()), CultureInfo.InvariantCulture,
            new DateOnly(2021, 02, 28)
        },
        new object[]
        {
            new DateOnly(2021, 2, 14),
            CultureInfo.GetCultureInfo("fa-IR"),
            new DateOnly(2021, 2, 18)
        }
    ];

    private static object[] StartOfMonthTestData() =>
    [
        new object[]
        {
            new DateOnly(2021, 2, 14), CultureInfo.InvariantCulture,
            new DateOnly(2021, 2, 1)
        },
        new object[]
        {
            new DateOnly(2024, 2, 1), CultureInfo.InvariantCulture,
            new DateOnly(2024, 2, 1)
        },
        new object[]
        {
            new DateOnly(1399, 11, 26, new PersianCalendar()), CultureInfo.InvariantCulture,
            new DateOnly(2021, 2, 1)
        },
        new object[]
        {
            new DateOnly(2021, 2, 14),
            CultureInfo.GetCultureInfo("fa-IR"),
            new DateOnly(2021, 1, 20)
        }
    ];
}

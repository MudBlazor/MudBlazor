// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using FluentAssertions;
using MudBlazor.Services.DateOperations;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateWrapperDateTimeOffsetTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(EndOfMonthTestData))]
    public void EndOfMonth_ShouldReturnExpectedDate(DateTimeOffset date, CultureInfo culture, DateTimeOffset expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateTimeOffset>(new DateTimeOffsetConverter(), culture);

        // Act
        var result = dateWrapper.EndOfMonth(date);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Theory]
    [TestCaseSource(nameof(StartOfMonthTestData))]
    public void StartOfMonth_ShouldReturnExpectedDate(DateTimeOffset date, CultureInfo culture, DateTimeOffset expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateTimeOffset>(new DateTimeOffsetConverter(), culture);

        // Act
        var result = dateWrapper.StartOfMonth(date);

        // Assert
        result.Should().Be(expected);
    }

    private static object[] EndOfMonthTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 12, 13, 15, TimeSpan.Zero), CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 02, 28, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 12, 13, 15, TimeSpan.FromHours(1)), CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 02, 28, 0, 0, 0, TimeSpan.FromHours(1))
        },
        new object[]
        {
            new DateTimeOffset(2021, 2, 14, 0, 0, 0, TimeSpan.Zero),
            CultureInfo.GetCultureInfo("fa-IR"),
            new DateTimeOffset(2021, 2, 18, 0, 0, 0, TimeSpan.Zero)
        }
    ];

    private static object[] StartOfMonthTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 2, 14, 0, 0, 0, TimeSpan.Zero), CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 2, 1, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero), CultureInfo.InvariantCulture,
            new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(1399, 11, 26, 0, 0, 0, 0, new PersianCalendar(), TimeSpan.Zero), CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 2, 1, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(2021, 2, 14, 0, 0, 0, TimeSpan.Zero),
            CultureInfo.GetCultureInfo("fa-IR"),
            new DateTimeOffset(2021, 1, 20, 0, 0, 0, TimeSpan.Zero)
        }
    ];
}

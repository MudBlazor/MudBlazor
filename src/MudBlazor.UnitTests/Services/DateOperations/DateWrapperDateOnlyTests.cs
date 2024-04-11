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
public class DateWrapperDateOnlyTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(EndOfMonthTestData))]
    public void EndOfMonth_ShouldReturnExpectedDate(DateOnly date, CultureInfo culture, DateOnly expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateOnly>(new DateOnlyConverter(), culture);

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
        var dateWrapper = new DateWrapper<DateOnly>(new DateOnlyConverter(), culture);

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

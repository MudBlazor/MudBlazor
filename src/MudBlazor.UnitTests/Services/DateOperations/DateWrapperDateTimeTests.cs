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
public class DateWrapperDateTimeTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(EndOfMonthTestData))]
    public void EndOfMonth_ShouldReturnExpectedDate(DateTime date, CultureInfo culture, DateTime expected)
    {
        // Arrange
        var dateWrapper = new DateWrapper<DateTime>(new DateTimeConverter(), culture);

        // Act
        var result = dateWrapper.EndOfMonth(date);

        // Assert
        result.Should().Be(expected);
    }

    private static object[] EndOfMonthTestData() =>
    [
        new object[]
        {
            new DateTime(2021, 2, 14, 12, 13, 15, DateTimeKind.Utc),
            CultureInfo.InvariantCulture,
            new DateTime(2021, 2, 28, 0, 0, 0, DateTimeKind.Utc)
        },
        new object[]
        {
            new DateTime(2021, 2, 14, 12, 13, 15),
            CultureInfo.InvariantCulture,
            new DateTime(2021, 2, 28, 0, 0, 0)
        },
        new object[]
        {
            new DateTime(2021, 2, 14, 12, 13, 15, DateTimeKind.Local),
            CultureInfo.InvariantCulture,
            new DateTime(2021, 2, 28, 0, 0, 0, DateTimeKind.Local)
        },
        new object[]
        {
            new DateTime(2021, 2, 14, 12, 13, 15, DateTimeKind.Unspecified),
            CultureInfo.InvariantCulture,
            new DateTime(2021, 2, 28, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new object[]
        {
            new DateTime(2021, 2, 14),
            CultureInfo.GetCultureInfo("fa-IR"),
            new DateTime(2021, 2, 18)
        },
    ];
}

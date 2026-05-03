// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using AwesomeAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateOnlyConverterTests
{
    [Test]
    [TestCaseSource(nameof(ConvertToTestData))]
    public void ConvertTo_ShouldReturnExpectedDateTimeOffset(DateOnly date, DateTimeOffset expected)
    {
        // Arrange
        var dateWrapper = new DateOnlyConverter();

        // Act
        var result = dateWrapper.ConvertTo(date);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [TestCaseSource(nameof(ConvertFromTestData))]
    public void ConvertFrom_ShouldReturnExpectedDateOnly(DateTimeOffset date, DateOnly expected)
    {
        // Arrange
        var dateWrapper = new DateOnlyConverter();

        // Act
        var result = dateWrapper.ConvertFrom(date);

        // Assert
        result.Should().Be(expected);
    }

    // Round-trip invariant: ConvertFrom(ConvertTo(d)) must equal d for every DateOnly value.
    // Guards against future "simplifications" in DateOnlyConverter that read dto.UtcDateTime
    // or dto.LocalDateTime instead of the raw Year/Month/Day, which would zone-shift the date.
    [Test]
    public void RoundTrip_AcrossFullLeapYear_PreservesDate()
    {
        var converter = new DateOnlyConverter();

        var date = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 12, 31);
        while (date <= end)
        {
            converter.ConvertFrom(converter.ConvertTo(date)).Should().Be(date);
            date = date.AddDays(1);
        }
    }

    // Civil-date invariant: when ConvertFrom is given a DateTimeOffset whose offset places
    // its instant on a different UTC day, the result must reflect the *local* civil date
    // carried by the offset — not the UTC date.
    [Test]
    public void ConvertFrom_NonUtcOffset_ReturnsCivilLocalDate()
    {
        var converter = new DateOnlyConverter();

        // 2024-03-15 23:00 in UTC-5 == 2024-03-16 04:00 UTC. Civil date is 15th, not 16th.
        var lateEvening = new DateTimeOffset(2024, 3, 15, 23, 0, 0, TimeSpan.FromHours(-5));
        converter.ConvertFrom(lateEvening).Should().Be(new DateOnly(2024, 3, 15));

        // 2024-03-15 02:00 in UTC+10 == 2024-03-14 16:00 UTC. Civil date is 15th, not 14th.
        var earlyMorning = new DateTimeOffset(2024, 3, 15, 2, 0, 0, TimeSpan.FromHours(10));
        converter.ConvertFrom(earlyMorning).Should().Be(new DateOnly(2024, 3, 15));
    }

    private static object[] ConvertToTestData() =>
    [
        new object[]
        {
            new DateOnly(2021, 02, 14),
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            DateOnly.MinValue,
            DateTimeOffset.MinValue
        },
        new object[]
        {
            DateOnly.MaxValue,
            new DateTimeOffset(DateTimeOffset.MaxValue.Date, TimeSpan.Zero)
        }
    ];

    private static object[] ConvertFromTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero),
            new DateOnly(2021, 02, 14),
        },
        new object[]
        {
            DateTimeOffset.MinValue,
            DateOnly.MinValue,
        },
        new object[]
        {
            DateTimeOffset.MaxValue,
            DateOnly.MaxValue,
        }
    ];
}

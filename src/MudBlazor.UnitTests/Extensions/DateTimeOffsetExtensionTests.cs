// Copyright (c) 2021 - MudBlazor

using System;
using System.Globalization;
using FluentAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

[TestFixture]
public class DateTimeOffsetExtensionTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(StartOfMonthTestData))]
    public void StartOfMonth_ShouldReturnExpectedDateTimeOffset(DateTimeOffset start, CultureInfo culture, DateTimeOffset expected)
    {
        start.StartOfMonth(culture).Should().Be(expected);
    }

    [Test]
    [Theory]
    [TestCaseSource(nameof(AddMonthsTestData))]
    public void AddMonths_ShouldReturnExpectedDateTimeOffset(DateTimeOffset start, int months, CultureInfo culture, DateTimeOffset expected)
    {
        start.AddMonths(months, culture).Should().Be(expected);
    }

    public static object[] AddMonthsTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 12, 13, 15, TimeSpan.Zero),
            1,
            CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 03, 14, 12, 13, 15, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(2021, 01, 31, 12, 13, 15, TimeSpan.Zero),
            1,
            CultureInfo.InvariantCulture,
            new DateTimeOffset(2021, 02, 28, 12, 13, 15, TimeSpan.Zero)
        },
    ];

    public static object[] StartOfMonthTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(new DateTime(2021, 02, 14, 12, 13, 15)),
            CultureInfo.InvariantCulture,
            new DateTimeOffset(new DateTime(2021, 02, 01))
        },
        new object[]
        {
            new DateTimeOffset(new DateTime(2021, 02, 14, 0, 0, 0)),
            CultureInfo.CreateSpecificCulture("zh"),
            new DateTimeOffset(new DateTime(2021, 02, 01, 0, 0, 0))
        },
        new object[]
        {
            new DateTimeOffset(new DateTime(2021, 02, 28)),
            CultureInfo.InvariantCulture,
            new DateTimeOffset(new DateTime(2021, 02, 01))
        },
        new object[]
        {
            new DateTimeOffset(new DateTime(2021, 02, 01)),
            CultureInfo.InvariantCulture,
            new DateTimeOffset(new DateTime(2021, 02, 01))
        }
    ];
}

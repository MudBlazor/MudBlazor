// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateTimeConverterTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(ConvertToTestData))]
    public void ConvertTo(DateTime date, DateTimeOffset expected)
    {
        // Arrange
        var dateWrapper = new DateTimeConverter();

        // Act
        var result = dateWrapper.ConvertTo(date);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Theory]
    [TestCaseSource(nameof(ConvertFromTestData))]
    public void ConvertFrom_ShouldReturnExpectedDateTime(DateTimeOffset date, DateTime expected)
    {
        // Arrange
        var dateWrapper = new DateTimeConverter();

        // Act
        var result = dateWrapper.ConvertFrom(date);

        // Assert
        result.Should().Be(expected);
    }

    private static object[] ConvertToTestData() =>
    [
        new object[]
        {
            new DateTime(2021, 02, 14, 0, 0, 0, DateTimeKind.Utc),
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTime(2021, 02, 14, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(),
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero).ToLocalTime()
        },
        new object[]
        {
            DateTime.MinValue,
            DateTimeOffset.MinValue
        },
        new object[]
        {
            DateTime.MaxValue,
            DateTimeOffset.MaxValue
        }
    ];

    private static object[] ConvertFromTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero),
            new DateTime(2021, 02, 14, 0, 0, 0, DateTimeKind.Utc),
        },
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero).ToLocalTime(),
            new DateTime(2021, 02, 14, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(),
        },
        new object[]
        {
            DateTimeOffset.MinValue,
            DateTime.MinValue,
        },
        new object[]
        {
            DateTimeOffset.MaxValue,
            DateTime.MaxValue,
        }
    ];
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateTimeOffsetConverterTests
{
    [Test]
    [Theory]
    [TestCaseSource(nameof(ConvertToTestData))]
    public void ConvertTo_ShouldReturnSameDateTimeOffset(DateTimeOffset date)
    {
        // Arrange
        var dateWrapper = new DateTimeOffsetConverter();

        // Act
        var result = dateWrapper.ConvertTo(date);

        // Assert
        result.Should().Be(date);
    }

    [Test]
    [Theory]
    [TestCaseSource(nameof(ConvertFromTestData))]
    public void ConvertFrom_ShouldReturnSameDateTimeOffset(DateTimeOffset date)
    {
        // Arrange
        var dateWrapper = new DateTimeOffsetConverter();

        // Act
        var result = dateWrapper.ConvertFrom(date);

        // Assert
        result.Should().Be(date);
    }

    private static object[] ConvertToTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero)
        },
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero).ToLocalTime()
        },
        new object[]
        {
            DateTimeOffset.MinValue
        },
        new object[]
        {
            DateTimeOffset.MaxValue
        }
    ];

    private static object[] ConvertFromTestData() =>
    [
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero),
        },
        new object[]
        {
            new DateTimeOffset(2021, 02, 14, 0, 0, 0, TimeSpan.Zero).ToLocalTime(),
        },
        new object[]
        {
            DateTimeOffset.MinValue,
        },
        new object[]
        {
            DateTimeOffset.MaxValue,
        }
    ];
}

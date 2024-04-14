// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateOnlyConverterTests
{
    [Test]
    [Theory]
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
    [Theory]
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

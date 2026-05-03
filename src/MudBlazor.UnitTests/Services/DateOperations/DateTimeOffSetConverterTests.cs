// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using AwesomeAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DateOperations;

[TestFixture]
public class DateTimeOffsetConverterTests
{
    [Test]
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

    [Test]
    public void ConvertTo_Nullable_NullInput_ReturnsNull()
    {
        new DateTimeOffsetConverter().ConvertTo((DateTimeOffset?)null).Should().BeNull();
    }

    [Test]
    public void ConvertTo_Nullable_NonNullInput_ReturnsSameValue()
    {
        var sample = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.FromHours(2));
        new DateTimeOffsetConverter().ConvertTo((DateTimeOffset?)sample).Should().Be(sample);
    }

    [Test]
    public void ConvertFrom_Nullable_NullInput_ReturnsNull()
    {
        new DateTimeOffsetConverter().ConvertFrom((DateTimeOffset?)null).Should().BeNull();
    }

    [Test]
    public void ConvertFrom_Nullable_NonNullInput_ReturnsSameValue()
    {
        var sample = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.FromHours(2));
        new DateTimeOffsetConverter().ConvertFrom((DateTimeOffset?)sample).Should().Be(sample);
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

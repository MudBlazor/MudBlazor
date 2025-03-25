// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenStringIsNull()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenStringIsEmpty()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = value.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenStringIsWhitespace()
    {
        // Arrange
        const string Value = "   ";

        // Act
        var result = Value.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsEmpty_ShouldReturnFalse_WhenStringIsNotEmpty()
    {
        // Arrange
        const string Value = "Hello";

        // Act
        var result = Value.IsEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Trimmed_ShouldReturnEmptyString_WhenStringIsNull()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.Trimmed();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void Trimmed_ShouldReturnTrimmedString_WhenStringHasWhitespace()
    {
        // Arrange
        const string Value = "  Hello  ";

        // Act
        var result = Value.Trimmed();

        // Assert
        result.Should().Be("Hello");
    }

    [Test]
    public void Trimmed_ShouldReturnSameString_WhenStringHasNoWhitespace()
    {
        // Arrange
        const string Value = "Hello";

        // Act
        var result = Value.Trimmed();

        // Assert
        result.Should().Be("Hello");
    }

    [Test]
    public void ToPercentage_ShouldReturnFormattedString_WhenDecimalHasTwoDecimalPlaces()
    {
        // Arrange
        const decimal Value = 12.34m;

        // Act
        var result = Value.ToPercentage();

        // Assert
        result.Should().Be("12.34");
    }

    [Test]
    public void ToPercentage_ShouldReturnFormattedString_WhenDecimalHasMoreThanTwoDecimalPlaces()
    {
        // Arrange
        const decimal Value = 12.3456m;

        // Act
        var result = Value.ToPercentage();

        // Assert
        result.Should().Be("12.35");
    }

    [Test]
    public void ToPercentage_ShouldReturnFormattedString_WhenDecimalHasNoDecimalPlaces()
    {
        // Arrange
        const decimal Value = 12m;

        // Act
        var result = Value.ToPercentage();

        // Assert
        result.Should().Be("12");
    }
}


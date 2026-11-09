using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

[TestFixture]
public class KeepInRangeExtensionsTests
{
    [Test]
    public void EnsureRange_DoubleMax_ClampsValueToUpperBound()
    {
        // Arrange
        const double input = 5.5;

        // Act
        var result = input.EnsureRange(5.0);

        // Assert
        result.Should().Be(5.0);
    }

    [Test]
    public void EnsureRange_DoubleMinMax_ClampsValueToLowerBound()
    {
        // Arrange
        const double input = -1.2;

        // Act
        var result = input.EnsureRange(0.0, 10.0);

        // Assert
        result.Should().Be(0.0);
    }

    [Test]
    public void EnsureRange_DoubleMinMax_ReturnsInputWhenInBounds()
    {
        // Arrange
        const double input = 7.25;

        // Act
        var result = input.EnsureRange(0.0, 10.0);

        // Assert
        result.Should().Be(7.25);
    }

    [Test]
    public void EnsureRange_ByteMinMax_ClampsValueToBounds()
    {
        // Act
        var belowRange = ((byte)1).EnsureRange(10, 20);
        var aboveRange = ((byte)200).EnsureRange(10, 20);

        // Assert
        belowRange.Should().Be(10);
        aboveRange.Should().Be(20);
    }

    [Test]
    public void EnsureRange_ByteMax_UsesZeroAsLowerBound()
    {
        // Arrange
        const byte input = 20;

        // Act
        var result = input.EnsureRange(10);

        // Assert
        result.Should().Be(10);
    }

    [Test]
    public void EnsureRange_IntMinMax_ClampsValueToBounds()
    {
        // Act
        var belowRange = (-5).EnsureRange(0, 10);
        var aboveRange = 42.EnsureRange(0, 10);

        // Assert
        belowRange.Should().Be(0);
        aboveRange.Should().Be(10);
    }

    [Test]
    public void EnsureRange_IntMax_UsesZeroAsLowerBound()
    {
        // Arrange
        const int input = -3;

        // Act
        var result = input.EnsureRange(10);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void EnsureRangeToByte_ClampsToByteRange()
    {
        // Act
        var belowRange = (-1).EnsureRangeToByte();
        var inRange = 128.EnsureRangeToByte();
        var aboveRange = 300.EnsureRangeToByte();

        // Assert
        belowRange.Should().Be(byte.MinValue);
        inRange.Should().Be((byte)128);
        aboveRange.Should().Be(byte.MaxValue);
    }
}

using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class MathExtensionsTests
{
    [Test]
    public void Map_ReturnsMappedValue_WhenUsingZeroBasedSourceRange()
    {
        // Arrange
        const double sourceMin = 0;
        const double sourceMax = 100;
        const double targetMin = 0;
        const double targetMax = 10;
        const double value = 50;

        // Act
        var result = MathExtensions.Map(sourceMin, sourceMax, targetMin, targetMax, value);

        // Assert
        result.Should().Be(5);
    }

    [Test]
    public void Map_IgnoresSourceMinOffset_WhenSourceRangeDoesNotStartAtZero()
    {
        // Arrange
        const double sourceMin = 20;
        const double sourceMax = 70;
        const double targetMin = 0;
        const double targetMax = 100;
        const double value = 45;

        // Act
        var result = MathExtensions.Map(sourceMin, sourceMax, targetMin, targetMax, value);

        // Assert
        // Formula uses only the range widths: 45 / (70-20) * (100-0). A true linear remap of 45 would be 50.
        result.Should().Be(90);
    }

    [Test]
    public void Map_ReturnsInfinity_WhenSourceRangeIsZero()
    {
        // Arrange
        const double sourceMin = 50;
        const double sourceMax = 50;
        const double targetMin = 0;
        const double targetMax = 10;
        const double value = 5;

        // Act
        var result = MathExtensions.Map(sourceMin, sourceMax, targetMin, targetMax, value);

        // Assert
        result.Should().Be(double.PositiveInfinity);
    }

    [Test]
    public void SumGeneric_ReturnsZero_WhenListIsNull()
    {
        // Arrange
        IReadOnlyList<int>? values = null;

        // Act
        var result = values.SumGeneric();

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void SumGeneric_ReturnsZero_WhenListIsEmpty()
    {
        // Arrange
        IReadOnlyList<int> values = [];

        // Act
        var result = values.SumGeneric();

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void SumGeneric_ReturnsSum_ForIntegerValues()
    {
        // Arrange
        IReadOnlyList<int> values = [1, 2, 3, 4];

        // Act
        var result = values.SumGeneric();

        // Assert
        result.Should().Be(10);
    }

    [Test]
    public void SumGeneric_ReturnsSum_ForDecimalValues()
    {
        // Arrange
        IReadOnlyList<decimal> values = [10.5m, -1.5m, 2m];

        // Act
        var result = values.SumGeneric();

        // Assert
        result.Should().Be(11m);
    }
}

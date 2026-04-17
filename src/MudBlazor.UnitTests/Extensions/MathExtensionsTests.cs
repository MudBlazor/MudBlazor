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

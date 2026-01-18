// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

#nullable enable
[TestFixture]
public class MudColorComparerTests
{
    [Test]
    public void Singleton_Instances_ShouldBeCreated()
    {
        // Arrange & Act
        var rgba = MudColor.MudColorComparer.Rgba;
        var hsl = MudColor.MudColorComparer.Hsl;
        var both = MudColor.MudColorComparer.RgbaAndHsl;

        // Assert
        rgba.Should().NotBeNull();
        hsl.Should().NotBeNull();
        both.Should().NotBeNull();
    }

    [Test]
    public void Singleton_Instances_ShouldHaveCorrectComparisonModes()
    {
        // Arrange & Act
        var rgba = MudColor.MudColorComparer.Rgba;
        var hsl = MudColor.MudColorComparer.Hsl;
        var both = MudColor.MudColorComparer.RgbaAndHsl;

        // Assert
        rgba.Comparison.Should().Be(MudColorComparison.Rgba);
        hsl.Comparison.Should().Be(MudColorComparison.Hsl);
        both.Comparison.Should().Be(MudColorComparison.RgbaAndHsl);
    }

    [Test]
    public void Equals_ShouldUseFallback_WhenComparisonIsInvalid()
    {
        // Arrange
        var comparer = ConstructInvalidComparer();

        var color1 = new MudColor(10, 20, 30, 40);
        var color2 = new MudColor(10, 20, 30, 40);

        // Act
        var result = comparer.Equals(color1, color2);

        // Assert
        result.Should().BeTrue("fallback: x.Equals(y)");
    }

    [Test]
    [TestCaseSource(nameof(AllComparers))]
    public void Equals_ShouldReturnTrue_WhenBothNull(MudColor.MudColorComparer comparer)
    {
        // Arrange & Act
        var result = comparer.Equals(null, null);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    [TestCaseSource(nameof(AllComparers))]
    public void Equals_ShouldReturnFalse_WhenOneNull(MudColor.MudColorComparer comparer)
    {
        // Arrange
        var color = new MudColor("#ff0000");

        // Act
        var result1 = comparer.Equals(color, null);
        var result2 = comparer.Equals(null, color);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    [TestCaseSource(nameof(AllComparers))]
    public void Equals_ShouldReturnTrue_WhenSameReference(MudColor.MudColorComparer comparer)
    {
        // Arrange
        var color = new MudColor("#ff0000");

        // Act
        var result = comparer.Equals(color, color);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_RGBA_ShouldReturnTrue_WhenRgbaMatches()
    {
        // Arrange
        var color1 = new MudColor("#ff0000");
        var color2 = new MudColor("#ff0000");

        // Act
        var result = MudColor.MudColorComparer.Rgba.Equals(color1, color2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_RGBA_ShouldReturnFalse_WhenRgbaDiffers()
    {
        // Arrange
        var red = new MudColor("#ff0000");
        var blue = new MudColor("#0000ff");

        // Act
        var result = MudColor.MudColorComparer.Rgba.Equals(red, blue);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_RGBA_ShouldMatchForEqualColors()
    {
        // Arrange
        var color1 = new MudColor("#ff0000");
        var color2 = new MudColor("#ff0000");

        // Act
        var h1 = MudColor.MudColorComparer.Rgba.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.Rgba.GetHashCode(color2);

        // Assert
        h1.Should().Be(h2);
    }

    [Test]
    [TestCaseSource(nameof(AllComparers))]
    public void GetHashCode_NullObject(MudColor.MudColorComparer comparer)
    {
        // Arrange & Act
        var h1 = comparer.GetHashCode(null);
        var h2 = comparer.GetHashCode(null);

        // Assert
        h1.Should().Be(h2);
    }

    [Test]
    public void GetHashCode_RGBA_ShouldDifferForDifferentColors()
    {
        // Arrange
        var color1 = new MudColor("#ff0000");
        var color2 = new MudColor("#0000ff");

        // Act
        var h1 = MudColor.MudColorComparer.Rgba.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.Rgba.GetHashCode(color2);

        // Assert
        h1.Should().NotBe(h2);
    }

    [Test]
    public void Equals_HSL_ShouldReturnTrue_WhenHslMatches()
    {
        // Arrange
        var color1 = new MudColor(245, 0.34, 0.95, 1);
        var color2 = new MudColor(245, 0.34, 0.95, 1);

        // Act
        var result = MudColor.MudColorComparer.Hsl.Equals(color1, color2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_HSL_ShouldReturnFalse_WhenHslDiffers()
    {
        // Arrange
        var color1 = new MudColor(245, 0.34, 0.95, 1);
        var color2 = new MudColor(245, 0.35, 0.95, 1);

        // Act
        var result = MudColor.MudColorComparer.Hsl.Equals(color1, color2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_HSL_ShouldMatchForEqualColors()
    {
        // Arrange
        var color1 = new MudColor(245, 0.34, 0.95, 1);
        var color2 = new MudColor(245, 0.34, 0.95, 1);

        // Act
        var h1 = MudColor.MudColorComparer.Hsl.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.Hsl.GetHashCode(color2);

        // Assert
        h1.Should().Be(h2);
    }

    [Test]
    public void GetHashCode_HSL_ShouldDifferForDifferentColors()
    {
        // Arrange
        var color1 = new MudColor(245, 0.34, 0.95, 1);
        var color2 = new MudColor(245, 0.35, 0.95, 1);

        // Act
        var h1 = MudColor.MudColorComparer.Hsl.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.Hsl.GetHashCode(color2);

        // Assert
        h1.Should().NotBe(h2);
    }

    [Test]
    public void Equals_Both_ShouldReturnTrue_WhenBothRgbaAndHslMatch()
    {
        // Arrange
        var color1 = new MudColor(239, 238, 247, 1);
        var color2 = new MudColor(color1.H, color1.S, color1.L, 1);

        // Act
        var result = MudColor.MudColorComparer.RgbaAndHsl.Equals(color1, color2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_Both_ShouldReturnFalse_WhenRgbaMatchesButHslDiffers()
    {
        // Arrange
        var color1 = new MudColor(239, 238, 247, 1);
        var color2 = new MudColor(color1.H, color1.S + 0.01, color1.L, 1);

        // Act
        var result = MudColor.MudColorComparer.RgbaAndHsl.Equals(color1, color2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_Both_ShouldMatchForEqualColors()
    {
        // Arrange
        var color1 = new MudColor(239, 238, 247, 1);
        var color2 = new MudColor(color1.H, color1.S, color1.L, 1);

        // Act
        var h1 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color2);

        // Assert
        h1.Should().Be(h2);
    }

    [Test]
    public void GetHashCode_Both_ShouldDiffer_WhenOnlyOneAspectMatches()
    {
        // Arrange
        var color1 = new MudColor(239, 238, 247, 1);
        var color2 = new MudColor(color1.H, color1.S + 0.01, color1.L, 1);

        // Act
        var h1 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color2);

        // Assert
        h1.Should().NotBe(h2);
    }

    [Test]
    public void GetHashCode_Both_ShouldDiffer_ForCompletelyDifferentColors()
    {
        // Arrange
        var color1 = new MudColor("#ff0000");
        var color2 = new MudColor("#0000ff");

        // Act
        var h1 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color1);
        var h2 = MudColor.MudColorComparer.RgbaAndHsl.GetHashCode(color2);

        // Assert
        h1.Should().NotBe(h2);
    }

    [Test]
    public void GetHashCode_ShouldUseFallback_WhenComparisonIsInvalid()
    {
        // Arrange
        var comparer = ConstructInvalidComparer();

        var color = new MudColor(1, 2, 3, 4);

        // Act
        var hash = comparer.GetHashCode(color);

        // Assert
        hash.Should().Be(color.GetHashCode(), because: "fallback: mudColor.GetHashCode()");
    }

    private static MudColor.MudColorComparer ConstructInvalidComparer()
    {
        var invalidComparison = (MudColorComparison)(-1);
        var comparer = (MudColor.MudColorComparer)typeof(MudColor.MudColorComparer)
            .GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                [typeof(MudColorComparison)],
                modifiers: null
            )!
            .Invoke([invalidComparison]);

        return comparer;
    }

    private static IEnumerable<MudColor.MudColorComparer> AllComparers()
    {
        yield return MudColor.MudColorComparer.Rgba;
        yield return MudColor.MudColorComparer.Hsl;
        yield return MudColor.MudColorComparer.RgbaAndHsl;
    }
}

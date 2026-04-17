using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

[TestFixture]
public class ObjectExtensionsTests
{
    [Test]
    public void As_ReturnsTypedInstance_WhenTypeMatches()
    {
        // Arrange
        object value = "mudblazor";

        // Act
        var result = value.As<string>();

        // Assert
        result.Should().Be("mudblazor");
    }

    [Test]
    public void As_ReturnsNull_WhenTypeDoesNotMatch()
    {
        // Arrange
        object value = 5;

        // Act
        var result = value.As<string>();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void As_ReturnsNull_WhenInputIsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.As<string>();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void As_ReturnsDefault_WhenTypeDoesNotMatchValueType()
    {
        // Arrange
        object value = "10";

        // Act
        var result = value.As<int>();

        // Assert
        result.Should().Be(default(int));
    }
}

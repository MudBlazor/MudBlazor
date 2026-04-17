using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class ObjectExtensionsTests
{
    [Test]
    public void As_ReturnsTypedInstance_WhenTypeMatches()
    {
        // Arrange
        object value = "mudblazor";

        // Act
        var result = ObjectExtensions.As<string>(value);

        // Assert
        result.Should().Be("mudblazor");
    }

    [Test]
    public void As_ReturnsNull_WhenTypeDoesNotMatch()
    {
        // Arrange
        object value = 5;

        // Act
        var result = ObjectExtensions.As<string>(value);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void As_ReturnsNull_WhenInputIsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = ObjectExtensions.As<string>(value);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void As_ReturnsDefault_WhenTypeDoesNotMatchValueType()
    {
        // Arrange
        object value = "10";

        // Act
        var result = ObjectExtensions.As<int>(value);

        // Assert
        result.Should().Be(default(int));
    }
}

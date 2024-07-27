// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests;

[TestFixture]
public class CollectionExtensionsTests
{
    private readonly int?[] _array = [1, 2, 3, 4, 5];
    private readonly List<int?> _list = [1, 2, 3, 4, 5];

    [Test]
    public void Any_ReturnsTrue_IfAnyElementMatches()
    {
        // Act
        var result = _array.Any(x => x > 3);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Any_ReturnsFalse_IfNoElementMatches()
    {
        // Act
        var result = _array.Any(x => x > 5);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Any_ReturnsTrue_IfArrayHasElements()
    {
        // Act
        var result = _array.Any();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Any_ReturnsFalse_IfArrayIsEmpty()
    {
        // Arrange
        int[] array = [];

        // Act
        var result = array.Any();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Contains_ReturnsTrue_IfElementExistsInArray()
    {
        // Act
        var result = _array.Contains(3);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Contains_ReturnsFalse_IfElementDoesNotExistInArray()
    {
        // Act
        var result = _array.Contains(6);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void FirstOrDefault_ReturnsElement_IfElementDoesExistInArray()
    {
        // Act
        var result = _array.FirstOrDefault(x => x > 1);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void FirstOrDefault_ReturnsDefault_IfElementDoesNotExistInArray()
    {
        // Act
        var result = _array.FirstOrDefault(x => x > 7);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void FirstOrDefault_ReturnsFirstElement_IfArrayHasElements()
    {
        // Act
        var result = _array.FirstOrDefault();

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void FirstOrDefault_ReturnsDefault_IfArrayIsEmpty()
    {
        // Arrange
        int?[] array = [];

        // Act
        var result = array.FirstOrDefault();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Any_ReturnsTrue_IfAnyElementMatchesInList()
    {
        // Act
        var result = _list.Any(x => x > 3);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Any_ReturnsFalse_IfNoElementMatchesInList()
    {
        // Act
        var result = _list.Any(x => x > 5);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Any_ReturnsTrue_IfListHasElements()
    {
        // Act
        var result = _list.Any();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Any_ReturnsFalse_IfListIsEmpty()
    {
        // Arrange
        List<int> list = [];

        // Act
        var result = list.Any();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Contains_ReturnsTrue_IfElementExistsInList()
    {
        // Act
        var result = _list.Contains(3);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Contains_ReturnsFalse_IfElementDoesNotExistInList()
    {
        // Act
        var result = _list.Contains(6);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void FirstOrDefault_ReturnsElement_IfElementDoesExistInList()
    {
        // Act
        var result = _list.FirstOrDefault(x => x > 1);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void FirstOrDefault_ReturnsDefault_IfElementDoesNotExistInList()
    {
        // Act
        var result = _list.FirstOrDefault(x => x > 7);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void FirstOrDefault_ReturnsFirstElement_IfListHasElements()
    {
        // Act
        var result = _list.FirstOrDefault();

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void FirstOrDefault_ReturnsDefault_IfListIsEmpty()
    {
        // Arrange
        List<int?> array = [];

        // Act
        var result = array.FirstOrDefault();

        // Assert
        result.Should().BeNull();
    }
}

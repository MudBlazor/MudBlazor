// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests;

[TestFixture]
public class EnumerableExtensionsTests
{
    [Test]
    public void AsReadOnlyCollection_NullSource_ReturnsEmptyCollection()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act
        var result = source.AsReadOnlyCollection();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void AsReadOnlyCollection_IReadOnlyCollectionSource_ReturnsSameInstance()
    {
        // Arrange
        IReadOnlyCollection<int> source = new[] { 1, 2, 3 };

        // Act
        var result = source.AsReadOnlyCollection();

        // Assert
        result.Should().BeSameAs(source);
    }

    [Test]
    public void AsReadOnlyCollection_ICollectionSource_ReflectsCount()
    {
        // Arrange
        ICollection<string> source = new List<string> { "a", "b", "c" };

        // Act
        var result = source.AsReadOnlyCollection();

        // Assert
        result.Count.Should().Be(3);
    }

    [Test]
    public void AsReadOnlyCollection_ICollectionSource_ReflectsEnumeration()
    {
        // Arrange
        ICollection<string> source = new List<string> { "a", "b", "c" };

        // Act
        var result = source.AsReadOnlyCollection();

        // Assert
        result.Should().Equal("a", "b", "c");
    }

    [Test]
    public void AsReadOnlyCollection_ICollectionSource_ReflectsLiveCount()
    {
        // Arrange
        var inner = new List<int> { 1, 2 };
        ICollection<int> source = inner;
        var result = source.AsReadOnlyCollection();

        // Act – mutate the underlying collection
        inner.Add(3);

        // Assert – the wrapper reflects the live count
        result.Count.Should().Be(3);
    }

    [Test]
    public void AsReadOnlyCollection_PlainEnumerable_MaterializesOnce()
    {
        // Arrange
        var enumerationCount = 0;
        IEnumerable<int> Source()
        {
            enumerationCount++;
            yield return 1;
            yield return 2;
            yield return 3;
        }

        // Act
        var result = Source().AsReadOnlyCollection();

        // Access the result multiple times to confirm it was enumerated only once
        _ = result.Count;
        _ = result.ToList();

        // Assert
        enumerationCount.Should().Be(1);
        result.Should().Equal(1, 2, 3);
    }
}

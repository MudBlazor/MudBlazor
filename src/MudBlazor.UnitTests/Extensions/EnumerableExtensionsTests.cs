// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
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
    public void AsReadOnlyCollection_ListSource_ReturnsSameInstance()
    {
        // List<T> implements IReadOnlyCollection<T>, so the fast path must return it directly (not wrap or copy).
        var source = new List<int> { 1, 2, 3 };

        var result = source.AsReadOnlyCollection();

        result.Should().BeSameAs(source);
    }

    [Test]
    public void AsReadOnlyCollection_PlainCollectionSource_WrapsWithoutCopying()
    {
        // A type that is ICollection<T> but NOT IReadOnlyCollection<T> hits the wrapper path.
        var inner = new PlainCollection<string> { "a", "b", "c" };

        var result = inner.AsReadOnlyCollection();

        // Wrapped, not the same instance and not a materialized copy.
        result.Should().NotBeSameAs(inner);
        result.Count.Should().Be(3);
        result.Should().Equal("a", "b", "c");
    }

    [Test]
    public void AsReadOnlyCollection_PlainCollectionSource_ReflectsLiveMutations()
    {
        // The wrapper must adapt the live collection rather than snapshot it.
        var inner = new PlainCollection<int> { 1, 2 };
        var result = inner.AsReadOnlyCollection();

        inner.Add(3);

        result.Count.Should().Be(3);
        result.Should().Equal(1, 2, 3);
    }

    [Test]
    public void AsReadOnlyCollection_EmptyEnumerable_ReturnsEmptyCollection()
    {
        // Empty non-collection sequence still materializes to an empty result.
        static IEnumerable<int> Source() { yield break; }

        var result = Source().AsReadOnlyCollection();

        result.Should().BeEmpty();
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

    // ICollection<T> that intentionally does NOT implement IReadOnlyCollection<T>, forcing the wrapper path.
    private sealed class PlainCollection<T> : ICollection<T>
    {
        private readonly List<T> _items = [];

        public int Count => _items.Count;
        public bool IsReadOnly => false;
        public void Add(T item) => _items.Add(item);
        public void Clear() => _items.Clear();
        public bool Contains(T item) => _items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public bool Remove(T item) => _items.Remove(item);
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

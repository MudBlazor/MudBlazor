using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Comparer;

[TestFixture]
public class CollectionComparerTests
{

    [Test]
    public void EqualsTest()
    {
        var comparer = new CollectionComparer<int>();

        // check equality
        comparer.Equals(null, null).Should().Be(true);
        comparer.Equals([], []).Should().Be(true);
        comparer.Equals([1], [1]).Should().Be(true);
        comparer.Equals([2, 1], [1, 2]).Should().Be(true);
        comparer.Equals([1, 2, 1], [1, 2]).Should().Be(true);
        comparer.Equals([1, 2, 1], [1, 2, 2, 2, 1]).Should().Be(true);
        comparer.Equals([1], [1, 1, 1]).Should().Be(true);

        // check unequality
        comparer.Equals(null, []).Should().Be(false);
        comparer.Equals([], null).Should().Be(false);
        comparer.Equals(null, [1]).Should().Be(false);
        comparer.Equals([2], null).Should().Be(false);
        comparer.Equals([1, 2], []).Should().Be(false);
        comparer.Equals([1, 2], [1]).Should().Be(false);
        comparer.Equals([1, 2], [1, 3]).Should().Be(false);
        comparer.Equals([1, 2], [1, 3, 5]).Should().Be(false);
    }

    [Test]
    public void GetHashCodeTest()
    {
        var comparer = CollectionComparer<int>.Default;
        // check equality
        comparer.GetHashCode([1, 2, 3]).Should().Be(comparer.GetHashCode([1, 1, 2, 2, 2, 3, 3, 3]));
        comparer.GetHashCode([1, 2, 3]).Should().Be(comparer.GetHashCode([3, 1, 2]));
        comparer.GetHashCode([1, 2, 3]).Should().Be(comparer.GetHashCode([1, 2, 3]));
        comparer.GetHashCode([]).Should().Be(comparer.GetHashCode([]));
        comparer.GetHashCode([69]).Should().Be(comparer.GetHashCode([69]));
        comparer.GetHashCode(null).Should().Be(0);

        // check inequality
        comparer.GetHashCode(null).Should().NotBe(comparer.GetHashCode([]));
        comparer.GetHashCode([1, 2, 3]).Should().NotBe(comparer.GetHashCode([1, 2]));
        comparer.GetHashCode([1, 2, 3]).Should().NotBe(comparer.GetHashCode([1, 2, 4]));
        comparer.GetHashCode([1, 2, 3]).Should().NotBe(comparer.GetHashCode([1, 2, 4]));
    }

    private class LowercaseEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => EqualityComparer<string>.Default.Equals(x?.ToLowerInvariant(), y?.ToLowerInvariant());

        public int GetHashCode(string obj) => EqualityComparer<string>.Default.GetHashCode(obj?.ToLowerInvariant());
    }

    [Test]
    public void EqualsWithCustomComparerTest()
    {
        var comparer = new CollectionComparer<string>(new LowercaseEqualityComparer());

        // check equality
        comparer.Equals(null, null).Should().Be(true);
        comparer.Equals([], []).Should().Be(true);
        comparer.Equals(["a"], ["A"]).Should().Be(true);
        comparer.Equals(["xyz"], ["xyz"]).Should().Be(true);
        comparer.Equals(["XYZ"], ["xyz"]).Should().Be(true);
        comparer.Equals(["a", "B", "c"], ["c", "A", "b", "a", "C", "b", "b"]).Should().Be(true);
        comparer.Equals(["a", "b", "c"], ["a", "b", "c"]).Should().Be(true);

        // check unequality
        comparer.Equals(null, []).Should().Be(false);
        comparer.Equals([], null).Should().Be(false);
        comparer.Equals(null, ["A"]).Should().Be(false);
        comparer.Equals(["A"], null).Should().Be(false);
        comparer.Equals(["a", "B", "c"], ["c", "A", "x"]).Should().Be(false);
        comparer.Equals(["a", "b", "c"], ["a", "b"]).Should().Be(false);
    }

    [Test]
    public void GetHashCodeWithCustomComparerTest()
    {
        var comparer = new CollectionComparer<string>(new LowercaseEqualityComparer());

        // check equality
        comparer.GetHashCode(["a", "b", "c"]).Should().Be(comparer.GetHashCode(["a", "a", "B", "b", "b", "c", "c", "C"]));
        comparer.GetHashCode(["a", "b", "c"]).Should().Be(comparer.GetHashCode(["c", "a", "b"]));
        comparer.GetHashCode(["a", "b", "c"]).Should().Be(comparer.GetHashCode(["A", "B", "C"]));
        comparer.GetHashCode([]).Should().Be(comparer.GetHashCode([]));
        comparer.GetHashCode(["XY"]).Should().Be(comparer.GetHashCode(["XY"]));
        comparer.GetHashCode(["XY"]).Should().Be(comparer.GetHashCode(["xY"]));
        comparer.GetHashCode(["XY"]).Should().Be(comparer.GetHashCode(["Xy"]));
        comparer.GetHashCode(["XY"]).Should().Be(comparer.GetHashCode(["xy"]));
        comparer.GetHashCode(null).Should().Be(0);

        // check inequality
        comparer.GetHashCode(null).Should().NotBe(comparer.GetHashCode([]));
        comparer.GetHashCode(["a", "b", "c"]).Should().NotBe(comparer.GetHashCode(["a", "b"]));
        comparer.GetHashCode(["a", "b", "c"]).Should().NotBe(comparer.GetHashCode(["a", "b", "x"]));
        comparer.GetHashCode(["a", "b", "c"]).Should().NotBe(comparer.GetHashCode(["a", "a", "x"]));
    }
}

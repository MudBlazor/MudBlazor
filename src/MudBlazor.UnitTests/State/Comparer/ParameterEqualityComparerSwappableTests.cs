// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Comparer;

#nullable enable
[TestFixture]
public class ParameterEqualityComparerSwappableTests
{
    private class CustomEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x != y;

        public int GetHashCode(int obj) => 0;

        public static CustomEqualityComparer Default { get; } = new();
    }

    [Test]
    public void Constructor_ShouldInitializeWithStaticComparer()
    {
        // Arrange
        var comparer = EqualityComparer<int>.Default;

        // Act
        var swappableComparer = new ParameterEqualityComparerSwappable<int>(comparer);

        // Assert
        swappableComparer.UnderlyingComparer().Should().Be(comparer);
    }

    [Test]
    public void Constructor_ShouldInitializeWithComparerFunc()
    {
        // Arrange
        IEqualityComparer<int> ComparerFunc() => EqualityComparer<int>.Default;

        // Act
        var swappableComparer = new ParameterEqualityComparerSwappable<int>(ComparerFunc);

        // Assert
        swappableComparer.UnderlyingComparer().Should().Be(EqualityComparer<int>.Default);
    }

    [Test]
    public void Constructor_ShouldInitializeWithNullFunc()
    {
        // Arrange
        Func<IEqualityComparer<int>>? comparerFunc = null;

        // Act
        var swappableComparer = new ParameterEqualityComparerSwappable<int>(comparerFunc);

        // Assert
        swappableComparer.UnderlyingComparer().Should().Be(EqualityComparer<int>.Default);
    }

    [Test]
    public void Equals_ShouldReturnTrueForEqualValues()
    {
        // Arrange
        var comparer = new ParameterEqualityComparerSwappable<int>(EqualityComparer<int>.Default);

        // Act
        var result = comparer.Equals(1, 1);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ShouldReturnFalseForDifferentValues()
    {
        // Arrange
        var comparer = new ParameterEqualityComparerSwappable<int>(EqualityComparer<int>.Default);

        // Act
        var result = comparer.Equals(1, 2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ShouldReturnCorrectHashCode()
    {
        // Arrange
        var comparer = new ParameterEqualityComparerSwappable<int>(EqualityComparer<int>.Default);

        // Act
        var hashCode = comparer.GetHashCode(1);

        // Assert
        hashCode.Should().Be(1.GetHashCode());
    }

    [Test]
    public void SwappableComparer_ShouldUseNewComparer_WhenSwapped()
    {
        // Arrange
        IEqualityComparer<int> comparer = EqualityComparer<int>.Default;
        // ReSharper disable once AccessToModifiedClosure
        var swappableComparer = new ParameterEqualityComparerSwappable<int>(() => comparer);
        var result1 = swappableComparer.Equals(1, 1);
        var getHashCode1 = swappableComparer.GetHashCode(1);

        // Act
        comparer = CustomEqualityComparer.Default;
        var result2 = swappableComparer.Equals(1, 1);
        var getHashCode2 = swappableComparer.GetHashCode(1);

        // Assert
        result1.Should().NotBe(result2);
        getHashCode1.Should().NotBe(getHashCode2);
    }

    [Test]
    public void TryGetFromParameterView_ShouldReturnTrueAndComparer_WhenParameterExists()
    {
        // Arrange
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            { "comparer", EqualityComparer<int>.Default }
        });
        var comparer = new ParameterEqualityComparerSwappable<int>(EqualityComparer<int>.Default);

        // Act
        var result = comparer.TryGetFromParameterView(parameters, "comparer", out var retrievedComparer);

        // Assert
        result.Should().BeTrue();
        retrievedComparer.Should().Be(EqualityComparer<int>.Default);
    }

    [Test]
    public void TryGetFromParameterView_ShouldReturnFalse_WhenParameterDoesNotExist()
    {
        // Arrange
        var parameters = ParameterView.Empty;
        var comparer = new ParameterEqualityComparerSwappable<int>(EqualityComparer<int>.Default);

        // Act
        var result = comparer.TryGetFromParameterView(parameters, "comparer", out var retrievedComparer);

        // Assert
        result.Should().BeFalse();
        retrievedComparer.Should().BeNull();
    }
}

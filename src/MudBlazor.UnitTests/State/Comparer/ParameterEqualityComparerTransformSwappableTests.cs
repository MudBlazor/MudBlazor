using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Comparer;

#nullable enable
[TestFixture]
public class ParameterEqualityComparerTransformSwappableTests
{
    [Test]
    public void Constructor_ShouldInitializeWithComparerFunc()
    {
        // Arrange
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;

        // Act
        var swappableComparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Assert
        swappableComparer.UnderlyingComparer().Should().BeOfType<CustomStringComparer>();
    }

    [Test]
    public void Equals_ShouldReturnTrueForEqualValues()
    {
        // Arrange
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;
        var comparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Act
        var result = comparer.Equals("test", "test");

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ShouldReturnFalseForDifferentValues()
    {
        // Arrange
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;
        var comparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Act
        var result = comparer.Equals("test", "different");

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ShouldReturnCorrectHashCode()
    {
        // Arrange
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;
        var comparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Act
        var hashCode = comparer.GetHashCode("test");

        // Assert
        hashCode.Should().Be("test".GetHashCode());
    }

    [Test]
    public void TryGetFromParameterView_ShouldReturnTrueAndComparer_WhenParameterExists()
    {
        // Arrange
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            { "comparer", EqualityComparer<int>.Default }
        });
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;
        var comparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Act
        var result = comparer.TryGetFromParameterView(parameters, "comparer", out var retrievedComparer);

        // Assert
        result.Should().BeTrue();
        retrievedComparer.Should().BeOfType<CustomStringComparer>();
    }

    [Test]
    public void TryGetFromParameterView_ShouldReturnFalse_WhenParameterDoesNotExist()
    {
        // Arrange
        var parameters = ParameterView.Empty;
        Func<IEqualityComparer<int>> comparerFromFunc = () => EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>, IEqualityComparer<string>> comparerToFunc = _ => CustomStringComparer.Default;
        var comparer = new ParameterEqualityComparerTransformSwappable<int, string>(comparerFromFunc, comparerToFunc);

        // Act
        var result = comparer.TryGetFromParameterView(parameters, "comparer", out var retrievedComparer);

        // Assert
        result.Should().BeFalse();
        retrievedComparer.Should().BeNull();
    }

    private class CustomStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.Ordinal);

        public int GetHashCode(string obj) => obj.GetHashCode();

        public static CustomStringComparer Default { get; } = new();
    }
}

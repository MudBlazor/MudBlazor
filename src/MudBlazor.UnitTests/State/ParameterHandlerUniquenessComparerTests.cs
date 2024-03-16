// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.State;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

[TestFixture]
public class ParameterHandlerUniquenessComparerTests
{
    [Test]
    public void Equals_NullInstances_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;

        // Act
        var result = comparer.Equals(null, null);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_OneInstanceNull_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var parameterMetadata1 = new ParameterMetadata("Parameter1", "Handler1");

        // Act
        var result1 = comparer.Equals(parameterMetadata1, null);
        var result2 = comparer.Equals(null, parameterMetadata1);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    public void Equals_SameHandlerNames_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler1");

        // Act
        var result = comparer.Equals(handler1, handler2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_DifferentHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler2");

        // Act
        var result = comparer.Equals(handler1, handler2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_NullHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", null);
        var handler2 = new ParameterMetadata("Parameter2", null);

        // Act
        var result = comparer.Equals(handler1, handler2);

        // Assert
        result.Should().BeFalse("If there is no handler name we consider them to be unique.");
    }

    [Test]
    public void GetHashCode_SameHandlerNames_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler1");

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var result = handler1HashCode == handler2HashCode;

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void GetHashCode_DifferentHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler2");

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var result = handler1HashCode == handler2HashCode;

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_NullHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", null);
        var handler2 = new ParameterMetadata("Parameter2", null);

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var result = handler1HashCode == handler2HashCode;

        // Assert
        result.Should().BeFalse("If there is no handler name we consider them to be unique.");
    }
}

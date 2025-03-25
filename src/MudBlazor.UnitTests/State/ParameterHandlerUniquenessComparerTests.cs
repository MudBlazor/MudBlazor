// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.State;
using MudBlazor.State.Builder;
using MudBlazor.State.Comparer;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterHandlerUniquenessComparerTests
{
    [Test]
    public void Equals_NullInstances_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        ParameterMetadata? parameterMetadata1 = null;
        ParameterMetadata? parameterMetadata2 = null;
        IParameterComponentLifeCycle? parameterComponentLifeCycle1 = null;
        IParameterComponentLifeCycle? parameterComponentLifeCycle2 = null;

        // Act
        var result1 = comparer.Equals(parameterMetadata1, parameterMetadata2);
        var result2 = comparer.Equals(parameterComponentLifeCycle1, parameterComponentLifeCycle2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Test]
    public void Equals_OneInstanceNull_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var parameterMetadata = new ParameterMetadata("Parameter1", "Handler1");
        IParameterComponentLifeCycle parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(parameterMetadata)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var result1 = comparer.Equals(parameterMetadata, null);
        var result2 = comparer.Equals(null, parameterMetadata);
        var result3 = comparer.Equals(parameterState, null);
        var result4 = comparer.Equals(null, parameterState);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
        result3.Should().BeFalse();
        result4.Should().BeFalse();
    }

    [Test]
    public void Equals_SameHandlerNames_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler1");
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var result1 = comparer.Equals(handler1, handler2);
        var result2 = comparer.Equals(parameterState1, parameterState2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Test]
    public void Equals_DifferentHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler2");
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var result1 = comparer.Equals(handler1, handler2);
        var result2 = comparer.Equals(parameterState1, parameterState2);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    public void Equals_NullHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", null);
        var handler2 = new ParameterMetadata("Parameter2", null);
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var result1 = comparer.Equals(handler1, handler2);
        var result2 = comparer.Equals(parameterState1, parameterState2);

        // Assert
        result1.Should().BeFalse("If there is no handler name we consider them to be unique.");
        result2.Should().BeFalse("If there is no handler name we consider them to be unique.");
    }

    [Test]
    public void GetHashCode_SameHandlerNames_ReturnsTrue()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler1");
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var parameterState1HashCode = comparer.GetHashCode(parameterState1);
        var parameterState2HashCode = comparer.GetHashCode(parameterState2);
        var result1 = handler1HashCode == handler2HashCode;
        var result2 = parameterState1HashCode == parameterState2HashCode;

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Test]
    public void GetHashCode_DifferentHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", "Handler1");
        var handler2 = new ParameterMetadata("Parameter2", "Handler2");
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var parameterState1HashCode = comparer.GetHashCode(parameterState1);
        var parameterState2HashCode = comparer.GetHashCode(parameterState2);
        var result1 = handler1HashCode == handler2HashCode;
        var result2 = parameterState1HashCode == parameterState2HashCode;

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_NullHandlerNames_ReturnsFalse()
    {
        // Arrange
        var comparer = ParameterHandlerUniquenessComparer.Default;
        var handler1 = new ParameterMetadata("Parameter1", null);
        var handler2 = new ParameterMetadata("Parameter2", null);
        IParameterComponentLifeCycle parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler1)
            .WithGetParameterValueFunc(() => 0)
            .Attach();
        IParameterComponentLifeCycle parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(handler2)
            .WithGetParameterValueFunc(() => 0)
            .Attach();

        // Act
        var handler1HashCode = comparer.GetHashCode(handler1);
        var handler2HashCode = comparer.GetHashCode(handler2);
        var parameterState1HashCode = comparer.GetHashCode(parameterState1);
        var parameterState2HashCode = comparer.GetHashCode(parameterState2);
        var result1 = handler1HashCode == handler2HashCode;
        var result2 = parameterState1HashCode == parameterState2HashCode;

        // Assert
        result1.Should().BeFalse("If there is no handler name we consider them to be unique.");
        result2.Should().BeFalse("If there is no handler name we consider them to be unique.");
    }
}

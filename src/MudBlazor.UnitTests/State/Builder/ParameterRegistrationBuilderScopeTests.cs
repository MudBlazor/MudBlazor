// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using MudBlazor.State;
using MudBlazor.State.Builder;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Builder;

#nullable enable
[TestFixture]
public class ParameterRegistrationBuilderScopeTests
{
    [Test]
    public void IsLocked_ReturnsFalse_WhenScopeIsNotEnded()
    {
        // Arrange
        var factoryWriterMock = new Mock<IParameterStatesFactoryWriter>();
        using var scope = new ParameterRegistrationBuilderScope(factoryWriterMock.Object);

        // Act
        var isLocked = scope.IsLocked;

        // Assert
        isLocked.Should().BeFalse();
    }

    [Test]
    public void IsLocked_ReturnsTrue_WhenScopeIsEnded()
    {
        // Arrange
        var factoryWriterMock = new Mock<IParameterStatesFactoryWriter>();
        var scope = new ParameterRegistrationBuilderScope(factoryWriterMock.Object);

        // Act
        using (scope)
        {
            // Do nothing
        }

        // Assert
        scope.IsLocked.Should().BeTrue();
    }

    [Test]
    public void Dispose_LocksScopeAndWritesParameters_WhenScopeNotEnded()
    {
        // Arrange
        var factoryWriterMock = new Mock<IParameterStatesFactoryWriter>();
        using var scope = new ParameterRegistrationBuilderScope(factoryWriterMock.Object);
        scope.CreateParameterBuilder<int>();
        scope.CreateParameterBuilder<string>();

        // Act
        ((IDisposable)scope).Dispose();

        // Assert
        factoryWriterMock.Verify(factory => factory.WriteParameters(It.IsAny<IEnumerable<IParameterComponentLifeCycle>>()), Times.Once);
        factoryWriterMock.Verify(factory => factory.Close(), Times.Once);
        scope.IsLocked.Should().BeTrue();
    }

    [Test]
    public void Dispose_DoesNotWriteParameters_AfterScopeEnded()
    {
        // Arrange
        var factoryWriterMock = new Mock<IParameterStatesFactoryWriter>();
        var scope = new ParameterRegistrationBuilderScope(factoryWriterMock.Object);
        using (scope)
        {
            scope.CreateParameterBuilder<int>();
            scope.CreateParameterBuilder<string>();
        }

        scope.CreateParameterBuilder<double>();
        scope.CreateParameterBuilder<float>();
        // Act
        ((IDisposable)scope).Dispose();

        // Assert
        factoryWriterMock.Verify(factory => factory.WriteParameters(It.IsAny<IEnumerable<IParameterComponentLifeCycle>>()), Times.Once);
        factoryWriterMock.Verify(factory => factory.Close(), Times.Once);
        scope.IsLocked.Should().BeTrue();
    }
}

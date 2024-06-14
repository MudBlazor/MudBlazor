// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
        var mockWriter = new Mock<IParameterStatesWriter>();
        var mockReader = new Mock<IParameterStatesReader>();
        using var scope = new ParameterRegistrationBuilderScope(new ParameterScopeContainer(mockReader.Object), mockWriter.Object);

        // Act
        var isLocked = scope.IsLocked;

        // Assert
        isLocked.Should().BeFalse();
    }

    [Test]
    public void IsLocked_ReturnsTrue_WhenScopeIsEnded()
    {
        // Arrange
        var mockWriter = new Mock<IParameterStatesWriter>();
        var mockReader = new Mock<IParameterStatesReader>();
        using var scope = new ParameterRegistrationBuilderScope(new ParameterScopeContainer(mockReader.Object), mockWriter.Object);

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
        var mockWriter = new Mock<IParameterStatesWriter>();
        var mockReader = new Mock<IParameterStatesReader>();
        using var scope = new ParameterRegistrationBuilderScope(new ParameterScopeContainer(mockReader.Object), mockWriter.Object);

        scope.RegisterParameter<int>();
        scope.RegisterParameter<string>();

        // Act
        ((IDisposable)scope).Dispose();

        // Assert
        mockReader.Verify(reader => reader.ReadParameters(), Times.Once);
        mockReader.Verify(reader => reader.Complete(), Times.Once);
        scope.IsLocked.Should().BeTrue();
    }

    [Test]
    public void Dispose_DoesNotWriteParameters_AfterScopeEnded()
    {
        // Arrange
        var mockWriter = new Mock<IParameterStatesWriter>();
        var mockReader = new Mock<IParameterStatesReader>();
        var scope = new ParameterRegistrationBuilderScope(new ParameterScopeContainer(mockReader.Object), mockWriter.Object);
        using (scope)
        {
            scope.RegisterParameter<int>();
            scope.RegisterParameter<string>();
        }

        scope.RegisterParameter<double>();
        scope.RegisterParameter<float>();

        // Act
        ((IDisposable)scope).Dispose();

        // Assert
        mockReader.Verify(reader => reader.ReadParameters(), Times.Once);
        mockReader.Verify(reader => reader.Complete(), Times.Once);
        scope.IsLocked.Should().BeTrue();
    }
}

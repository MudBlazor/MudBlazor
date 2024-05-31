// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
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
        using var scope = new ParameterRegistrationBuilderScope();

        // Act
        var isLocked = scope.IsLocked;

        // Assert
        isLocked.Should().BeFalse();
    }

    [Test]
    public void IsLocked_ReturnsTrue_WhenScopeIsEnded()
    {
        // Arrange
        var scope = new ParameterRegistrationBuilderScope();

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
        var scopeEndedCount = 0;
        void OnScopeEndedAction(IParameterStatesReaderOwner? owner) => scopeEndedCount++;
        using var scope = new ParameterRegistrationBuilderScope(OnScopeEndedAction);

        scope.RegisterParameter<int>();
        scope.RegisterParameter<string>();

        // Act
        ((IDisposable)scope).Dispose();

        // Assert
        scopeEndedCount.Should().Be(1);
        scope.IsLocked.Should().BeTrue();
    }

    [Test]
    public void Dispose_DoesNotWriteParameters_AfterScopeEnded()
    {
        // Arrange
        var scopeEndedCount = 0;
        void OnScopeEndedAction(IParameterStatesReaderOwner? owner) => scopeEndedCount++;
        var scope = new ParameterRegistrationBuilderScope(OnScopeEndedAction);
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
        scopeEndedCount.Should().Be(1);
        scope.IsLocked.Should().BeTrue();
    }
}

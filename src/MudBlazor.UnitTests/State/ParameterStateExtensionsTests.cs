// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State.Builder;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterStateExtensionsTests
{
    [Test]
    public async Task SetValueAfterEventCallbackAsync_EventCallbackFireFirst_ThenUpdatesValue()
    {
        // Arrange
        const int InitialValue = 5;
        const int NewValue = 10;
        var eventFired = false;
        var parameterStateBuilder = new RegisterParameterBuilder<int>()
            .WithName(nameof(InitialValue))
            .WithParameter(() => InitialValue);

        var callback = new EventCallback<int>(null, new Action<int>(newValue =>
        {
            eventFired = true;
            // We use the builder to get the parameter state because we have an instance of it.
            // We cannot directly use ParameterState as it would result in an 'unassigned local variable' error.
            // The Attach() method returns the same instance. This technique works with RegisterParameterBuilder 
            // but not with ParameterAttachBuilder, as the latter returns a new instance of ParameterState, which would not be the same instance.
            var existingParameterState = parameterStateBuilder.Attach();

            // Verify that when the callback is fired, the value has not yet been updated to the new value.
            existingParameterState.Value.Should().NotBe(newValue);
        }));

        var parameterState = parameterStateBuilder
            .WithEventCallback(() => callback)
            .Attach();

        // Act
        parameterState.OnInitialized();
        await parameterState.SetValueAfterEventCallbackAsync(NewValue);

        // Assert
        parameterState.Value.Should().Be(NewValue);
        eventFired.Should().BeTrue();
    }
}

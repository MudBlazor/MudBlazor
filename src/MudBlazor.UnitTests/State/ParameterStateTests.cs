// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.UnitTests.State.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

[TestFixture]
public class ParameterStateTests
{
    [Test]
    public async Task SetValueAsync_UpdatesValue_And_EventCallbackFire()
    {
        // Arrange
        const int InitialValue = 5;
        const int NewValue = 10;
        var eventFired = false;
        var callback = new EventCallback<int>(null, () => { eventFired = true; });
        var parameterState = ParameterState<int>.Attach(nameof(InitialValue), () => InitialValue, () => callback);

        // Act
        parameterState.OnInitialized();
        await parameterState.SetValueAsync(NewValue);

        // Assert
        parameterState.Value.Should().Be(NewValue);
        eventFired.Should().BeTrue();
    }

    [Test]
    public void OnInitialized_SetsInitialValue()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterState = ParameterState<int>.Attach(nameof(InitialValue), () => InitialValue, () => default);

        // Act
        parameterState.OnInitialized();

        // Assert
        parameterState.Value.Should().Be(InitialValue);
        parameterState.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task ParameterChangeHandleAsync_HandlesParameterChangeIfHandlerExists()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock();
        var parameterState = ParameterState<int>.Attach(nameof(InitialValue), () => InitialValue, () => default, parameterChangedHandlerMock);

        // Act
        await parameterState.ParameterChangeHandleAsync();

        // Assert
        parameterChangedHandlerMock.FireCount.Should().Be(1);
    }
}

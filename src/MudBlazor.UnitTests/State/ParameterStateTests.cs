// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.UnitTests.State.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
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
        var parameterState = ParameterState.Attach(nameof(InitialValue), () => InitialValue, () => callback);

        // Act
        parameterState.OnInitialized();
        await parameterState.SetValueAsync(NewValue);

        // Assert
        parameterState.Value.Should().Be(NewValue);
        eventFired.Should().BeTrue();
    }

    [Test]
    public async Task SetValueAsync_UpdatesWithSameValue_And_NoEventCallbackFire()
    {
        // Arrange
        const int InitialValue = 5;
        var eventFired = false;
        var callback = new EventCallback<int>(null, () => { eventFired = true; });
        var parameterState = ParameterState.Attach(nameof(InitialValue), () => InitialValue, () => callback);

        // Act
        parameterState.OnInitialized();
        await parameterState.SetValueAsync(InitialValue);

        // Assert
        parameterState.Value.Should().Be(InitialValue);
        eventFired.Should().BeFalse();
    }

    [Test]
    public void OnInitialized_SetsInitialValue()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterState = ParameterState.Attach(nameof(InitialValue), () => InitialValue, () => (EventCallback<int>)default);

        // Act
        parameterState.OnInitialized();

        // Assert
        parameterState.Value.Should().Be(InitialValue);
        parameterState.IsInitialized.Should().BeTrue();
    }

    [Test]
    public void OnParametersSet_UpdatesValueIfChanged()
    {
        // Arrange
        var initialValue = 5;
        const int NewValue = 10;
        // ReSharper disable once AccessToModifiedClosure
        var parameterState = ParameterState.Attach(nameof(initialValue), () => initialValue, () => (EventCallback<int>)default);

        // Act
        parameterState.OnParametersSet();

        // Assert
        parameterState.Value.Should().Be(initialValue);

        // Act & Assert
        initialValue = NewValue;
        parameterState.OnParametersSet();
        parameterState.Value.Should().Be(NewValue);
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
        parameterState.HasHandler.Should().BeTrue();
        parameterChangedHandlerMock.FireCount.Should().Be(1);
    }

    [Test]
    public async Task HasHandler_ReturnsFalseIfNoHandlerSupplied()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterState = ParameterState.Attach(nameof(InitialValue), () => InitialValue, () => (EventCallback<int>)default);

        // Act & Assert
        await parameterState.ParameterChangeHandleAsync(); //Does nothing, we are making coverage happy
        parameterState.HasHandler.Should().BeFalse();
    }

    [Test]
    public void HasParameterChanged_ReturnsTrueIfParameterChanged()
    {
        // Arrange
        const int InitialValue = 5;
        const int NewValue = 10;
        var parameterName = nameof(InitialValue);
        var parameterState = ParameterState.Attach(parameterName, () => InitialValue, () => (EventCallback<int>)default);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { parameterName, NewValue }
        };
        var parameters = ParameterView.FromDictionary(parametersDictionary);

        // Act
        var changed = parameterState.HasParameterChanged(parameters);

        // Assert
        changed.Should().BeTrue();
    }

    [Test]
    public void HasParameterChanged_ReturnsFalseIfParameterNotChanged()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterName = nameof(InitialValue);
        var parameterState = ParameterState.Attach(parameterName, () => InitialValue, () => (EventCallback<int>)default);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { parameterName, InitialValue }
        };
        var parameters = ParameterView.FromDictionary(parametersDictionary);

        // Act
        var changed = parameterState.HasParameterChanged(parameters);

        // Assert
        changed.Should().BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrueForSameParameterName()
    {
        // Arrange
        var parameterState1 = ParameterState.Attach("TestParameter", () => 5, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach("TestParameter", () => 10, () => (EventCallback<int>)default);

        // Act
        var result = parameterState1.Equals(parameterState2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentParameterName()
    {
        // Arrange
        var parameterState1 = ParameterState.Attach("TestParameter1", () => 5, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach("TestParameter2", () => 10, () => (EventCallback<int>)default);

        // Act
        var result = parameterState1.Equals(parameterState2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrueForSameReference()
    {
        // Arrange
        var parameterState = ParameterState.Attach("TestParameter1", () => 5, () => (EventCallback<int>)default);

        // Act
        var result = parameterState.Equals(parameterState);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForNull()
    {
        // Arrange
        var parameterState = ParameterState.Attach("TestParameter1", () => 5, () => (EventCallback<int>)default);

        // Act
        var result = parameterState.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var parameterState = ParameterState.Attach("TestParameter1", () => 5, () => (EventCallback<int>)default);
        var otherObject = new object();

        // Act
        var result = parameterState.Equals(otherObject);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ReturnsTrueForSameParameterName()
    {
        // Arrange
        var parameterState1 = ParameterState.Attach("TestParameter", () => 5, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach("TestParameter", () => 10, () => (EventCallback<int>)default);

        // Act
        var hashCode1 = parameterState1.GetHashCode();
        var hashCode2 = parameterState2.GetHashCode();
        var result = hashCode1 == hashCode2;

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void GetHashCode_ReturnsFalseForDifferentParameterName()
    {
        // Arrange
        var parameterState1 = ParameterState.Attach("TestParameter1", () => 5, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach("TestParameter2", () => 10, () => (EventCallback<int>)default);

        // Act
        var hashCode1 = parameterState1.GetHashCode();
        var hashCode2 = parameterState2.GetHashCode();
        var result = hashCode1 == hashCode2;

        // Assert
        result.Should().BeFalse();
    }
}

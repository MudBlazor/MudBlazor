// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;
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
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(InitialValue)))
            .WithGetParameterValueFunc(() => InitialValue)
            .WithEventCallbackFunc(() => callback)
            .Attach();

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
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(InitialValue)))
            .WithGetParameterValueFunc(() => InitialValue)
            .WithEventCallbackFunc(() => callback)
            .Attach();

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
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(InitialValue)))
            .WithGetParameterValueFunc(() => InitialValue)
            .Attach();

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
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(initialValue)))
            .WithGetParameterValueFunc(() => initialValue)
            .Attach();

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
    public async Task ParameterChangeHandleAsync_ShouldFire_WhenParameterChanged()
    {
        // Arrange
        const int InitialValue = 5;
        const int NewValue = 10;
        const string ParameterName = nameof(InitialValue);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<int>();
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName))
            .WithGetParameterValueFunc(() => InitialValue)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .Attach();
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, NewValue }
        };
        var parameters = ParameterView.FromDictionary(parametersDictionary);

        // Act
        var changed = parameterState.HasParameterChanged(parameters);
        await parameterState.ParameterChangeHandleAsync();

        // Assert
        changed.Should().BeTrue();
        parameterState.HasHandler.Should().BeTrue();
        parameterChangedHandlerMock.Changes.Should().BeEquivalentTo(new[]
        {
            new ParameterChangedEventArgs<int>(ParameterName, InitialValue, NewValue)
        });
    }

    [Test]
    public async Task ParameterChangeHandleAsync_ShouldNotFire_WhenParameterNotChanged()
    {
        // Arrange
        const int InitialValue = 5;
        const string ParameterName = nameof(InitialValue);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<int>();
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName))
            .WithGetParameterValueFunc(() => InitialValue)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .Attach();
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, InitialValue }
        };
        var parameters = ParameterView.FromDictionary(parametersDictionary);

        // Act
        var changed = parameterState.HasParameterChanged(parameters);
        await parameterState.ParameterChangeHandleAsync();

        // Assert
        changed.Should().BeFalse();
        parameterState.HasHandler.Should().BeTrue();
        parameterChangedHandlerMock.Changes.Should().BeEmpty();
    }

    [Test]
    public async Task ParameterChangeHandleAsync_ShouldNotFire_WhenHasParameterChangedNotCalled()
    {
        // Arrange
        const int InitialValue = 5;
        const string ParameterName = nameof(InitialValue);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<int>();
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName))
            .WithGetParameterValueFunc(() => InitialValue)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .Attach();

        // Act
        await parameterState.ParameterChangeHandleAsync();

        // Assert
        parameterState.HasHandler.Should().BeTrue();
        parameterChangedHandlerMock.Changes.Should().BeEmpty("HasParameterChanged wasn't called.");
    }

    [Test]
    public async Task HasHandler_ReturnsFalseIfNoHandlerSupplied()
    {
        // Arrange
        const int InitialValue = 5;
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(InitialValue)))
            .WithGetParameterValueFunc(() => InitialValue)
            .Attach();

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
        const string ParameterName = nameof(InitialValue);
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName))
            .WithGetParameterValueFunc(() => InitialValue)
            .Attach();
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, NewValue }
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
        const string ParameterName = nameof(InitialValue);
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName))
            .WithGetParameterValueFunc(() => InitialValue)
            .Attach();
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, InitialValue }
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter"))
            .WithGetParameterValueFunc(() => 10)
            .Attach();

        // Act
        var result = parameterState1.Equals(parameterState2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentParameterName()
    {
        // Arrange
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter2"))
            .WithGetParameterValueFunc(() => 10)
            .Attach();

        // Act
        var result = parameterState1.Equals(parameterState2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrueForSameReference()
    {
        // Arrange
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();

        // Act
        var result = parameterState.Equals(parameterState);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForNull()
    {
        // Arrange
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();

        // Act
        var result = parameterState.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter"))
            .WithGetParameterValueFunc(() => 10)
            .Attach();

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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter2"))
            .WithGetParameterValueFunc(() => 10)
            .Attach();

        // Act
        var hashCode1 = parameterState1.GetHashCode();
        var hashCode2 = parameterState2.GetHashCode();
        var result = hashCode1 == hashCode2;

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void MetadataToString_ShouldReturnParameterName()
    {
        // Arrange
        var parameterName = "TestParameter1";
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(parameterName))
            .WithGetParameterValueFunc(() => 5)
            .Attach();

        var toString = parameterState1.Metadata.ToString();

        // Assert
        toString.Should().Be(parameterName);
    }

    [Test]
    public void ImplicitOperator()
    {
        // Arrange
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata("TestParameter1"))
            .WithGetParameterValueFunc(() => 5)
            .Attach();

        // Act
        parameterState.OnInitialized();
        var value1 = parameterState.Value;
        int value2 = parameterState;

        // Assert
        value1.Should().Be(5);
        value2.Should().Be(5);
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Builder;

#nullable enable
[TestFixture]
public class RegisterParameterBuilderTests
{
    [Test]
    public async Task RegisterParameterBuilder_ReturnsBuilderInstance1()
    {
        // Arrange
        var callBackCalled = false;
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        var callBack = EventCallback.Factory.Create<double>(this, () => { callBackCalled = true; });
        var comparer = DoubleEpsilonEqualityComparer.Default;
        double parameterValue = 5;

        void OnParameterChanged()
        {
        }

        // Act
        var result = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue)
            .WithEventCallback(() => callBack)
            .WithChangeHandler(OnParameterChanged)
            .WithComparer(() => comparer);

        var parameterState = result.Attach();
        await parameterState.SetValueAsync(parameterValue);

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(parameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChanged));
        parameterState.Metadata.ComparerParameterName.Should().Be(nameof(comparer));
        parameterState.Value.Should().Be(parameterValue);
        parameterState.HasHandler.Should().BeTrue();
        callBackCalled.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeOfType<DoubleEpsilonEqualityComparer>();
    }

    [Test]
    public void RegisterParameterBuilder_ReturnsBuilderInstance2()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        double parameterValue = 5;

        void OnParameterChanged(ParameterChangedEventArgs<double> args)
        {
        }

        // Act
        var result = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue)
            .WithChangeHandler(OnParameterChanged);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(parameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChanged));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(parameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeAssignableTo<EqualityComparer<double>>();
    }

    [Test]
    public void RegisterParameterBuilder_ReturnsBuilderInstance3()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        double parameterValue = 5;

        Task OnParameterChangedAsync() => Task.CompletedTask;

        // Act
        var result = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue)
            .WithChangeHandler(OnParameterChangedAsync);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(parameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChangedAsync));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(parameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeAssignableTo<EqualityComparer<double>>();
    }

    [Test]
    public void RegisterParameterBuilder_ReturnsBuilderInstance4()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        double parameterValue = 5;

        Task OnParameterChangedAsync(ParameterChangedEventArgs<double> args) => Task.CompletedTask;

        // Act
        var result = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue)
            .WithChangeHandler(OnParameterChangedAsync)
            .WithComparer(DoubleEpsilonEqualityComparer.Default);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(parameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChangedAsync));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(parameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeOfType<DoubleEpsilonEqualityComparer>();
    }

    [Test]
    public void RegisterParameterBuilder_AttachReturnsSameInstance()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        double parameterValue = 5;
        var parameterState = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue);

        // Act
        var parameterState1 = parameterState.Attach();
        var parameterState2 = parameterState.Attach();

        // Assert
        parameterState1.Should().BeSameAs(parameterState2);
    }

    [Test]
    public void IsAttached_True()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        var parameterName = "TestParameter";
        double parameterValue = 5;
        var parameterState = builder
            .WithName(parameterName)
            .WithParameter(() => parameterValue);

        // Act
        parameterState.Attach();

        // Assert
        ((IParameterBuilderAttach)builder).IsAttached.Should().BeTrue();
    }

    [Test]
    public void IsAttached_False()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();

        // Act & Assert
        ((IParameterBuilderAttach)builder).IsAttached.Should().BeFalse();
    }
}

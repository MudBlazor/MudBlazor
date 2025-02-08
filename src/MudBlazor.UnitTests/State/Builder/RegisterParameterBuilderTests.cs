// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public void RegisterParameterBuilder_ThrowOnMissingParameterName()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();

        // Act
        var construct = () => builder.Attach();

        // Assert
        construct.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void RegisterParameterBuilder_ThrowOnMissingParameterValue()
    {
        // Arrange
        const string ParameterName = "TestParameter";
        var builder = new RegisterParameterBuilder<double>()
            .WithName(ParameterName);

        // Act
        var construct = () => builder.Attach();

        // Assert
        construct.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task RegisterParameterBuilder_ReturnsBuilderInstance1()
    {
        // Arrange
        var callBackCalled = false;
        var builder = new RegisterParameterBuilder<double>();
        const string ParameterName = "TestParameter";
        var callBack = EventCallback.Factory.Create<double>(this, () => { callBackCalled = true; });
        var comparer = DoubleEpsilonEqualityComparer.Default;
        double parameterValue = 5;

        void OnParameterChanged()
        {
        }

        // Act
        var result = builder
            .WithName(ParameterName)
            .WithParameter(() => parameterValue)
            .WithEventCallback(() => callBack)
            .WithChangeHandler(OnParameterChanged)
            .WithComparer(() => comparer);

        var parameterState = result.Attach();
        await parameterState.SetValueAsync(parameterValue);

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(ParameterName);
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
        const string ParameterName = "TestParameter";
        const double ParameterValue = 5;

        void OnParameterChanged(ParameterChangedEventArgs<double> args)
        {
        }

        // Act
        var result = builder
            .WithName(ParameterName)
            .WithParameter(() => ParameterValue)
            .WithChangeHandler(OnParameterChanged);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(ParameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChanged));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(ParameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeAssignableTo<EqualityComparer<double>>();
    }

    [Test]
    public void RegisterParameterBuilder_ReturnsBuilderInstance3()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        const string ParameterName = "TestParameter";
        const double ParameterValue = 5;

        Task OnParameterChangedAsync() => Task.CompletedTask;

        // Act
        var result = builder
            .WithName(ParameterName)
            .WithParameter(() => ParameterValue)
            .WithChangeHandler(OnParameterChangedAsync);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(ParameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChangedAsync));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(ParameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeAssignableTo<EqualityComparer<double>>();
    }

    [Test]
    public void RegisterParameterBuilder_ReturnsBuilderInstance4()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        const string ParameterName = "TestParameter";
        const double ParameterValue = 5;

        Task OnParameterChangedAsync(ParameterChangedEventArgs<double> args) => Task.CompletedTask;

        // Act
        var result = builder
            .WithName(ParameterName)
            .WithParameter(() => ParameterValue)
            .WithChangeHandler(OnParameterChangedAsync)
            .WithComparer(DoubleEpsilonEqualityComparer.Default);

        var parameterState = result.Attach();
        parameterState.OnInitialized();

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(ParameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(OnParameterChangedAsync));
        parameterState.Metadata.ComparerParameterName.Should().BeNull();
        parameterState.Value.Should().Be(ParameterValue);
        parameterState.HasHandler.Should().BeTrue();
        parameterState.Comparer.UnderlyingComparer().Should().BeOfType<DoubleEpsilonEqualityComparer>();
    }

    [Test]
    public void RegisterParameterBuilder_AttachReturnsSameInstance()
    {
        // Arrange
        var builder = new RegisterParameterBuilder<double>();
        const string ParameterName = "TestParameter";
        const double ParameterValue = 5;
        var parameterState = builder
            .WithName(ParameterName)
            .WithParameter(() => ParameterValue);

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

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Moq;
using MudBlazor.State.Builder;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Builder;

#nullable enable
[TestFixture]
public class RegisterParameterBuilderTests
{
    [Test]
    public async Task RegisterParameterBuilder_ReturnsBuilderInstance()
    {
        // Arrange
        var callBackCalled = false;
        var builder = new RegisterParameterBuilder<double>(Mock.Of<IParameterSetRegister>());
        var parameterName = "TestParameter";
        var callBack = EventCallback.Factory.Create<double>(this, () => { callBackCalled = true; });
        var comparer = DoubleEpsilonEqualityComparer.Default;
        double parameterValue = 5;

        void ParameterChangedHandler()
        {
        }

        // Act
        var result = builder
            .WithParameterName(parameterName)
            .WithParameter(() => parameterValue)
            .WithEventCallback(() => callBack)
            .WithChangeHandler(ParameterChangedHandler)
            .WithComparer(() => comparer);

        var parameterState = result.Attach();
        await parameterState.SetValueAsync(parameterValue);

        // Assert
        parameterState.Metadata.ParameterName.Should().Be(parameterName);
        parameterState.Metadata.HandlerName.Should().Be(nameof(ParameterChangedHandler));
        parameterState.Metadata.ComparerParameterName.Should().Be(nameof(comparer));
        parameterState.Value.Should().Be(parameterValue);
        parameterState.HasHandler.Should().BeTrue();
        callBackCalled.Should().BeTrue();
        parameterState.ComparerFunc().Should().BeOfType<DoubleEpsilonEqualityComparer>();
    }
}

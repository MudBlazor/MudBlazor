// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
internal class ParameterSetTests
{
    [Test]
    public void Add_AddsParameterSuccessfully()
    {
        // Arrange
        const int Parameter = 1;
        var parameterSet = new ParameterSet();
        var parameterState = ParameterState.Attach(nameof(Parameter), () => Parameter, () => (EventCallback<int>)default);

        // Act
        parameterSet.Add(parameterState);

        // Assert
        parameterSet.Contains(parameterState).Should().BeTrue();
    }

    [Test]
    public void Add_ThrowsExceptionIfParameterAlreadyRegistered()
    {
        // Arrange
        const int Parameter = 1;
        var parameterSet = new ParameterSet();
        var parameterState = ParameterState.Attach(nameof(Parameter), () => Parameter, () => (EventCallback<int>)default);
        parameterSet.Add(parameterState);

        // Act 
        var addSameParameter = () => parameterSet.Add(parameterState);

        // Assert
        addSameParameter.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void GetEnumeratorNonGeneric_ReturnsAllParameters()
    {
        // Arrange
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        var parameters = new ParameterSet();
        var parameterState1 = ParameterState.Attach(nameof(Parameter1), () => Parameter1, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach(nameof(Parameter2), () => Parameter2, () => (EventCallback<int>)default);
        var parameterState3 = ParameterState.Attach(nameof(Parameter3), () => Parameter3, () => (EventCallback<int>)default);
        var expectedParameters = new List<IParameterComponentLifeCycle> { parameterState1, parameterState2, parameterState3 };

        foreach (var expectedParameter in expectedParameters)
        {
            parameters.Add(expectedParameter);
        }

        // Act
        var actualParameters = new List<IParameterComponentLifeCycle>();
        var enumerator = ((IEnumerable)parameters).GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is IParameterComponentLifeCycle parameter)
            {
                actualParameters.Add(parameter);
            }
        }

        // Assert
        actualParameters.Should().BeEquivalentTo(expectedParameters);
    }

    [Test]
    public void IEnumerable_GetEnumerator_ReturnsAllParameters()
    {
        // Arrange
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        var parameters = new ParameterSet();
        var parameterState1 = ParameterState.Attach(nameof(Parameter1), () => Parameter1, () => (EventCallback<int>)default);
        var parameterState2 = ParameterState.Attach(nameof(Parameter2), () => Parameter2, () => (EventCallback<int>)default);
        var parameterState3 = ParameterState.Attach(nameof(Parameter3), () => Parameter3, () => (EventCallback<int>)default);
        var expectedParameters = new List<IParameterComponentLifeCycle> { parameterState1, parameterState2, parameterState3 };

        foreach (var expectedParameter in expectedParameters)
        {
            parameters.Add(expectedParameter);
        }

        // Act
        var actualParameters = expectedParameters.ToList();

        // Assert
        actualParameters.Should().BeEquivalentTo(expectedParameters);
    }
}

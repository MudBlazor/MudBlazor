// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterSetTests
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
        var parameterState = ParameterState.Attach(nameof(Parameter), () => Parameter, () => (EventCallback<int>)default);
        var parameterSet = new ParameterSet { parameterState };

        // Act 
        var addSameParameter = () => parameterSet.Add(parameterState);

        // Assert
        addSameParameter.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public async Task SetParametersAsync_SameHandlerNamesShouldFireOnce()
    {
        var handlerFireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        const string ParameterName1 = nameof(Parameter1);
        const string ParameterName2 = nameof(Parameter2);
        const string ParameterName3 = nameof(Parameter3);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName1, 2 },
            { ParameterName2, 3 },
            { ParameterName3, 4 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState1 = ParameterState.Attach(new ParameterMetadata(ParameterName1, nameof(OnParameterChange)), () => Parameter1, OnParameterChange);
        var parameterState2 = ParameterState.Attach(new ParameterMetadata(ParameterName2, nameof(OnParameterChange)), () => Parameter2, OnParameterChange);
        var parameterState3 = ParameterState.Attach(new ParameterMetadata(ParameterName3, nameof(OnParameterChange)), () => Parameter3, OnParameterChange);
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };
        void OnParameterChange()
        {
            handlerFireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handlerFireCount.Should().Be(1);
    }

    [Test]
    public async Task SetParametersAsync_NullHandlerNamesShouldFireAll()
    {
        var handlerFireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        const string ParameterName1 = nameof(Parameter1);
        const string ParameterName2 = nameof(Parameter2);
        const string ParameterName3 = nameof(Parameter3);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName1, 2 },
            { ParameterName2, 3 },
            { ParameterName3, 4 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState1 = ParameterState.Attach(new ParameterMetadata(ParameterName1, null), () => Parameter1, OnParameterChange);
        var parameterState2 = ParameterState.Attach(new ParameterMetadata(ParameterName2, null), () => Parameter2, OnParameterChange);
        var parameterState3 = ParameterState.Attach(new ParameterMetadata(ParameterName3, null), () => Parameter3, OnParameterChange);
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };
        void OnParameterChange()
        {
            handlerFireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handlerFireCount.Should().Be(3);
    }

    [Test]
    public async Task SetParametersAsync_DifferentHandlerNamesShouldFireAll()
    {
        var handlerFireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        const string ParameterName1 = nameof(Parameter1);
        const string ParameterName2 = nameof(Parameter2);
        const string ParameterName3 = nameof(Parameter3);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName1, 2 },
            { ParameterName2, 3 },
            { ParameterName3, 4 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState1 = ParameterState.Attach(new ParameterMetadata(ParameterName1, nameof(OnParameterChange1)), () => Parameter1, OnParameterChange1);
        var parameterState2 = ParameterState.Attach(new ParameterMetadata(ParameterName2, nameof(OnParameterChange2)), () => Parameter2, OnParameterChange2);
        var parameterState3 = ParameterState.Attach(new ParameterMetadata(ParameterName3, nameof(OnParameterChange3)), () => Parameter3, OnParameterChange3);
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };
        void OnParameterChange1()
        {
            handlerFireCount++;
        }
        void OnParameterChange2()
        {
            handlerFireCount++;
        }
        void OnParameterChange3()
        {
            handlerFireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handlerFireCount.Should().Be(3);
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

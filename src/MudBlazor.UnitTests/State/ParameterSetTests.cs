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
    public async Task SetParametersAsync_ActionHandlerShouldFire()
    {
        // Arrange
        ParameterChangedEventArgs<int>? parameter2ChangedEventArgs = null;
        var handler1FireCount = 0;
        var handler2FireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter1NewValue = 2;
        const int Parameter2NewValue = 3;
        const string Parameter1Name = nameof(Parameter1);
        const string Parameter2Name = nameof(Parameter2);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { Parameter1Name, Parameter1NewValue },
            { Parameter2Name, Parameter2NewValue }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameter1State = ParameterState.Attach(Parameter1Name, () => Parameter1, OnParameter1Change);
        var parameter2State = ParameterState.Attach(Parameter2Name, () => Parameter2, OnParameter2Change);
        var parameterSet = new ParameterSet { parameter1State, parameter2State };
        void OnParameter1Change()
        {
            handler1FireCount++;
        }
        void OnParameter2Change(ParameterChangedEventArgs<int> parameterChangedEventArgs)
        {
            parameter2ChangedEventArgs = parameterChangedEventArgs;
            handler2FireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handler1FireCount.Should().Be(1);
        handler2FireCount.Should().Be(1);
        parameter2ChangedEventArgs.Should().NotBeNull();
        parameter2ChangedEventArgs!.ParameterName.Should().Be(Parameter2Name);
        parameter2ChangedEventArgs!.LastValue.Should().Be(Parameter2);
        parameter2ChangedEventArgs!.Value.Should().Be(Parameter2NewValue);
    }

    [Test]
    public async Task SetParametersAsync_ActionHandlerShouldNotFire()
    {
        // Arrange
        ParameterChangedEventArgs<int>? parameter2ChangedEventArgs = null;
        var handler1FireCount = 0;
        var handler2FireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const string Parameter1Name = nameof(Parameter1);
        const string Parameter2Name = nameof(Parameter2);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { Parameter1Name, Parameter1 },
            { Parameter2Name, Parameter2 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameter1State = ParameterState.Attach(Parameter1Name, () => Parameter1, OnParameter1Change);
        var parameter2State = ParameterState.Attach(Parameter2Name, () => Parameter2, OnParameter2Change);
        var parameterSet = new ParameterSet { parameter1State, parameter2State };
        void OnParameter1Change()
        {
            handler1FireCount++;
        }
        void OnParameter2Change(ParameterChangedEventArgs<int> parameterChangedEventArgs)
        {
            parameter2ChangedEventArgs = parameterChangedEventArgs;
            handler2FireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handler1FireCount.Should().Be(0);
        handler2FireCount.Should().Be(0);
        parameter2ChangedEventArgs.Should().BeNull();
    }

    [Test]
    public async Task SetParametersAsync_FuncHandlerShouldFire()
    {
        // Arrange
        ParameterChangedEventArgs<int>? parameter2ChangedEventArgs = null;
        var handler1FireCount = 0;
        var handler2FireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter1NewValue = 2;
        const int Parameter2NewValue = 3;
        const string Parameter1Name = nameof(Parameter1);
        const string Parameter2Name = nameof(Parameter2);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { Parameter1Name, Parameter1NewValue },
            { Parameter2Name, Parameter2NewValue }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameter1State = ParameterState.Attach(Parameter1Name, () => Parameter1, OnParameter1ChangeAsync);
        var parameter2State = ParameterState.Attach(Parameter2Name, () => Parameter2, OnParameter2ChangeAsync);
        var parameterSet = new ParameterSet { parameter1State, parameter2State };
        Task OnParameter1ChangeAsync()
        {
            handler1FireCount++;

            return Task.CompletedTask;
        }
        Task OnParameter2ChangeAsync(ParameterChangedEventArgs<int> parameterChangedEventArgs)
        {
            parameter2ChangedEventArgs = parameterChangedEventArgs;
            handler2FireCount++;

            return Task.CompletedTask;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handler1FireCount.Should().Be(1);
        handler2FireCount.Should().Be(1);
        parameter2ChangedEventArgs.Should().NotBeNull();
        parameter2ChangedEventArgs!.ParameterName.Should().Be(Parameter2Name);
        parameter2ChangedEventArgs!.LastValue.Should().Be(Parameter2);
        parameter2ChangedEventArgs!.Value.Should().Be(Parameter2NewValue);
    }

    [Test]
    public async Task SetParametersAsync_FuncHandlerShouldNotFire()
    {
        // Arrange
        ParameterChangedEventArgs<int>? parameter2ChangedEventArgs = null;
        var handler1FireCount = 0;
        var handler2FireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const string Parameter1Name = nameof(Parameter1);
        const string Parameter2Name = nameof(Parameter2);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { Parameter1Name, Parameter1 },
            { Parameter2Name, Parameter2 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameter1State = ParameterState.Attach(Parameter1Name, () => Parameter1, OnParameter1ChangeAsync);
        var parameter2State = ParameterState.Attach(Parameter2Name, () => Parameter2, OnParameter2ChangeAsync);
        var parameterSet = new ParameterSet { parameter1State, parameter2State };
        Task OnParameter1ChangeAsync()
        {
            handler1FireCount++;

            return Task.CompletedTask;
        }
        Task OnParameter2ChangeAsync(ParameterChangedEventArgs<int> parameterChangedEventArgs)
        {
            parameter2ChangedEventArgs = parameterChangedEventArgs;
            handler2FireCount++;

            return Task.CompletedTask;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handler1FireCount.Should().Be(0);
        handler2FireCount.Should().Be(0);
        parameter2ChangedEventArgs.Should().BeNull();
    }

    [Test]
    public async Task SetParametersAsync_SameHandlerNamesShouldFireOnce()
    {
        // Arrange
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
    public async Task SetParametersAsync_LambdaHandlerNamesShouldFileAll()
    {
        // Arrange
        var handlerFireCount = 0;
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        const string ParameterName1 = nameof(Parameter1);
        const string ParameterName2 = nameof(Parameter2);
        const string ParameterName3 = nameof(Parameter3);
        const string HandlerNameExpression = "() => handlerFireCount++";
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName1, 2 },
            { ParameterName2, 3 },
            { ParameterName3, 4 }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState1 = ParameterState.Attach(new ParameterMetadata(ParameterName1, HandlerNameExpression), () => Parameter1, () => handlerFireCount++);
        var parameterState2 = ParameterState.Attach(new ParameterMetadata(ParameterName2, HandlerNameExpression), () => Parameter2, () => handlerFireCount++);
        var parameterState3 = ParameterState.Attach(new ParameterMetadata(ParameterName3, HandlerNameExpression), () => Parameter3, () => handlerFireCount++);
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handlerFireCount.Should().Be(3);
    }

    [Test]
    public async Task SetParametersAsync_NullHandlerNamesShouldFireAll()
    {
        // Arrange
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
        // Arrange
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

﻿// Copyright (c) MudBlazor 2021
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
using MudBlazor.State.Builder;
using MudBlazor.UnitTests.State.Mocks;
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
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter)))
            .WithGetParameterValueFunc(() => Parameter)
            .Attach();

        // Act
        parameterSet.Add(parameterState);

        // Assert
        parameterSet.Count.Should().Be(1);
        parameterSet.Contains(parameterState).Should().BeTrue();
    }

    [Test]
    public void Add_ThrowsExceptionIfParameterAlreadyRegistered()
    {
        // Arrange
        const int Parameter = 1;
        var parameterState = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter)))
            .WithGetParameterValueFunc(() => Parameter)
            .Attach();
        var parameterSet = new ParameterSet { parameterState };

        // Act 
        var addSameParameter = () => parameterSet.Add(parameterState);

        // Assert
        addSameParameter.Should().Throw<InvalidOperationException>();
        parameterSet.Count.Should().Be(1);
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
        var parameter1State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter1Name))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameter1Change)
            .Attach();
        var parameter2State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter2Name))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameter2Change)
            .Attach();
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
        parameterSet.Count.Should().Be(2);
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
        var parameter1State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter1Name))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameter1Change)
            .Attach();
        var parameter2State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter2Name))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameter2Change)
            .Attach();
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
        parameterSet.Count.Should().Be(2);
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
        var parameter1State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter1Name))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameter1ChangeAsync)
            .Attach();
        var parameter2State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter2Name))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameter2ChangeAsync)
            .Attach();
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
        parameterSet.Count.Should().Be(2);
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
        var parameter1State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter1Name))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameter1ChangeAsync)
            .Attach();
        var parameter2State = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(Parameter2Name))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameter2ChangeAsync)
            .Attach();
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
        parameterSet.Count.Should().Be(2);
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName1, nameof(OnParameterChange)))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName2, nameof(OnParameterChange)))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName3, nameof(OnParameterChange)))
            .WithGetParameterValueFunc(() => Parameter3)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };
        void OnParameterChange()
        {
            handlerFireCount++;
        }

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        parameterSet.Count.Should().Be(3);
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName1, HandlerNameExpression))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(() => handlerFireCount++)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName2, HandlerNameExpression))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(() => handlerFireCount++)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName3, HandlerNameExpression))
            .WithGetParameterValueFunc(() => Parameter3)
            .WithParameterChangedHandler(() => handlerFireCount++)
            .Attach();
        var parameterSet = new ParameterSet { parameterState1, parameterState2, parameterState3 };

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        parameterSet.Count.Should().Be(3);
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName1, null))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName2, null))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName3, null))
            .WithGetParameterValueFunc(() => Parameter3)
            .WithParameterChangedHandler(OnParameterChange)
            .Attach();
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName1, nameof(OnParameterChange1)))
            .WithGetParameterValueFunc(() => Parameter1)
            .WithParameterChangedHandler(OnParameterChange1)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName2, nameof(OnParameterChange2)))
            .WithGetParameterValueFunc(() => Parameter2)
            .WithParameterChangedHandler(OnParameterChange2)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(ParameterName3, nameof(OnParameterChange3)))
            .WithGetParameterValueFunc(() => Parameter3)
            .WithParameterChangedHandler(OnParameterChange3)
            .Attach();
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
    public async Task SetParametersAsync_StaticCustomComparer_HandlerShouldFire()
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<double>();
        const double Parameter = 10000f;
        const double ParameterNewValue = 10001f;
        const string ParameterName = nameof(Parameter);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, ParameterNewValue },
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState = ParameterAttachBuilder
            .Create<double>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter)))
            .WithGetParameterValueFunc(() => Parameter)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .WithComparer(comparer)
            .Attach();
        var parameterSet = new ParameterSet { parameterState };

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        parameterChangedHandlerMock.Changes.Should().BeEquivalentTo(new[]
        {
            new ParameterChangedEventArgs<double>(ParameterName, Parameter, ParameterNewValue)
        });
    }

    [Test]
    public async Task SetParametersAsync_StaticCustomComparer_HandlerShouldNotFire()
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<double>();
        const double Parameter = 1000000f;
        const double ParameterNewValue = 1000001f;
        const string ParameterName = nameof(Parameter);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, ParameterNewValue }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        var parameterState = ParameterAttachBuilder
            .Create<double>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter)))
            .WithGetParameterValueFunc(() => Parameter)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .WithComparer(comparer)
            .Attach();
        var parameterSet = new ParameterSet { parameterState };

        // Act
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        parameterChangedHandlerMock.Changes.Should().BeEmpty("Within the epsilon tolerance.");
    }

    [Test]
    public async Task SetParametersAsync_FuncCustomComparer_Swap()
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.0001f);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<double>();
        const double Parameter = 10000f;
        const double ParameterNewValue = 10001f;
        const string ParameterName = nameof(Parameter);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, ParameterNewValue }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        // ReSharper disable once AccessToModifiedClosure
        var parameterState = ParameterAttachBuilder
            .Create<double>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter)))
            .WithGetParameterValueFunc(() => Parameter)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .WithComparer(() => comparer)
            .Attach();
        var parameterSet = new ParameterSet { parameterState };

        // Act && Assert
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);
        parameterChangedHandlerMock.Changes.Should().BeEmpty("Within the epsilon tolerance.");

        comparer = new DoubleEpsilonEqualityComparer(0.00001f);
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);
        parameterChangedHandlerMock.Changes.Should().BeEquivalentTo(new[]
        {
            new ParameterChangedEventArgs<double>(ParameterName, Parameter, ParameterNewValue)
        }, because: "We swapped comparer.");
    }

    [Test(Description = "Tests a very special case described in ParameterStateInternal.HasParameterChanged when comparer is a blazor parameter and changes together with the associated value.")]
    public async Task SetParametersAsync_FuncCustomComparerAsParameter_Swap()
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.0001f);
        var parameterChangedHandlerMock = new ParameterChangedHandlerMock<double>();
        const double Parameter = 10000f;
        const double ParameterNewValue = 10001f;
        const string ParameterName = nameof(Parameter);
        var parametersDictionary = new Dictionary<string, object?>
        {
            { ParameterName, ParameterNewValue },
            { nameof(comparer), new DoubleEpsilonEqualityComparer(0.00001f) }
        };
        var parameterView = ParameterView.FromDictionary(parametersDictionary);
        // ReSharper disable once AccessToModifiedClosure
        var parameterState = ParameterAttachBuilder
            .Create<double>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter), null, nameof(comparer)))
            .WithGetParameterValueFunc(() => Parameter)
            .WithParameterChangedHandler(parameterChangedHandlerMock)
            .WithComparer(() => comparer)
            .Attach();
        var parameterSet = new ParameterSet { parameterState };

        // Act && Assert
        await parameterSet.SetParametersAsync(_ => Task.CompletedTask, parameterView);
        parameterChangedHandlerMock.Changes.Should().BeEquivalentTo(new[]
        {
            new ParameterChangedEventArgs<double>(ParameterName, Parameter, ParameterNewValue)
        });
    }

    [Test]
    public void GetEnumeratorNonGeneric_ReturnsAllParameters()
    {
        // Arrange
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
        var parameters = new ParameterSet();
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter1)))
            .WithGetParameterValueFunc(() => Parameter1)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter2)))
            .WithGetParameterValueFunc(() => Parameter2)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter3)))
            .WithGetParameterValueFunc(() => Parameter3)
            .Attach();
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
        parameters.Count.Should().Be(3);
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
        var parameterState1 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter1)))
            .WithGetParameterValueFunc(() => Parameter1)
            .Attach();
        var parameterState2 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter2)))
            .WithGetParameterValueFunc(() => Parameter2)
            .Attach();
        var parameterState3 = ParameterAttachBuilder
            .Create<int>()
            .WithMetadata(new ParameterMetadata(nameof(Parameter3)))
            .WithGetParameterValueFunc(() => Parameter3)
            .Attach();
        var expectedParameters = new List<IParameterComponentLifeCycle> { parameterState1, parameterState2, parameterState3 };

        foreach (var expectedParameter in expectedParameters)
        {
            parameters.Add(expectedParameter);
        }

        // Act
        var actualParameters = expectedParameters.ToList();

        // Assert
        parameters.Count.Should().Be(3);
        actualParameters.Should().BeEquivalentTo(expectedParameters);
    }
}

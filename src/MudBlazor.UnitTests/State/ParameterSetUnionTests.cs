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
using MudBlazor.State.Builder;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterSetUnionTests
{
    [Test]
    public async Task SetParametersAsync_ActionHandlerShouldFire_WhenDefinedInDifferentSets()
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
        var parameterSetUnion = new ParameterSetUnion { new(parameter1State), new(parameter2State) };
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
        await parameterSetUnion.SetParametersAsync(_ => Task.CompletedTask, parameterView);

        // Assert
        handler1FireCount.Should().Be(1);
        handler2FireCount.Should().Be(1);
        parameter2ChangedEventArgs.Should().NotBeNull();
        parameter2ChangedEventArgs!.ParameterName.Should().Be(Parameter2Name);
        parameter2ChangedEventArgs!.LastValue.Should().Be(Parameter2);
        parameter2ChangedEventArgs!.Value.Should().Be(Parameter2NewValue);
    }

    [Test]
    public void GetEnumeratorNonGeneric_ReturnsAllParameterSets()
    {
        // Arrange
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
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
        var parameterSetUnion = new ParameterSetUnion();
        var expectedParameters = new List<ParameterSet> { new(parameterState1), new(parameterState2), new(parameterState3) };
        foreach (var expectedParameter in expectedParameters)
        {
            parameterSetUnion.Add(expectedParameter);
        }

        // Act
        var actualParameters = new List<ParameterSet>();
        var enumerator = ((IEnumerable)parameterSetUnion).GetEnumerator();
        using (enumerator as IDisposable)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ParameterSet parameterSet)
                {
                    actualParameters.Add(parameterSet);
                }
            }
        }

        // Assert
        parameterSetUnion.Count().Should().Be(3);
        actualParameters.Should().BeEquivalentTo(expectedParameters);
    }

    [Test]
    public void IEnumerable_GetEnumerator_ReturnsAllParameterSets()
    {
        // Arrange
        const int Parameter1 = 1;
        const int Parameter2 = 2;
        const int Parameter3 = 3;
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
        var parameterSetUnion = new ParameterSetUnion();
        var expectedParameters = new List<ParameterSet> { new(parameterState1), new(parameterState2), new(parameterState3) };
        foreach (var expectedParameter in expectedParameters)
        {
            parameterSetUnion.Add(expectedParameter);
        }

        // Act
        var actualParameters = parameterSetUnion.ToList();

        // Assert
        parameterSetUnion.Count().Should().Be(3);
        actualParameters.Should().BeEquivalentTo(expectedParameters);
    }
}

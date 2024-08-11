// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components.Dialog;

#nullable enable
[TestFixture]
public sealed class DialogParametersTests
{
    [Test]
    public void DialogParametersGeneric_Add_ShouldAddGenericParameter()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        dialogParameters._parameters.Should().BeEmpty();

        dialogParameters.Add(x => x.TestValue, "Test");
        dialogParameters._parameters.Should().Contain(new KeyValuePair<string, object?>("TestValue", "Test"));
    }

    [Test]
    public void DialogParametersGeneric_Add_ShouldAddParameter()
    {
        var dialogParameters = new DialogParameters();
        dialogParameters._parameters.Should().BeEmpty();

        dialogParameters.Add("TestValue", "Test");
        dialogParameters._parameters.Should().Contain(new KeyValuePair<string, object?>("TestValue", "Test"));
    }

    [Test]
    public void DialogParametersGeneric_Add_ShouldThrow_IfNotMemberExpression()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        Assert.Throws<ArgumentException>(() => dialogParameters.Add(x => 1, 2));
    }

    [Test]
    public void DialogParametersGeneric_Get_ShouldGetParameter()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        dialogParameters._parameters = new() { { "TestValue", "Test" } };

        var parameter = dialogParameters.Get(x => x.TestValue);
        parameter.Should().Be("Test");
    }

    [Test]
    public void DialogParameters_Get_ShouldGetParameter()
    {
        var dialogParameters = new DialogParameters();
        dialogParameters._parameters = new() { { "TestValue", "Test" } };

        var parameter = dialogParameters.Get<string>("TestValue");
        parameter.Should().Be("Test");
    }

    [Test]
    public void DialogParametersGeneric_Get_ShouldThrow_IfNotMemberExpression()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        Assert.Throws<ArgumentException>(() => dialogParameters.Get(x => 1));
    }

    [Test]
    public void DialogParameters_TryGet_ShouldGetParameter()
    {
        var dialogParameters = new DialogParameters();
        dialogParameters._parameters = new() { { "TestValue", "Test" } };

        var parameter = dialogParameters.TryGet<string>("TestValue");
        parameter!.Should().Be("Test");
    }

    [Test]
    public void DialogParametersGeneric_TryGet_ShouldGetParameter()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        dialogParameters._parameters = new() { { "TestValue", "Test" } };

        var parameter = dialogParameters.TryGet(x => x.TestValue);
        parameter!.Should().Be("Test");
    }

    [Test]
    public void DialogParameters_TryGet_ShouldGetDefault_IfParameterDoesNotExist()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();

        var parameter = dialogParameters.TryGet<string>("TestValue");
        parameter.Should().Be(default(string));
    }

    [Test]
    public void DialogParametersGeneric_TryGet_ShouldGetDefault_IfParameterDoesNotExist()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();

        var parameter = dialogParameters.TryGet(x => x.TestValue);
        parameter.Should().Be(default(string));
    }

    [Test]
    public void DialogParametersGeneric_TryGet_ShouldThrow_IfNotMemberExpression()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        Assert.Throws<ArgumentException>(() => dialogParameters.TryGet(x => 1));
    }

    [Test]
    public void DialogParameters_Count_ShouldBeCorrect()
    {
        var dialogParameters = new DialogParameters();
        dialogParameters.Count.Should().Be(0);
        for (var index = 1; index <= 50; index++)
        {
            dialogParameters.Add($"TestValue{index}", "Test");
            dialogParameters.Count.Should().Be(index);
        }
    }

    [Test]
    public void DialogParametersGeneric_Count_ShouldBeCorrect()
    {
        var dialogParameters = new DialogParameters<DialogWithParameters>();
        dialogParameters.Count.Should().Be(0);
        for (var index = 1; index <= 50; index++)
        {
            dialogParameters.Add($"TestValue{index}", "Test");
            dialogParameters.Count.Should().Be(index);
        }
    }
}

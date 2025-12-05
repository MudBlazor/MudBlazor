// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using MudBlazor.Analyzers;
using MudBlazor.UnitTests.Analyzers.Verifiers;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

using VerifyCS = CSharpAnalyzerVerifier<ParameterStateAnalyzer>;

/// <summary>
/// Tests for ParameterStateAnalyzer following Microsoft's analyzer testing patterns.
/// </summary>
[TestFixture]
public class ParameterStateAnalyzerTests
{
    [Test]
    public void AnalyzerShouldReportSupportedDiagnostics()
    {
        var analyzer = new ParameterStateAnalyzer();
        var supportedDiagnostics = analyzer.SupportedDiagnostics;

        Assert.That(supportedDiagnostics, Has.Length.EqualTo(3));
        Assert.That(supportedDiagnostics, Has.Some.Matches<DiagnosticDescriptor>(d => d.Id == "MUD0010"));
        Assert.That(supportedDiagnostics, Has.Some.Matches<DiagnosticDescriptor>(d => d.Id == "MUD0011"));
        Assert.That(supportedDiagnostics, Has.Some.Matches<DiagnosticDescriptor>(d => d.Id == "MUD0012"));
    }

    [Test]
    public void DiagnosticDescriptors_ShouldHaveCorrectProperties()
    {
        // Assert MUD0010
        Assert.That(ParameterStateAnalyzer.ReadDescriptor.Id, Is.EqualTo("MUD0010"));
        Assert.That(ParameterStateAnalyzer.ReadDescriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Warning));
        Assert.That(ParameterStateAnalyzer.ReadDescriptor.IsEnabledByDefault, Is.True);

        // Assert MUD0011
        Assert.That(ParameterStateAnalyzer.WriteDescriptor.Id, Is.EqualTo("MUD0011"));
        Assert.That(ParameterStateAnalyzer.WriteDescriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Warning));
        Assert.That(ParameterStateAnalyzer.WriteDescriptor.IsEnabledByDefault, Is.True);

        // Assert MUD0012
        Assert.That(ParameterStateAnalyzer.ExternalAccessDescriptor.Id, Is.EqualTo("MUD0012"));
        Assert.That(ParameterStateAnalyzer.ExternalAccessDescriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Warning));
        Assert.That(ParameterStateAnalyzer.ExternalAccessDescriptor.IsEnabledByDefault, Is.True);
    }

    [Test]
    public async Task MUD0010_ReadInsideMethod_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public int GetCounter()
    {
        return {|#0:Counter|};
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0010").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0010_ReadInVariableAssignment_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void Method()
    {
        var x = {|#0:Counter|};
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0010").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0010_ReadAsMethodArgument_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void Method()
    {
        DoSomething({|#0:Counter|});
    }

    private void DoSomething(int value) { }
}";

        var expected = VerifyCS.Diagnostic("MUD0010").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0011_WriteInsideMethod_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void SetCounter(int value)
    {
        {|#0:Counter|} = value;
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0011").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0011_CompoundAssignment_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void Increment()
    {
        {|#0:Counter|} += 1;
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0011").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0011_Increment_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void Increment()
    {
        {|#0:Counter|}++;
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0011").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0011_Decrement_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public void Decrement()
    {
        {|#0:Counter|}--;
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0011").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0011_ConstructorAssignment_ShouldNotReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public MyComponent()
    {
        Counter = 0;
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task MUD0011_SetParametersAsyncAssignment_ShouldNotReportDiagnostic()
    {
        var source = @"
using System;
using System.Threading.Tasks;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public Task SetParametersAsync()
    {
        Counter = 5;
        return Task.CompletedTask;
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task MUD0012_ExternalRead_ShouldReportDiagnostic()
    {
        var source = @"
using System;
using MudBlazor.State;

class ComponentA
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }
}

class ComponentB
{
    private ComponentA _componentA = new ComponentA();

    public int GetExternalCounter()
    {
        return {|#0:_componentA.Counter|};
    }
}";

        var expected = VerifyCS.Diagnostic("MUD0012").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task MUD0010_ShouldNotReportForExternalAccess()
    {
        var source = @"
using System;
using MudBlazor.State;

class ComponentA
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }
}

class ComponentB
{
    private ComponentA _componentA = new ComponentA();

    public int GetExternalCounter()
    {
        return {|#0:_componentA.Counter|};
    }
}";

        // External access should report MUD0012, not MUD0010
        var expected = VerifyCS.Diagnostic("MUD0012").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyDoesNotHaveParameterStateAttribute()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    public int Counter { get; set; }

    public int GetCounter()
    {
        return Counter;
    }

    public void SetCounter(int value)
    {
        Counter = value;
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task NoDiagnostic_WhenAttributeNotAvailable()
    {
        var source = @"
class MyComponent
{
    public int Counter { get; set; }

    public int GetCounter()
    {
        return Counter;
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task NoDiagnostic_WhenUsingNameofOnParameterStateProperty()
    {
        var source = @"
using System;
using MudBlazor.State;

class MyComponent
{
    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public string GetPropertyName()
    {
        return nameof(Counter);
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Test]
    public async Task NoDiagnostic_WhenUsingConstructorWithParameterStateFramework()
    {
        var source = @"
using System;
using MudBlazor;
using MudBlazor.State;

class MyComponent : ComponentBaseWithState
{
    private readonly ParameterState<int> _counterState;

    [MudBlazor.State.ParameterState]
    public int Counter { get; set; }

    public MyComponent()
    {
        Counter = 0;
        using var registerScope = base.CreateRegisterScope();
        _counterState = registerScope.RegisterParameter<int>(nameof(Counter))
            .WithParameter(() => Counter);
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }
}

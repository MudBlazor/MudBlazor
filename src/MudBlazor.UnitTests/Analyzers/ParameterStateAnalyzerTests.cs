// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using MudBlazor.Analyzers;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

/// <summary>
/// Tests for ParameterStateAnalyzer using inline code and AdhocWorkspace following Microsoft's approach.
/// </summary>
[TestFixture]
public class ParameterStateAnalyzerTests
{
    // Base source code that defines the ParameterStateAttribute
    private const string ParameterStateAttributeSource = @"
namespace MudBlazor.State
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ParameterStateAttribute : System.Attribute { }
}
";

    private static readonly MetadataReference[] References =
    [
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(System.Threading.Tasks.Task).Assembly.Location)
    ];

    private static Diagnostic[] GetDiagnostics(string source)
    {
        var fullSource = ParameterStateAttributeSource + source;
        var analyzer = new ParameterStateAnalyzer();

        var projectId = ProjectId.CreateNewId("TestProject");
        var documentId = DocumentId.CreateNewId(projectId, "Test0.cs");

        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReferences(projectId, References)
            .AddDocument(documentId, "Test0.cs", SourceText.From(fullSource));

        var project = solution.GetProject(projectId)!;
        var compilation = project.GetCompilationAsync().Result!;
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
    }

    [Test]
    public void AnalyzerShouldReportSupportedDiagnostics()
    {
        var analyzer = new ParameterStateAnalyzer();
        var supportedDiagnostics = analyzer.SupportedDiagnostics;

        supportedDiagnostics.Should().HaveCount(3);
        supportedDiagnostics.Should().Contain(d => d.Id == "MUD0010");
        supportedDiagnostics.Should().Contain(d => d.Id == "MUD0011");
        supportedDiagnostics.Should().Contain(d => d.Id == "MUD0012");
    }

    [Test]
    public void DiagnosticDescriptors_ShouldHaveCorrectProperties()
    {
        // Assert MUD0010
        ParameterStateAnalyzer.ReadDescriptor.Id.Should().Be("MUD0010");
        ParameterStateAnalyzer.ReadDescriptor.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        ParameterStateAnalyzer.ReadDescriptor.IsEnabledByDefault.Should().BeTrue();

        // Assert MUD0011
        ParameterStateAnalyzer.WriteDescriptor.Id.Should().Be("MUD0011");
        ParameterStateAnalyzer.WriteDescriptor.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        ParameterStateAnalyzer.WriteDescriptor.IsEnabledByDefault.Should().BeTrue();

        // Assert MUD0012
        ParameterStateAnalyzer.ExternalAccessDescriptor.Id.Should().Be("MUD0012");
        ParameterStateAnalyzer.ExternalAccessDescriptor.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        ParameterStateAnalyzer.ExternalAccessDescriptor.IsEnabledByDefault.Should().BeTrue();
    }

    [Test]
    public void MUD0010_ReadInsideMethod_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public int GetCounter()
    {
        return Counter; // Should trigger MUD0010
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0010");
    }

    [Test]
    public void MUD0010_ReadInVariableAssignment_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void Method()
    {
        var x = Counter; // Should trigger MUD0010
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0010");
    }

    [Test]
    public void MUD0010_ReadAsMethodArgument_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void Method()
    {
        DoSomething(Counter); // Should trigger MUD0010
    }

    private void DoSomething(int value) { }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0010");
    }

    [Test]
    public void MUD0011_WriteInsideMethod_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void SetCounter(int value)
    {
        Counter = value; // Should trigger MUD0011
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0011");
    }

    [Test]
    public void MUD0011_CompoundAssignment_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void Increment()
    {
        Counter += 1; // Should trigger MUD0011
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0011");
    }

    [Test]
    public void MUD0011_Increment_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void Increment()
    {
        Counter++; // Should trigger MUD0011
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0011");
    }

    [Test]
    public void MUD0011_Decrement_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public void Decrement()
    {
        Counter--; // Should trigger MUD0011
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0011");
    }

    [Test]
    public void MUD0011_ConstructorAssignment_ShouldNotReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public MyComponent()
    {
        Counter = 0; // Should NOT trigger MUD0011 - constructor is allowed
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Where(d => d.Id == "MUD0011").Should().BeEmpty();
    }

    [Test]
    public void MUD0011_SetParametersAsyncAssignment_ShouldNotReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;
using System.Threading.Tasks;

class MyComponent
{
    [ParameterState]
    public int Counter { get; set; }

    public Task SetParametersAsync()
    {
        Counter = 5; // Should NOT trigger MUD0011 - SetParametersAsync is allowed
        return Task.CompletedTask;
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Where(d => d.Id == "MUD0011").Should().BeEmpty();
    }

    [Test]
    public void MUD0012_ExternalRead_ShouldReportDiagnostic()
    {
        var source = @"
using MudBlazor.State;

class ComponentA
{
    [ParameterState]
    public int Counter { get; set; }
}

class ComponentB
{
    private ComponentA _componentA = new ComponentA();

    public int GetExternalCounter()
    {
        return _componentA.Counter; // Should trigger MUD0012
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().ContainSingle(d => d.Id == "MUD0012");
    }

    [Test]
    public void MUD0010_ShouldNotReportForExternalAccess()
    {
        var source = @"
using MudBlazor.State;

class ComponentA
{
    [ParameterState]
    public int Counter { get; set; }
}

class ComponentB
{
    private ComponentA _componentA = new ComponentA();

    public int GetExternalCounter()
    {
        return _componentA.Counter; // Should trigger MUD0012, NOT MUD0010
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Where(d => d.Id == "MUD0010").Should().BeEmpty();
        diagnostics.Should().ContainSingle(d => d.Id == "MUD0012");
    }

    [Test]
    public void NoDiagnostic_WhenPropertyDoesNotHaveParameterStateAttribute()
    {
        var source = @"
class MyComponent
{
    public int Counter { get; set; }

    public int GetCounter()
    {
        return Counter; // Should NOT trigger any diagnostic
    }

    public void SetCounter(int value)
    {
        Counter = value; // Should NOT trigger any diagnostic
    }
}";

        var diagnostics = GetDiagnostics(source);

        diagnostics.Should().BeEmpty();
    }

    [Test]
    public void NoDiagnostic_WhenAttributeNotAvailable()
    {
        // Test with source that doesn't include ParameterStateAttribute at all
        var source = @"
class MyComponent
{
    public int Counter { get; set; }

    public int GetCounter()
    {
        return Counter;
    }
}";

        // Create a separate compilation without ParameterStateAttribute
        var analyzer = new ParameterStateAnalyzer();
        var projectId = ProjectId.CreateNewId("TestProject");
        var documentId = DocumentId.CreateNewId(projectId, "Test0.cs");

        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReferences(projectId, References)
            .AddDocument(documentId, "Test0.cs", SourceText.From(source));

        var project = solution.GetProject(projectId)!;
        var compilation = project.GetCompilationAsync().Result!;
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        diagnostics.Should().BeEmpty();
    }
}

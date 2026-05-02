// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using MudBlazor.UnitTests.Analyzers.Verifiers;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;
using VerifyCS = CSharpAnalyzerVerifier<MudBlazorAnalyzer::MudBlazor.Analyzers.PickerTypeArgumentAnalyzer>;

[TestFixture]
public class PickerTypeArgumentAnalyzerTests
{
    [Test]
    public void AnalyzerShouldReportSupportedDiagnostics()
    {
        var analyzer = new MudBlazorAnalyzer::MudBlazor.Analyzers.PickerTypeArgumentAnalyzer();
        var supported = analyzer.SupportedDiagnostics;

        Assert.That(supported, Has.Length.EqualTo(1));
        Assert.That(supported[0].Id, Is.EqualTo("MUD0003"));
        Assert.That(supported[0].DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Warning));
        Assert.That(supported[0].IsEnabledByDefault, Is.True);
    }

    [TestCase("System.DateTime")]
    [TestCase("System.DateTime?")]
    [TestCase("System.DateOnly")]
    [TestCase("System.DateOnly?")]
    [TestCase("System.DateTimeOffset")]
    [TestCase("System.DateTimeOffset?")]
    public async Task ValidTypeArgument_ShouldNotReport(string type)
    {
        var source = $@"
using MudBlazor;

class C
{{
    {type.Replace('.', '.')} _v;
    MudDatePicker<{type}> _p;
    MudDateRangePicker<{type}> _rp;
    DateRange<{type}> _r;
}}
";
        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestCase("string")]
    [TestCase("bool")]
    [TestCase("int")]
    [TestCase("System.TimeSpan")]
    public async Task InvalidTypeArgument_OnMudDatePicker_ShouldReport(string type)
    {
        var source = $@"
using MudBlazor;

class C
{{
    MudDatePicker<{{|#0:{type}|}}> _p;
}}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<TValue>", type);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task InvalidTypeArgument_OnMudDateRangePicker_ShouldReport()
    {
        var source = @"
using MudBlazor;

class C
{
    MudDateRangePicker<{|#0:string|}> _p;
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDateRangePicker<TValue>", "string");
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task InvalidTypeArgument_OnDateRange_ShouldReport()
    {
        var source = @"
using MudBlazor;

class C
{
    DateRange<{|#0:string|}> _r;
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("DateRange<TValue>", "string");
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task InvalidTypeArgument_OnTypeOf_ShouldReport()
    {
        var source = @"
using System;
using MudBlazor;

class C
{
    Type _t = typeof(MudDatePicker<{|#0:string|}>);
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<TValue>", "string");
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task InvalidTypeArgument_OnObjectCreation_ShouldReport()
    {
        var source = @"
using MudBlazor;

class C
{
    object _o = new MudDatePicker<{|#0:string|}>();
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<TValue>", "string");
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using MudBlazor.UnitTests.Analyzers.Verifiers;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;
using VerifyCS = CSharpCodeFixVerifier<
    MudBlazorAnalyzer::MudBlazor.Analyzers.PickerTypeArgumentAnalyzer,
    MudBlazorAnalyzer::MudBlazor.Analyzers.PickerTypeArgumentCodeFixProvider>;

[TestFixture]
public class PickerTypeArgumentCodeFixProviderTests
{
    [Test]
    public async Task FixToDateTimeNullable()
    {
        var source = @"
using MudBlazor;

class C
{
    MudDatePicker<{|#0:string|}> _p;
}
";
        var fixedSource = @"
using MudBlazor;

class C
{
    MudDatePicker<System.DateTime?> _p;
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<T>", "string");
        var test = new VerifyCS.Test
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionIndex = 0, // first replacement: DateTime?
        };
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task FixToDateOnlyNullable()
    {
        var source = @"
using MudBlazor;

class C
{
    MudDatePicker<{|#0:string|}> _p;
}
";
        var fixedSource = @"
using MudBlazor;

class C
{
    MudDatePicker<System.DateOnly?> _p;
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<T>", "string");
        var test = new VerifyCS.Test
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionIndex = 1, // second: DateOnly?
        };
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task FixToDateTimeOffsetNullable()
    {
        var source = @"
using MudBlazor;

class C
{
    MudDatePicker<{|#0:string|}> _p;
}
";
        var fixedSource = @"
using MudBlazor;

class C
{
    MudDatePicker<System.DateTimeOffset?> _p;
}
";
        var expected = VerifyCS.Diagnostic("MUD0003").WithLocation(0).WithArguments("MudDatePicker<T>", "string");
        var test = new VerifyCS.Test
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionIndex = 2, // third: DateTimeOffset?
        };
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(CancellationToken.None);
    }
}

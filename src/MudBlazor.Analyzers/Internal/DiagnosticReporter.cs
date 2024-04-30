// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Diagnostics;


namespace MudBlazor.Analyzers.Internal;

internal readonly struct DiagnosticReporter
{
    private readonly Action<Diagnostic> _reportDiagnostic;

    public DiagnosticReporter(SymbolAnalysisContext context)
    {
        _reportDiagnostic = context.ReportDiagnostic;
        CancellationToken = context.CancellationToken;
    }

    public DiagnosticReporter(OperationAnalysisContext context)
    {
        _reportDiagnostic = context.ReportDiagnostic;
        CancellationToken = context.CancellationToken;
    }

    public DiagnosticReporter(OperationBlockAnalysisContext context)
    {
        _reportDiagnostic = context.ReportDiagnostic;
        CancellationToken = context.CancellationToken;
    }

    public DiagnosticReporter(SyntaxNodeAnalysisContext context)
    {
        _reportDiagnostic = context.ReportDiagnostic;
        CancellationToken = context.CancellationToken;
    }

    public DiagnosticReporter(CompilationAnalysisContext context)
    {
        _reportDiagnostic = context.ReportDiagnostic;
        CancellationToken = context.CancellationToken;
    }

    public CancellationToken CancellationToken { get; }

    public void ReportDiagnostic(Diagnostic diagnostic) => _reportDiagnostic(diagnostic);

    public static implicit operator DiagnosticReporter(SymbolAnalysisContext context) => new(context);
    public static implicit operator DiagnosticReporter(OperationAnalysisContext context) => new(context);
    public static implicit operator DiagnosticReporter(OperationBlockAnalysisContext context) => new(context);
    public static implicit operator DiagnosticReporter(SyntaxNodeAnalysisContext context) => new(context);
    public static implicit operator DiagnosticReporter(CompilationAnalysisContext context) => new(context);
}

namespace BlazorFiddlePoC.Shared
{
    using Microsoft.AspNetCore.Razor.Language;
    using Microsoft.CodeAnalysis;

    public class CompilationDiagnostics
    {
        public string Code { get; set; }

        public DiagnosticSeverity Severity { get; set; }

        public string Description { get; set; }

        public int Line { get; set; }

        public string File { get; set; }

        public CompilationDiagnosticsKind Kind { get; set; }

        public static CompilationDiagnostics FromCSharpDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            return new CompilationDiagnostics
            {
                Code = diagnostic.Descriptor.Id,
                Severity = diagnostic.Severity,
                Description = diagnostic.GetMessage(),
                Line = diagnostic.Location.GetMappedLineSpan().StartLinePosition.Line,
                Kind = CompilationDiagnosticsKind.CSharp,
            };
        }

        public static CompilationDiagnostics FromRazorDiagnostic(RazorDiagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            return new CompilationDiagnostics
            {
                Code = diagnostic.Id,
                Severity = (DiagnosticSeverity)diagnostic.Severity,
                Description = diagnostic.GetMessage(),
                Line = diagnostic.Span.LineIndex, // TODO: Math
                Kind = CompilationDiagnosticsKind.Razor,
            };
        }
    }
}

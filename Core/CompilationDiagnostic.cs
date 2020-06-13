namespace BlazorRepl.Core
{
    using Microsoft.AspNetCore.Razor.Language;
    using Microsoft.CodeAnalysis;

    public class CompilationDiagnostic
    {
        public string Code { get; set; }

        public DiagnosticSeverity Severity { get; set; }

        public string Description { get; set; }

        public int? Line { get; set; }

        public string File { get; set; }

        public CompilationDiagnosticKind Kind { get; set; }

        public static CompilationDiagnostic FromCSharpDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            return new CompilationDiagnostic
            {
                Code = diagnostic.Descriptor.Id,
                Severity = diagnostic.Severity,
                Description = diagnostic.GetMessage(),
                Line = diagnostic.Location.GetMappedLineSpan().StartLinePosition.Line,
                Kind = CompilationDiagnosticKind.CSharp,
            };
        }

        public static CompilationDiagnostic FromRazorDiagnostic(RazorDiagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            return new CompilationDiagnostic
            {
                Code = diagnostic.Id,
                Severity = (DiagnosticSeverity)diagnostic.Severity,
                Description = diagnostic.GetMessage(),

                // Line = diagnostic.Span.LineIndex, // TODO: Find a way to calculate this
                Kind = CompilationDiagnosticKind.Razor,
            };
        }
    }
}

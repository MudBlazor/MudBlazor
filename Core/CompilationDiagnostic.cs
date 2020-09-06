namespace BlazorRepl.Core
{
    using System.IO;
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

        internal static CompilationDiagnostic FromCSharpDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            var mappedLineSpan = diagnostic.Location.GetMappedLineSpan();
            var file = Path.GetFileName(mappedLineSpan.Path);
            var line = mappedLineSpan.StartLinePosition.Line;

            if (file != CoreConstants.MainComponentFilePath)
            {
                // Make it 1-based. Skip the main component where we add @page directive line
                line++;
            }

            return new CompilationDiagnostic
            {
                Kind = CompilationDiagnosticKind.CSharp,
                Code = diagnostic.Descriptor.Id,
                Severity = diagnostic.Severity,
                Description = diagnostic.GetMessage(),
                File = file,
                Line = line,
            };
        }

        internal static CompilationDiagnostic FromRazorDiagnostic(RazorDiagnostic diagnostic)
        {
            if (diagnostic == null)
            {
                return null;
            }

            return new CompilationDiagnostic
            {
                Kind = CompilationDiagnosticKind.Razor,
                Code = diagnostic.Id,
                Severity = (DiagnosticSeverity)diagnostic.Severity,
                Description = diagnostic.GetMessage(),
                File = Path.GetFileName(diagnostic.Span.FilePath),

                // Line = diagnostic.Span.LineIndex, // TODO: Find a way to calculate this
            };
        }
    }
}

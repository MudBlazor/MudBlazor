using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace BlazorFiddlePoC.Shared
{
    public class CompileToCSharpResult
    {
        // A compilation that can be used *with* this code to compile an assembly
        public Compilation BaseCompilation { get; set; }

        public string Code { get; set; }

        public IEnumerable<RazorDiagnostic> Diagnostics { get; set; }
    }
}

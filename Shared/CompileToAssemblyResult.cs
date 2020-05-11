using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace BlazorFiddlePoC.Shared
{
    public class CompileToAssemblyResult
    {
        public Compilation Compilation { get; set; }

        public IEnumerable<CompilationDiagnostic> Diagnostics { get; set; }

        public byte[] AssemblyBytes { get; set; }
    }
}

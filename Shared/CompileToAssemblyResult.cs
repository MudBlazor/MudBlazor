using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BlazorRepl.Shared
{
    public class CompileToAssemblyResult
    {
        public CompileToAssemblyResult()
        {
            this.Diagnostics = Enumerable.Empty<CompilationDiagnostic>();
        }

        public Compilation Compilation { get; set; }

        public IEnumerable<CompilationDiagnostic> Diagnostics { get; set; }

        public byte[] AssemblyBytes { get; set; }
    }
}

namespace BlazorRepl.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

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

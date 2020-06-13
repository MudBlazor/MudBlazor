namespace BlazorRepl.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public class CompileToCSharpResult
    {
        public CompileToCSharpResult()
        {
            this.Diagnostics = Enumerable.Empty<CompilationDiagnostic>();
        }

        public Compilation BaseCompilation { get; set; }

        public string Code { get; set; }

        public IEnumerable<CompilationDiagnostic> Diagnostics { get; set; }
    }
}

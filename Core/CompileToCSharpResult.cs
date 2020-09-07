namespace BlazorRepl.Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal class CompileToCSharpResult
    {
        public CompileToCSharpResult()
        {
            this.Diagnostics = Enumerable.Empty<CompilationDiagnostic>();
        }

        public string Code { get; set; }

        public IEnumerable<CompilationDiagnostic> Diagnostics { get; set; }
    }
}

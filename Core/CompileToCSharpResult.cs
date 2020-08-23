namespace BlazorRepl.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Razor.Language;

    internal class CompileToCSharpResult
    {
        public CompileToCSharpResult()
        {
            this.Diagnostics = Enumerable.Empty<CompilationDiagnostic>();
        }

        public RazorProjectItem ProjectItem { get; set; }

        public string Code { get; set; }

        public IEnumerable<CompilationDiagnostic> Diagnostics { get; set; }
    }
}

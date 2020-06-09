using BlazorRepl.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRepl.Client.Components
{
    public partial class ErrorList
    {
        [Parameter]
        public IReadOnlyCollection<CompilationDiagnostic> Diagnostics { get; set; } = Array.Empty<CompilationDiagnostic>();

        [Parameter]
        public int UserComponentCodeStartLine { get; set; }

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback<bool> ShowChanged { get; set; }

        public int ErrorsCount { get; set; }

        public int WarningsCount { get; set; }

        public bool ShowIcon => Diagnostics.Any();

        protected override Task OnInitializedAsync()
        {
            ErrorsCount = Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error);
            WarningsCount = Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Warning);

            return base.OnInitializedAsync();
        }

        public Task ToggleDiagnostics()
        {
            Show = !Show;
            return ShowChanged.InvokeAsync(Show);
        }
    }
}

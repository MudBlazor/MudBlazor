// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Threading;

namespace MudBlazor.UnitTests.Analyzers.Helpers
{
#nullable enable
    internal class ProjectCompilation
    {
        internal MSBuildWorkspace Workspace { get; private set; }
        internal Project Project { get; private set; }
        internal Compilation Compilation { get; private set; }
        internal ImmutableArray<AdditionalText> AdditionalTexts { get; private set; }
        internal CompilationWithAnalyzers? CompilationWithAnalyzers { get; private set; }

        private ProjectCompilation(MSBuildWorkspace workspace, Project project, Compilation compilation, ImmutableArray<AdditionalText> additionalTexts)
        {
            Workspace = workspace;
            Project = project;
            Compilation = compilation;
            AdditionalTexts = additionalTexts;
        }

        internal static async Task<ProjectCompilation> CreateAsync(string projectPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPath);

            project.Should().NotBeNull("Project null");
            project!.SupportsCompilation.Should().BeTrue("Project compilation not supported");

            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            compilation.Should().NotBeNull("Compilation null");

            var additionalTexts = ImmutableArray<AdditionalText>.Empty;
            foreach (var document in project.AdditionalDocuments)
            {
                if (document.FilePath is not null)
                    additionalTexts = additionalTexts.Add(new TestAdditionalText(document.FilePath, await document.GetTextAsync()));
            }

            return new ProjectCompilation(workspace, project, compilation!, additionalTexts);
        }

        internal async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions analyzerOptions)
        {
            CompilationWithAnalyzers = Compilation.WithAnalyzers(analyzers, analyzerOptions);
            return await CompilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

#nullable restore
}

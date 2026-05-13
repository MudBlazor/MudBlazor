using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MudBlazor.UnitTests.Analyzers.Internal;

extern alias MudBlazorAnalyzer;

#nullable enable
internal static class MudComponentUnknownParametersAnalyzerFixture
{
    private static readonly ImmutableArray<MetadataReference> _references = CreateReferences();

    internal static async Task<IReadOnlyList<Diagnostic>> RunAsync(
        string generatedComponentSource,
        MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern allowedAttributePattern,
        string? allowedAttributeList = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            generatedComponentSource,
            new CSharpParseOptions(LanguageVersion.Preview),
            path: "/Generated/AttributeTest.generated.cs");

        var compilation = CSharpCompilation.Create(
            assemblyName: "MudBlazor.UnitTests.Analyzers.Generated",
            syntaxTrees: [syntaxTree],
            references: _references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var diagnostics = await compilation.WithAnalyzers(
                [new MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer()],
                TestAnalyzerOptions.Create(allowedAttributePattern, ImmutableArray<AdditionalText>.Empty, allowedAttributeList))
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);

        return diagnostics
            .OrderBy(x => x.AdditionalLocations[0].SourceSpan.Start)
            .ToArray();
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var referencePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?.Split(Path.PathSeparator) ?? [])
        {
            referencePaths.Add(path);
        }

        referencePaths.Add(typeof(object).Assembly.Location);
        referencePaths.Add(typeof(Enumerable).Assembly.Location);
        referencePaths.Add(typeof(Microsoft.AspNetCore.Components.ComponentBase).Assembly.Location);
        referencePaths.Add(typeof(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder).Assembly.Location);
        referencePaths.Add(typeof(Microsoft.AspNetCore.Components.EventCallback).Assembly.Location);
        referencePaths.Add(typeof(MudBlazor._Imports).Assembly.Location);

        return referencePaths
            .Select(path => MetadataReference.CreateFromFile(path))
            .Cast<MetadataReference>()
            .ToImmutableArray();
    }
}
#nullable restore

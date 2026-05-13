using AwesomeAssertions;
using Microsoft.CodeAnalysis;

namespace MudBlazor.UnitTests.Analyzers.Internal;

extern alias MudBlazorAnalyzer;

#nullable enable
internal sealed record ExpectedDiagnostic(string AttributeName, string ComponentName)
{
    internal static void Compare(
        IReadOnlyList<Diagnostic> diagnostics,
        IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics,
        string expectedClassName)
    {
        diagnostics.Should().HaveCount(expectedDiagnostics.Count);

        foreach (var diagnostic in diagnostics)
        {
            diagnostic.Id.Should().Be(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.DiagnosticId);
            diagnostic.Properties[MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey]
                .Should().Be(expectedClassName);
        }

        foreach (var expectedDiagnostic in expectedDiagnostics)
        {
            diagnostics.Should().ContainSingle(x =>
                x.GetMessage().StartsWith(
                    $"Illegal Attribute '{expectedDiagnostic.AttributeName}' on '{expectedDiagnostic.ComponentName}'",
                    StringComparison.Ordinal));
        }
    }
}
#nullable restore

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace MudBlazor.Analyzers;

/// <summary>
/// Analyzer that enforces a whitelisted T on <c>MudDatePicker&lt;T&gt;</c>,
/// <c>MudDateRangePicker&lt;T&gt;</c>, and <c>DateRange&lt;T&gt;</c>.
/// Permitted: <c>DateTime</c>, <c>DateOnly</c>, <c>DateTimeOffset</c>, and their nullable variants.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class PickerTypeArgumentAnalyzer : DiagnosticAnalyzer
{
    private const string MudDatePickerMetadataName = "MudBlazor.MudDatePicker`1";
    private const string MudDateRangePickerMetadataName = "MudBlazor.MudDateRangePicker`1";
    private const string DateRangeMetadataName = "MudBlazor.DateRange`1";

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var datePicker = context.Compilation.GetTypeByMetadataName(MudDatePickerMetadataName);
        var dateRangePicker = context.Compilation.GetTypeByMetadataName(MudDateRangePickerMetadataName);
        var dateRange = context.Compilation.GetTypeByMetadataName(DateRangeMetadataName);

        if (datePicker is null && dateRangePicker is null && dateRange is null)
        {
            // Consumer's project doesn't reference the generic MudBlazor pickers — nothing to analyze.
            return;
        }

        var targetTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        if (datePicker is not null) targetTypes.Add(datePicker);
        if (dateRangePicker is not null) targetTypes.Add(dateRangePicker);
        if (dateRange is not null) targetTypes.Add(dateRange);

        var whitelist = BuildWhitelist(context.Compilation);
        var state = new AnalyzerState(targetTypes, whitelist);

        // Catch direct C# usages: new MudDatePicker<X>(), MudDatePicker<X> field, typeof(MudDatePicker<X>), etc.
        context.RegisterSyntaxNodeAction(state.AnalyzeGenericName, SyntaxKind.GenericName);
    }

    private static ImmutableHashSet<ITypeSymbol> BuildWhitelist(Compilation compilation)
    {
        var builder = ImmutableHashSet.CreateBuilder<ITypeSymbol>(SymbolEqualityComparer.Default);

        builder.Add(compilation.GetSpecialType(SpecialType.System_DateTime));

        var dateTimeOffset = compilation.GetTypeByMetadataName("System.DateTimeOffset");
        if (dateTimeOffset is not null)
        {
            builder.Add(dateTimeOffset);
        }

        // DateOnly is .NET 6+; absent on older TFMs.
        var dateOnly = compilation.GetTypeByMetadataName("System.DateOnly");
        if (dateOnly is not null)
        {
            builder.Add(dateOnly);
        }

        return builder.ToImmutable();
    }

    private sealed class AnalyzerState
    {
        private readonly HashSet<INamedTypeSymbol> _targetTypes;
        private readonly ImmutableHashSet<ITypeSymbol> _whitelist;

        public AnalyzerState(HashSet<INamedTypeSymbol> targetTypes, ImmutableHashSet<ITypeSymbol> whitelist)
        {
            _targetTypes = targetTypes;
            _whitelist = whitelist;
        }

        public void AnalyzeGenericName(SyntaxNodeAnalysisContext context)
        {
            var genericName = (GenericNameSyntax)context.Node;

            // GenericNameSyntax fires once per appearance, including on type-of, new, field decl, etc.
            if (context.SemanticModel.GetSymbolInfo(genericName, context.CancellationToken).Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            if (typeSymbol.OriginalDefinition is null || !_targetTypes.Contains(typeSymbol.OriginalDefinition))
            {
                return;
            }

            if (typeSymbol.TypeArguments.Length != 1)
            {
                return;
            }

            var typeArgument = typeSymbol.TypeArguments[0];

            // Skip when the type argument is itself an open type parameter
            // (e.g. the picker's own class declaration `MudDatePicker<T> : MudBaseDatePicker<T>`,
            // `seealso cref="MudDatePicker{T}"`, or any other place where T isn't a concrete type yet).
            // The diagnostic only fires for closed generics.
            if (typeArgument is ITypeParameterSymbol)
            {
                return;
            }

            if (IsWhitelisted(typeArgument))
            {
                return;
            }

            // Build a friendly type name for the message — e.g. "MudDatePicker<T>".
            var componentName = typeSymbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            // Report at the type-argument's syntax location for a tight IDE squiggle.
            var typeArgumentSyntax = genericName.TypeArgumentList.Arguments.Count == 1
                ? genericName.TypeArgumentList.Arguments[0]
                : null;
            var location = typeArgumentSyntax?.GetLocation() ?? genericName.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptor,
                location,
                componentName,
                typeArgument.ToDisplayString()));
        }

        private bool IsWhitelisted(ITypeSymbol type)
        {
            // Strip nullable annotation: Nullable<T> → T.
            var underlying = type;
            if (type is INamedTypeSymbol named &&
                named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
                named.TypeArguments.Length == 1)
            {
                underlying = named.TypeArguments[0];
            }

            return _whitelist.Contains(underlying);
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.SourceGenerator;

[Generator]
public sealed class FastEnumDescriptionGenerator : IIncrementalGenerator
{
    private const string ExtensionClassName = "SourceGeneratorEnumExtensions";
    private const string DescriptionAttribute = "System.ComponentModel.DescriptionAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enums =
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: (syntaxNode, _) => syntaxNode is EnumDeclarationSyntax,
                    transform: GetEnumData)
                .Where(static enumData => enumData is not null);
        context.RegisterSourceOutput(enums, Build);
    }

    private static EnumData? GetEnumData(GeneratorSyntaxContext generatorSyntaxContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var enumDeclarationSyntax = (EnumDeclarationSyntax)generatorSyntaxContext.Node;
        var semanticModel = generatorSyntaxContext.SemanticModel.Compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
        var declaredSymbol = semanticModel.GetDeclaredSymbol(enumDeclarationSyntax);
        return declaredSymbol is null ? null : GetEnumDataFromSymbol(declaredSymbol);
    }

    private static void Build(SourceProductionContext context, EnumData? enumData)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        if (!enumData.HasValue) return;

        var data = enumData.Value;
        var sourceCode = SourceCodeBuilder.Build(in data);
        context.AddSource($"{data.Classname}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));

        // Report a warning if the enum has inconsistent usage of DescriptionAttribute
        if (data.InconsistentDescriptionAttributeUsage)
        {
            var diagnostic = DiagnosticHelper.CreateDescriptionWarning(data.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static EnumData? GetEnumDataFromSymbol(INamespaceOrTypeSymbol enumSymbol)
    {
        var accessModifier = GetAccessModifier(enumSymbol);
        if (accessModifier is null)
        {
            return null;
        }

        var classname = $"{enumSymbol.Name}{ExtensionClassName}";
        var @namespace = enumSymbol.ContainingNamespace.ToString();
        var name = enumSymbol.ToString();
        var fieldSymbols = enumSymbol.GetMembers().OfType<IFieldSymbol>().ToArray();
        var enumMembers = GetMembers(fieldSymbols).ToArray();
        return enumMembers.Length > 0 ? new EnumData(classname, name, @namespace, accessModifier, enumMembers, enumMembers.Length != fieldSymbols.Length) : null;
    }

    private static string? GetAccessModifier(ISymbol enumSymbol)
    {
        var accessibility = enumSymbol.DeclaredAccessibility;
        if (enumSymbol.ContainingType is not null && accessibility > enumSymbol.ContainingType.DeclaredAccessibility)
        {
            accessibility = enumSymbol.ContainingType.DeclaredAccessibility;
        }

        return accessibility switch

        {
            Accessibility.Internal => "internal",
            Accessibility.Public => "public",
            _ => null
        };
    }

    private static IEnumerable<EnumMember> GetMembers(IFieldSymbol[] symbols)
    {
        foreach (var enumMember in symbols)
        {
            foreach (var attribute in enumMember.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() == DescriptionAttribute &&
                    attribute.ConstructorArguments.Length > 0 &&
                    attribute.ConstructorArguments[0].Value is not null)
                {
                    var rawDescription = attribute.ConstructorArguments[0].Value!.ToString();
                    var description = SymbolDisplay.FormatLiteral(rawDescription, true);
                    var isKeyword = SyntaxFacts.GetKeywordKind(enumMember.Name) != SyntaxKind.None;
                    var memberName = isKeyword ? $"@{enumMember.Name}" : enumMember.Name;
                    var member = new EnumMember(memberName, description);
                    yield return member;
                }
            }
        }
    }
}

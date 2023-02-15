// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.SourceCodeGenerator;

[Generator]
public sealed class FastEnumDescriptionGenerator : IIncrementalGenerator
{
    private const string ExtensionClassName = "MudEnumExtensions";
    private const string DescriptionAttribute = "System.ComponentModel.DescriptionAttribute";
    private const string ExcludeFromCodeGeneratorAttribute = "MudBlazor.ExcludeFromCodeGeneratorAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enums =
            context.SyntaxProvider.CreateSyntaxProvider(
                    transform: GetEnumData,
                    predicate: (syntaxNode, _) => syntaxNode is EnumDeclarationSyntax)
                .Where(static enumData => enumData is not null);
        context.RegisterSourceOutput(enums, Build);
    }

    private static EnumData? GetEnumData(GeneratorSyntaxContext generatorSyntaxContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var enumDeclarationSyntax = (EnumDeclarationSyntax)generatorSyntaxContext.Node;
        var semanticModel = generatorSyntaxContext.SemanticModel.Compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
        {
            return null;
        }

        return HasExcludeFromCodeGeneratorAttribute(enumSymbol) ? null : GetEnumDataFromSymbol(enumSymbol);
    }

    private static void Build(SourceProductionContext context, EnumData? enumData)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        if (enumData.HasValue)
        {
            var data = enumData.Value;
            var sourceCode = SourceCodeBuilder.Build(ref data);
            context.AddSource($"{data.Classname}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static EnumData? GetEnumDataFromSymbol(INamespaceOrTypeSymbol enumSymbol)
    {
        var classname = $"{enumSymbol.Name}{ExtensionClassName}";
        var @namespace = enumSymbol.ContainingNamespace.ToString();
        var name = enumSymbol.ToString();
        var accessModifier = GetAccessModifier(enumSymbol);
        if (accessModifier == null)
        {
            return null;
        }

        return new EnumData(classname, name, @namespace, accessModifier, GetMembers(enumSymbol));
    }

    private static string? GetAccessModifier(ISymbol enumSymbol)
    {
        var accessibility = enumSymbol.DeclaredAccessibility;
        if (enumSymbol.ContainingType != null && accessibility > enumSymbol.ContainingType.DeclaredAccessibility)
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

    private static IEnumerable<EnumMember> GetMembers(INamespaceOrTypeSymbol symbol)
    {
        foreach (var enumMember in symbol.GetMembers())
        {
            if (enumMember is IFieldSymbol)
            {
                foreach (var attribute in enumMember.GetAttributes())
                {
                    if (attribute.AttributeClass?.ToDisplayString() == DescriptionAttribute && attribute.ConstructorArguments[0].Value != null)
                    {
                        var description = attribute.ConstructorArguments[0].Value!.ToString();
                        var member = new EnumMember(enumMember.Name, description);
                        yield return member;
                    }
                }
            }
        }
    }

    private static bool HasExcludeFromCodeGeneratorAttribute(ISymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == ExcludeFromCodeGeneratorAttribute)
            {
                return true;
            }
        }

        return false;
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.SourceGenerator;

[Generator]
public sealed class FastEnumDescriptionGenerator : IIncrementalGenerator
{
    private const string ExtensionClassName = "SourceGeneratorEnumExtensions";
    private const string DescriptionAttribute = "System.ComponentModel.DescriptionAttribute";
    private const string ExcludeFromCodeGeneratorAttribute = "MudBlazor.ExcludeFromCodeGeneratorAttribute";

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
            var sourceCode = SourceCodeBuilder.Build(in data);
            context.AddSource($"{data.Classname}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
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
        var members = GetMembers(enumSymbol).ToArray();
        return members.Length > 0 ? new EnumData(classname, name, @namespace, accessModifier, members) : null;
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

    private static IEnumerable<EnumMember> GetMembers(INamespaceOrTypeSymbol symbol)
    {
        foreach (var enumMember in symbol.GetMembers())
        {
            if (enumMember is not IFieldSymbol) continue;

            foreach (var attribute in enumMember.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() == DescriptionAttribute &&
                    attribute.ConstructorArguments.Length > 0 &&
                    attribute.ConstructorArguments[0].Value is not null)
                {
                    var description = attribute.ConstructorArguments[0].Value!.ToString();
                    var member = new EnumMember(enumMember.Name, description);
                    yield return member;
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

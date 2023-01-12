// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MudBlazor.SourceGenerator.Extensions;
using System.Text.RegularExpressions;

namespace MudBlazor.SourceGenerator.EnumGenerators
{
    [Generator]
    public class DescriptionEnumGenerator : ISourceGenerator
    {
        private const string FILE_NAME = "GeneratedEnumExtensions.g.cs";
        private const string CLASS_NAME = "GeneratedEnumExtensions";
        private const string EXTENSIONS_NAME = "Extensions";
        private const string MUDBLAZOR_NAME = "MudBlazor";

        public void Execute(GeneratorExecutionContext context)
        {
            var enums = GetAllEnumsButNotFlags(context);
            CreateExtensionsFileForEnums(enums, context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        private static void CreateExtensionsFileForEnums(List<EnumDeclarationSyntax> enums, GeneratorExecutionContext context)
        {
            if (!enums.Any()) return;

            var assemblyName = context.Compilation.AssemblyName;

            var sb = new StringBuilder($@"namespace {assemblyName ?? MUDBLAZOR_NAME}.{EXTENSIONS_NAME}
{{
    /// <summary>
    /// Generated enum extensions class
    /// </summary>
    public static class {CLASS_NAME}
    {{
");

            foreach (var enumSyntax in enums)
            {
                AppendExtensionMethodForEnum(sb, enumSyntax, context);
            }

            sb.Append(@"
    }
}
");
            context.AddSource(FILE_NAME,
                SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private static void AppendExtensionMethodForEnum(StringBuilder sb, EnumDeclarationSyntax enumSyntax, GeneratorExecutionContext context)
        {
            var semanticModel = context.Compilation.GetSemanticModel(enumSyntax.SyntaxTree);
            var declaredSymbol = semanticModel.GetDeclaredSymbol(enumSyntax);
            string symbolFullName;

            if (declaredSymbol == null)
                return;

            var accessibility = declaredSymbol.DeclaredAccessibility;

            if (declaredSymbol.ContainingType == null)
                symbolFullName = $"{declaredSymbol.ContainingNamespace}.{declaredSymbol.Name}";
            else
            {
                symbolFullName = $"{declaredSymbol.ContainingType}.{declaredSymbol.Name}";
                if(accessibility > declaredSymbol.ContainingType.DeclaredAccessibility)
                    accessibility = declaredSymbol.ContainingType.DeclaredAccessibility;
            }

            if (accessibility != Accessibility.Internal && accessibility != Accessibility.Public)
                return;

            var accessibilityName = accessibility.ToString().ToLowerInvariant();

            var enumMembers = declaredSymbol.GetMembers();

            sb.Append($@"
        /// <summary>
        /// Gets the description from the enum value.
        /// </summary>
        /// <param name=""value""></param>
        /// <returns>The description or the value as lowercase if no description was found.</returns>
        {accessibilityName} static string ToDescriptionString(this {symbolFullName} value)
        {{
            return value switch
            {{
");

            foreach (var member in enumMembers)
            {
                AppendReturnValuesForEnumMember(sb, member, symbolFullName);
            }

            sb.Append(@"               _ => string.Empty    
            };
        }
");
        }

        private static void AppendReturnValuesForEnumMember(StringBuilder sb, ISymbol member, string symbolFullName)
        {
            if (member is not IFieldSymbol fieldSymbol || fieldSymbol.ConstantValue == null) return;

            var memberAttributes = member.GetAttributes();

            string returnValue;

            if (TryGetDescriptionValueFromMemberAttributes(memberAttributes, out var description))
                returnValue =description.Replace(@"""", @"\""");
            else
                returnValue = member.Name.ToLower();

            string memberName;
            if (member.Name.IsKeyword() || member.Name.IsContextualKeyword())
                memberName = $"@{member.Name.ToLower()}";
            else
                memberName = member.Name;

            sb.AppendLine($@"               {symbolFullName}.{memberName} => ""{returnValue}"",");
        }

        private static bool TryGetDescriptionValueFromMemberAttributes(ImmutableArray<AttributeData> attributes, out string description)
        {
            description = string.Empty;

            foreach (var attribute in attributes)
            {
                bool isDescriptionAttribute = attribute.AttributeClass != null &&
                        attribute.AttributeClass.Name == nameof(DescriptionAttribute);

                if (!isDescriptionAttribute)
                    continue;

                var descriptionValue = attribute.ConstructorArguments
                    .Where(x => x.Type!.SpecialType == SpecialType.System_String)
                    .FirstOrDefault().Value;

                if (descriptionValue is string attributeValue)
                {
                    description = attributeValue;
                    return true;
                }
            }
            return false;
        }

        private static List<EnumDeclarationSyntax> GetAllEnumsButNotFlags(GeneratorExecutionContext context)
        {
            var enums = GetAllEnums(context);
            var enumsWithoutFlags = RemoveEnumsWithFlagsAttribute(enums, context);

            return enumsWithoutFlags;
        }

        private static List<EnumDeclarationSyntax> GetAllEnums(GeneratorExecutionContext context)
        {
            var enums = new List<EnumDeclarationSyntax>();

            foreach (var syntaxTree in context.Compilation.SyntaxTrees)
            {
                enums.AddRange(syntaxTree.GetRoot().DescendantNodesAndSelf()
                    .OfType<EnumDeclarationSyntax>());
            }

            return enums;
        }

        private static List<EnumDeclarationSyntax> RemoveEnumsWithFlagsAttribute(List<EnumDeclarationSyntax> enums, GeneratorExecutionContext context)
        {
            var enumsWithoutFlags = new List<EnumDeclarationSyntax>();
            foreach (var item in enums)
            {

                if (item.AttributeLists.Count == 0)
                    enumsWithoutFlags.Add(item);
                else
                {
                    var semanticModel = context.Compilation.GetSemanticModel(item.SyntaxTree);
                    var declaredSymbol = semanticModel.GetDeclaredSymbol(item);

                    if (declaredSymbol == null)
                        continue;

                    var attributes = declaredSymbol.GetAttributes();

                    if (!attributes.Any(x => string.Equals(x.AttributeClass!.Name, nameof(FlagsAttribute), StringComparison.OrdinalIgnoreCase)))
                        enumsWithoutFlags.Add(item);
                }
            }
            return enumsWithoutFlags;
        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp;

namespace MudBlazor.Analyzers.Internal
{
    internal static class OperationExtensions
    {
        internal static INamedTypeSymbol? GetClassSymbol(this IOperation operation, OperationAnalysisContext context)
        {
            var classDeclaration = operation.Syntax.FindClass();

            if (classDeclaration is null)
                return null;

            return context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration);
        }

        internal static string? GetClassName(this IOperation invocation, OperationAnalysisContext context)
        {
            return invocation.GetClassSymbol(context)?.ToDisplayString();
        }

        internal static string? GetRazorFilePath(this IOperation invocation)
        {
            var root = (PragmaChecksumDirectiveTriviaSyntax?)invocation.Syntax.SyntaxTree.GetRoot()
                                    .GetFirstDirective(x => x.IsKind(SyntaxKind.PragmaChecksumDirectiveTrivia));

            if (root is null)
                return null;

            return root.File.ValueText;
        }
    }
}

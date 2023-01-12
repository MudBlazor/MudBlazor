// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp;

namespace MudBlazor.SourceGenerator.Extensions
{
    internal static class GeneratorKeywordExtensions
    {
        internal static bool IsKeyword(this string keyword)
        {
            var syntaxKind = SyntaxFacts.GetKeywordKind(keyword);
            return SyntaxFacts.IsKeywordKind(syntaxKind);
        }

        internal static bool IsContextualKeyword(this string keyword)
        {
            var syntaxKind = SyntaxFacts.GetContextualKeywordKind(keyword);
            return SyntaxFacts.IsContextualKeyword(syntaxKind);
        }
    }
}

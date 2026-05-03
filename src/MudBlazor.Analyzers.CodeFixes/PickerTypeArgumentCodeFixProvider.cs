// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MudBlazor.Analyzers;

/// <summary>
/// Code fix provider for MUD0003: replaces an unsupported T on
/// MudDatePicker / MudDateRangePicker / DateRange with one of the three
/// supported nullable date types.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PickerTypeArgumentCodeFixProvider)), Shared]
public sealed class PickerTypeArgumentCodeFixProvider : CodeFixProvider
{
    private static readonly (string DisplayName, string Title)[] s_replacements =
    {
        ("System.DateTime?",       "Use 'DateTime?' instead"),
        ("System.DateOnly?",       "Use 'DateOnly?' instead"),
        ("System.DateTimeOffset?", "Use 'DateTimeOffset?' instead"),
    };

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(PickerTypeArgumentAnalyzer.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

        // Find the GenericNameSyntax — it may be the node itself or one of its ancestors.
        var generic = node as GenericNameSyntax ?? node.FirstAncestorOrSelf<GenericNameSyntax>();
        if (generic is null || generic.TypeArgumentList.Arguments.Count != 1)
        {
            return;
        }

        foreach (var (displayName, title) in s_replacements)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: ct => ReplaceTypeArgumentAsync(context.Document, generic, displayName, ct),
                    equivalenceKey: title),
                diagnostic);
        }
    }

    private static async Task<Document> ReplaceTypeArgumentAsync(
        Document document, GenericNameSyntax generic, string newTypeFullName, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        // newTypeFullName may end with '?' (nullable). SyntaxFactory.ParseTypeName handles that.
        var newType = SyntaxFactory.ParseTypeName(newTypeFullName)
            .WithTriviaFrom(generic.TypeArgumentList.Arguments[0]);

        var newGeneric = generic.WithTypeArgumentList(
            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(newType)));

        var newRoot = root.ReplaceNode(generic, newGeneric);
        return document.WithSyntaxRoot(newRoot);
    }
}

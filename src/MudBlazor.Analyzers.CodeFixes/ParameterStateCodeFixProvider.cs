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
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace MudBlazor.Analyzers;

/// <summary>
/// Code fix provider for MUD0012: External access of a parameter state property.
/// Suggests replacing <c>instance.Property</c> with <c>instance.GetState(x => x.Property)</c>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterStateCodeFixProvider)), Shared]
public sealed class ParameterStateCodeFixProvider : CodeFixProvider
{
    private const string Title = "Use GetState to access ParameterState property";
    private const string MudBlazorExtensionsNamespace = "MudBlazor.Extensions";

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(ParameterStateAnalyzer.ExternalAccessDiagnosticId);

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
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the member access expression identified by the diagnostic
        // Use getInnermostNodeForTie to handle cases where multiple nodes cover the span
        var node = root.FindNode(diagnosticSpan, getInnermostNodeForTie: true);
        var memberAccess = FindMemberAccessExpression(node);

        if (memberAccess is null)
        {
            return;
        }

        // Register a code action that will invoke the fix
        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ReplaceWithGetStateAsync(context.Document, memberAccess, c),
                equivalenceKey: Title),
            diagnostic);
    }

    /// <summary>
    /// Finds the MemberAccessExpressionSyntax from the given node, handling various syntax tree structures.
    /// </summary>
    private static MemberAccessExpressionSyntax? FindMemberAccessExpression(SyntaxNode node)
    {
        // Direct match
        if (node is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess;
        }

        // Check parent (common case when node is an identifier)
        if (node.Parent is MemberAccessExpressionSyntax parentMemberAccess)
        {
            return parentMemberAccess;
        }

        // Check descendants (when the node spans more than the member access)
        foreach (var descendant in node.DescendantNodes())
        {
            if (descendant is MemberAccessExpressionSyntax descendantMemberAccess)
            {
                return descendantMemberAccess;
            }
        }

        // Walk up the tree to find the member access (for nested cases)
        var current = node;
        while (current is not null)
        {
            if (current is MemberAccessExpressionSyntax foundMemberAccess)
            {
                return foundMemberAccess;
            }
            current = current.Parent;
        }

        return null;
    }

    private static async Task<Document> ReplaceWithGetStateAsync(
        Document document,
        MemberAccessExpressionSyntax memberAccess,
        CancellationToken cancellationToken)
    {
        // Use SyntaxGenerator for proper import handling
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        // Get the instance expression (e.g., _componentA)
        var instanceExpression = memberAccess.Expression;

        // Get the property name (e.g., Counter)
        var propertyName = memberAccess.Name.Identifier.Text;

        // Create the lambda parameter: x
        var lambdaParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("x"));

        // Create the lambda body: x.PropertyName
        var lambdaBody = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName("x"),
            SyntaxFactory.IdentifierName(propertyName));

        // Create the lambda expression: x => x.PropertyName
        var lambda = SyntaxFactory.SimpleLambdaExpression(lambdaParameter, lambdaBody);

        // Create the argument list: (x => x.PropertyName)
        var argumentList = SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(lambda)));

        // Create the GetState method invocation: instance.GetState(x => x.PropertyName)
        var getStateInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                instanceExpression,
                SyntaxFactory.IdentifierName("GetState")),
            argumentList);

        // Preserve trivia from the original expression
        var newNode = getStateInvocation
            .WithLeadingTrivia(memberAccess.GetLeadingTrivia())
            .WithTrailingTrivia(memberAccess.GetTrailingTrivia())
            .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);

        // Replace the old member access with the new GetState invocation
        editor.ReplaceNode(memberAccess, newNode);

        // Get the modified document
        var newDocument = editor.GetChangedDocument();

        // Add the using directive using the proper API
        var root = await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is CompilationUnitSyntax compilationUnit)
        {
            var hasUsingDirective = compilationUnit.Usings.Any(u =>
                u.Name?.ToString() == MudBlazorExtensionsNamespace);

            if (!hasUsingDirective)
            {
                // Create the namespace import using SyntaxGenerator
                var namespaceImport = generator.NamespaceImportDeclaration(MudBlazorExtensionsNamespace);

                // Find the compilation unit and add the import
                var newCompilationUnit = (CompilationUnitSyntax)generator.AddNamespaceImports(
                    compilationUnit,
                    namespaceImport);

                return newDocument.WithSyntaxRoot(newCompilationUnit);
            }
        }

        return newDocument;
    }
}

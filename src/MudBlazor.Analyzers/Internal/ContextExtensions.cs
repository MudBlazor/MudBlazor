// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace MudBlazor.Analyzers.Internal;

internal static partial class ContextExtensions
{
    private static Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string?>? properties, string?[]? messageArgs)
    {
        return Diagnostic.Create(descriptor, location, properties, messageArgs);
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, string?[]? messageArgs = null)
        => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, string?[]? messageArgs = null)
    {
        var syntaxNode = syntaxReference.GetSyntax(context.CancellationToken);
        context.ReportDiagnostic(CreateDiagnostic(descriptor, syntaxNode.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, Location location, string?[]? messageArgs = null) => context.ReportDiagnostic(CreateDiagnostic(descriptor, location, ImmutableDictionary<string, string?>.Empty, messageArgs));
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, string?[]? messageArgs = null) => context.ReportDiagnostic(CreateDiagnostic(descriptor, location, properties, messageArgs));

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, SyntaxNode syntax, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, syntax.GetLocation(), messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntax, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, properties, syntax.GetLocation(), messageArgs);

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, SyntaxToken token, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, token.GetLocation(), messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, syntaxToken.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ISymbol symbol, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, string?[]? messageArgs = null)
    {
        foreach (var location in symbol.Locations)
        {
            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, reportOptions, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, string?[]? messageArgs = null)
    {
        foreach (var location in symbol.Locations)
        {
            if (reportOptions.HasFlag(DiagnosticFieldReportOptions.ReportOnReturnType))
            {
                var node = location.SourceTree?.GetRoot(context.CancellationToken).FindNode(location.SourceSpan);
                if (node is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type: not null and var type } })
                {
                    ReportDiagnostic(context, descriptor, properties, type.GetLocation(), messageArgs);
                    return;
                }
            }

            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, reportOptions, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, string?[]? messageArgs = null)
    {
        foreach (var location in symbol.Locations)
        {
            if (reportOptions.HasFlag(DiagnosticMethodReportOptions.ReportOnReturnType))
            {
                var node = location.SourceTree?.GetRoot(context.CancellationToken).FindNode(location.SourceSpan);
                if (node is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    ReportDiagnostic(context, descriptor, properties, methodDeclarationSyntax.ReturnType.GetLocation(), messageArgs);
                    return;
                }

                if (node is DelegateDeclarationSyntax delegateDeclarationSyntax)
                {
                    ReportDiagnostic(context, descriptor, properties, delegateDeclarationSyntax.ReturnType.GetLocation(), messageArgs);
                    return;
                }
            }

            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, reportOptions, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, string?[]? messageArgs = null)
    {
        foreach (var location in symbol.Locations)
        {
            if (reportOptions.HasFlag(DiagnosticParameterReportOptions.ReportOnType))
            {
                var node = location.SourceTree?.GetRoot(context.CancellationToken).FindNode(location.SourceSpan);
                if (node is ParameterSyntax { Type: not null and var parameterType })
                {
                    ReportDiagnostic(context, descriptor, properties, parameterType.GetLocation(), messageArgs);
                    return;
                }
            }

            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, string?[]? messageArgs = null) => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, reportOptions, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, string?[]? messageArgs = null)
    {
        foreach (var location in symbol.Locations)
        {
            if (reportOptions.HasFlag(DiagnosticPropertyReportOptions.ReportOnReturnType))
            {
                var node = location.SourceTree?.GetRoot(context.CancellationToken).FindNode(location.SourceSpan);
                if (node is PropertyDeclarationSyntax { Type: not null and var returnType })
                {
                    ReportDiagnostic(context, descriptor, properties, returnType.GetLocation(), messageArgs);
                    return;
                }

                if (node is IndexerDeclarationSyntax { Type: not null and var returnType2 })
                {
                    ReportDiagnostic(context, descriptor, properties, returnType2.GetLocation(), messageArgs);
                    return;
                }
            }

            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, IOperation operation, string?[]? messageArgs = null)
        => ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, operation, messageArgs);
    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, string?[]? messageArgs = null)
        => context.ReportDiagnostic(CreateDiagnostic(descriptor, operation.Syntax.GetLocation(), properties, messageArgs));

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, string?[]? messageArgs = null)
    {
        if (options.HasFlag(DiagnosticMethodReportOptions.ReportOnMethodName) && operation.Syntax is LocalFunctionStatementSyntax memberAccessExpression)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessExpression.Identifier.GetLocation(), properties, messageArgs));
            return;
        }

        if (options.HasFlag(DiagnosticMethodReportOptions.ReportOnReturnType) && operation.Syntax is LocalFunctionStatementSyntax memberAccessExpression2)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessExpression2.ReturnType.GetLocation(), properties, messageArgs));
            return;
        }

        context.ReportDiagnostic(descriptor, properties, operation, messageArgs);
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[]? messageArgs)
    {
        if (options.HasFlag(DiagnosticInvocationReportOptions.ReportOnMember) &&
            operation.Syntax.ChildNodes().FirstOrDefault() is MemberAccessExpressionSyntax memberAccessExpression)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessExpression.Name.GetLocation(), properties, messageArgs));
            return;
        }

        context.ReportDiagnostic(descriptor, properties, operation, messageArgs);
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, attribute, messageArgs);
    }

    public static void ReportDiagnostic(this DiagnosticReporter context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
    {
        if (attribute.ApplicationSyntaxReference is not null)
        {
            context.ReportDiagnostic(descriptor, properties, attribute.ApplicationSyntaxReference, messageArgs);
        }
    }

}

internal static partial class ContextExtensions
{
    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, location, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, location, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, operation, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, attribute, messageArgs);

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, attribute, messageArgs);
    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, location, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, location, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, operation, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, attribute, messageArgs);

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, attribute, messageArgs);
    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, location, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, location, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, operation, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, attribute, messageArgs);

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, attribute, messageArgs);
    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, location, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, location, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, operation, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, attribute, messageArgs);

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, attribute, messageArgs);
    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxToken, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxNode, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IFieldSymbol symbol, DiagnosticFieldReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IMethodSymbol symbol, DiagnosticMethodReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IParameterSymbol symbol, DiagnosticParameterReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IPropertySymbol symbol, DiagnosticPropertyReportOptions reportOptions, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, symbol, reportOptions, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, location, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, location, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxReference syntaxReference, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, syntaxReference, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticInvocationReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ILocalFunctionOperation operation, DiagnosticMethodReportOptions options, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, options, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, operation, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string?[] messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, operation, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, attribute, messageArgs);

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, AttributeData attribute, params string?[]? messageArgs)
        => ReportDiagnostic(new DiagnosticReporter(context), descriptor, properties, attribute, messageArgs);
}

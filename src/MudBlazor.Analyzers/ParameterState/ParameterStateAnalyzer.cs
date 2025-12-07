// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using MudBlazor.State;

namespace MudBlazor.Analyzers;

/// <summary>
/// Analyzer that enforces correct usage of the ParameterStateAttribute.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class ParameterStateAnalyzer : DiagnosticAnalyzer
{
    private const string ParameterStateAttributeFullName = "MudBlazor.State.ParameterStateAttribute";
    private const string ParameterUsagePropertyName = "ParameterUsage";
    private const string SetParametersAsyncMethodName = "SetParametersAsync";
    private const string GetStateMethodName = "GetState";

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
        // Try to find the ParameterStateAttribute by metadata name
        var parameterStateAttributeSymbol = context.Compilation.GetTypeByMetadataName(ParameterStateAttributeFullName);
        if (parameterStateAttributeSymbol is null)
        {
            // ParameterStateAttribute is not available, nothing to analyze
            return;
        }

        var analyzerContext = new AnalyzerState(parameterStateAttributeSymbol);

        // Register for property reference operations
        context.RegisterOperationAction(
            ctx => analyzerContext.AnalyzePropertyReference(ctx),
            OperationKind.PropertyReference);

        // Register for simple assignment operations
        context.RegisterOperationAction(
            ctx => analyzerContext.AnalyzeSimpleAssignment(ctx),
            OperationKind.SimpleAssignment);

        // Register for compound assignment operations
        context.RegisterOperationAction(
            ctx => analyzerContext.AnalyzeCompoundAssignment(ctx),
            OperationKind.CompoundAssignment);

        // Register for increment operations
        context.RegisterOperationAction(
            ctx => analyzerContext.AnalyzeIncrement(ctx),
            OperationKind.Increment);

        // Register for decrement operations
        context.RegisterOperationAction(
            ctx => analyzerContext.AnalyzeDecrement(ctx),
            OperationKind.Decrement);
    }

    private sealed class AnalyzerState
    {
        private readonly INamedTypeSymbol _parameterStateAttributeSymbol;

        public AnalyzerState(INamedTypeSymbol parameterStateAttributeSymbol)
        {
            _parameterStateAttributeSymbol = parameterStateAttributeSymbol;
        }

        /// <summary>
        /// Analyzes property reference operations for reads of ParameterState properties.
        /// </summary>
        public void AnalyzePropertyReference(OperationAnalysisContext context)
        {
            var propertyReference = (IPropertyReferenceOperation)context.Operation;
            var propertySymbol = propertyReference.Property;

            if (!TryGetParameterStateAttribute(propertySymbol, out var parameterUsage))
            {
                return;
            }

            // Check if this property reference is inside a nameof expression
            // nameof(Counter) should not trigger any diagnostic
            if (IsInsideNameofExpression(propertyReference))
            {
                return;
            }

            // Check if this property reference is inside a GetState method call
            // GetState(x => x.Counter) should not trigger any diagnostic
            if (IsInsideGetStateInvocation(propertyReference))
            {
                return;
            }

            // Check if this property reference is the target of an assignment
            // If so, we handle it in the assignment analyzers, not here
            if (IsAssignmentTarget(propertyReference))
            {
                return;
            }

            // Check for external access (different containing type)
            if (IsExternalAccess(propertyReference, context))
            {
                // Only report MUD0012 if read checking is enabled
                if (ShouldCheckRead(parameterUsage))
                {
                    ReportExternalAccessDiagnostic(context, propertyReference);
                }
                return;
            }

            // Only check reads if the Read flag is set
            if (!ShouldCheckRead(parameterUsage))
            {
                return;
            }

            // Check if read is allowed (only in constructor, even inside lambdas)
            if (IsInsideConstructor(context.ContainingSymbol))
            {
                return;
            }

            // It's a read from the same component - report MUD0010
            ReportReadDiagnostic(context, propertyReference);
        }

        /// <summary>
        /// Analyzes simple assignment operations for writes to ParameterState properties.
        /// </summary>
        public void AnalyzeSimpleAssignment(OperationAnalysisContext context)
        {
            var assignment = (ISimpleAssignmentOperation)context.Operation;

            if (assignment.Target is not IPropertyReferenceOperation propertyReference)
            {
                return;
            }

            var propertySymbol = propertyReference.Property;

            if (!TryGetParameterStateAttribute(propertySymbol, out var parameterUsage))
            {
                return;
            }

            // Only check writes if the Write flag is set
            if (!ShouldCheckWrite(parameterUsage))
            {
                return;
            }

            // Check for external access
            if (IsExternalAccess(propertyReference, context))
            {
                ReportExternalAccessDiagnostic(context, propertyReference);
                return;
            }

            // Check if write is allowed (constructor or SetParametersAsync)
            if (IsAllowedWriteContext(context))
            {
                return;
            }

            ReportWriteDiagnostic(context, propertyReference);
        }

        /// <summary>
        /// Analyzes compound assignment operations for writes to ParameterState properties.
        /// </summary>
        public void AnalyzeCompoundAssignment(OperationAnalysisContext context)
        {
            var assignment = (ICompoundAssignmentOperation)context.Operation;

            if (assignment.Target is not IPropertyReferenceOperation propertyReference)
            {
                return;
            }

            var propertySymbol = propertyReference.Property;

            if (!TryGetParameterStateAttribute(propertySymbol, out var parameterUsage))
            {
                return;
            }

            // Only check writes if the Write flag is set
            if (!ShouldCheckWrite(parameterUsage))
            {
                return;
            }

            // Check for external access
            if (IsExternalAccess(propertyReference, context))
            {
                ReportExternalAccessDiagnostic(context, propertyReference);
                return;
            }

            // Check if write is allowed (constructor or SetParametersAsync)
            if (IsAllowedWriteContext(context))
            {
                return;
            }

            ReportWriteDiagnostic(context, propertyReference);
        }

        /// <summary>
        /// Analyzes increment operations for writes to ParameterState properties.
        /// </summary>
        public void AnalyzeIncrement(OperationAnalysisContext context)
        {
            var increment = (IIncrementOrDecrementOperation)context.Operation;

            if (increment.Target is not IPropertyReferenceOperation propertyReference)
            {
                return;
            }

            var propertySymbol = propertyReference.Property;

            if (!TryGetParameterStateAttribute(propertySymbol, out var parameterUsage))
            {
                return;
            }

            // Only check writes if the Write flag is set
            if (!ShouldCheckWrite(parameterUsage))
            {
                return;
            }

            // Check for external access
            if (IsExternalAccess(propertyReference, context))
            {
                ReportExternalAccessDiagnostic(context, propertyReference);
                return;
            }

            // Check if write is allowed (constructor or SetParametersAsync)
            if (IsAllowedWriteContext(context))
            {
                return;
            }

            ReportWriteDiagnostic(context, propertyReference);
        }

        /// <summary>
        /// Analyzes decrement operations for writes to ParameterState properties.
        /// </summary>
        public void AnalyzeDecrement(OperationAnalysisContext context)
        {
            var decrement = (IIncrementOrDecrementOperation)context.Operation;

            if (decrement.Target is not IPropertyReferenceOperation propertyReference)
            {
                return;
            }

            var propertySymbol = propertyReference.Property;

            if (!TryGetParameterStateAttribute(propertySymbol, out var parameterUsage))
            {
                return;
            }

            // Only check writes if the Write flag is set
            if (!ShouldCheckWrite(parameterUsage))
            {
                return;
            }

            // Check for external access
            if (IsExternalAccess(propertyReference, context))
            {
                ReportExternalAccessDiagnostic(context, propertyReference);
                return;
            }

            // Check if write is allowed (constructor or SetParametersAsync)
            if (IsAllowedWriteContext(context))
            {
                return;
            }

            ReportWriteDiagnostic(context, propertyReference);
        }

        private bool TryGetParameterStateAttribute(IPropertySymbol propertySymbol, out ParameterUsageOptions parameterUsage)
        {
            // Default value matches ParameterStateAttribute.ParameterUsage default (ParameterUsageOptions.All)
            parameterUsage = ParameterUsageOptions.All;

            foreach (var attribute in propertySymbol.GetAttributes())
            {
                if (attribute.AttributeClass is null)
                    continue;

                // Use symbol equality comparison with the cached attribute symbol
                if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, _parameterStateAttributeSymbol))
                {
                    // Try to get the ParameterUsage property value
                    foreach (var namedArg in attribute.NamedArguments)
                    {
                        if (string.Equals(namedArg.Key, ParameterUsagePropertyName, StringComparison.Ordinal))
                        {
                            // Enum values in attributes can be boxed as int or as the underlying enum type
                            var value = namedArg.Value.Value;
                            if (value is int intValue)
                            {
                                parameterUsage = (ParameterUsageOptions)intValue;
                            }
                            else if (value is not null)
                            {
                                // Convert enum to underlying int value
                                parameterUsage = (ParameterUsageOptions)Convert.ToInt32(value);
                            }
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool ShouldCheckRead(ParameterUsageOptions parameterUsage)
        {
            return (parameterUsage & ParameterUsageOptions.Read) != 0;
        }

        private static bool ShouldCheckWrite(ParameterUsageOptions parameterUsage)
        {
            return (parameterUsage & ParameterUsageOptions.Write) != 0;
        }

        private static bool IsAssignmentTarget(IPropertyReferenceOperation propertyReference)
        {
            var parent = propertyReference.Parent;

            // Direct assignment target
            if (parent is ISimpleAssignmentOperation simpleAssignment && simpleAssignment.Target == propertyReference)
            {
                return true;
            }

            // Compound assignment target (+=, -=, etc.)
            if (parent is ICompoundAssignmentOperation compoundAssignment && compoundAssignment.Target == propertyReference)
            {
                return true;
            }

            // Increment/decrement target (++, --)
            if (parent is IIncrementOrDecrementOperation incrementDecrement && incrementDecrement.Target == propertyReference)
            {
                return true;
            }

            return false;
        }

        private static bool IsInsideNameofExpression(IPropertyReferenceOperation propertyReference)
        {
            // Walk up the operation tree to check if we're inside a nameof expression
            var current = propertyReference.Parent;
            while (current is not null)
            {
                if (current is INameOfOperation)
                {
                    return true;
                }
                current = current.Parent;
            }
            return false;
        }

        private static bool IsInsideGetStateInvocation(IPropertyReferenceOperation propertyReference)
        {
            // Walk up the operation tree to check if we're inside a GetState method invocation
            // GetState(x => x.Counter) should not trigger any diagnostic
            var current = propertyReference.Parent;
            while (current is not null)
            {
                if (current is IInvocationOperation invocation)
                {
                    var methodSymbol = invocation.TargetMethod;

                    // Check if the method is named "GetState" and is defined in ComponentBaseWithStateExtensions
                    if (string.Equals(methodSymbol.Name, GetStateMethodName, StringComparison.Ordinal) &&
                        IsComponentBaseWithStateExtensionsType(methodSymbol.ContainingType))
                    {
                        return true;
                    }
                }
                current = current.Parent;
            }
            return false;
        }

        private static bool IsComponentBaseWithStateExtensionsType(INamedTypeSymbol? typeSymbol)
        {
            if (typeSymbol is null)
            {
                return false;
            }

            // Check type name
            if (!string.Equals(typeSymbol.Name, "ComponentBaseWithStateExtensions", StringComparison.Ordinal))
            {
                return false;
            }

            // Check namespace
            var containingNamespace = typeSymbol.ContainingNamespace;
            if (containingNamespace is null || containingNamespace.IsGlobalNamespace)
            {
                return false;
            }

            // Build namespace string and compare
            // Expected: MudBlazor.Extensions
            return string.Equals(containingNamespace.Name, "Extensions", StringComparison.Ordinal) &&
                   containingNamespace.ContainingNamespace is not null &&
                   string.Equals(containingNamespace.ContainingNamespace.Name, "MudBlazor", StringComparison.Ordinal) &&
                   (containingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? true);
        }

        private static bool IsExternalAccess(IPropertyReferenceOperation propertyReference, OperationAnalysisContext context)
        {
            // Get the containing type where the property is defined
            var propertyContainingType = propertyReference.Property.ContainingType;

            // Get the containing type where the access is occurring
            var accessingContainingType = GetContainingType(context);

            if (accessingContainingType is null || propertyContainingType is null)
            {
                return false;
            }

            // Check if the property is being accessed through an instance
            if (propertyReference.Instance is null)
            {
                // Static access - not external
                return false;
            }

            // If the instance is 'this', it's not external access
            if (propertyReference.Instance is IInstanceReferenceOperation { ReferenceKind: InstanceReferenceKind.ContainingTypeInstance })
            {
                return false;
            }

            // Check if the access is from a different containing type
            return !SymbolEqualityComparer.Default.Equals(accessingContainingType, propertyContainingType);
        }

        private static INamedTypeSymbol? GetContainingType(OperationAnalysisContext context)
        {
            var containingSymbol = context.ContainingSymbol;
            while (containingSymbol is not null)
            {
                if (containingSymbol is INamedTypeSymbol namedType)
                {
                    return namedType;
                }
                containingSymbol = containingSymbol.ContainingSymbol;
            }
            return null;
        }

        private static bool IsAllowedWriteContext(OperationAnalysisContext context)
        {
            return IsInsideConstructorOrSetParametersAsync(context.ContainingSymbol);
        }

        private static bool IsInsideConstructorOrSetParametersAsync(ISymbol? containingSymbol)
        {
            // Walk up the containing symbol chain to check if any ancestor is a constructor or SetParametersAsync
            // This handles lambdas/anonymous methods defined inside constructors
            while (containingSymbol is not null)
            {
                if (containingSymbol is IMethodSymbol methodSymbol)
                {
                    // Check if it's a constructor
                    if (methodSymbol.MethodKind == MethodKind.Constructor)
                    {
                        return true;
                    }

                    // Check if it's SetParametersAsync method
                    if (string.Equals(methodSymbol.Name, SetParametersAsyncMethodName, StringComparison.Ordinal))
                    {
                        return true;
                    }

                    // For lambdas/anonymous methods, continue walking up to find the containing method
                    if (methodSymbol.MethodKind == MethodKind.AnonymousFunction ||
                        methodSymbol.MethodKind == MethodKind.LocalFunction)
                    {
                        containingSymbol = containingSymbol.ContainingSymbol;
                        continue;
                    }

                    // For regular methods, stop here - it's not an allowed context
                    return false;
                }

                containingSymbol = containingSymbol.ContainingSymbol;
            }

            return false;
        }

        private static bool IsInsideConstructor(ISymbol? containingSymbol)
        {
            // Walk up the containing symbol chain to check if any ancestor is a constructor
            // This handles lambdas/anonymous methods defined inside constructors
            while (containingSymbol is not null)
            {
                if (containingSymbol is IMethodSymbol methodSymbol)
                {
                    // Check if it's a constructor
                    if (methodSymbol.MethodKind == MethodKind.Constructor)
                    {
                        return true;
                    }

                    // For lambdas/anonymous methods, continue walking up to find the containing method
                    if (methodSymbol.MethodKind == MethodKind.AnonymousFunction ||
                        methodSymbol.MethodKind == MethodKind.LocalFunction)
                    {
                        containingSymbol = containingSymbol.ContainingSymbol;
                        continue;
                    }

                    // For regular methods, stop here - it's not a constructor
                    return false;
                }

                containingSymbol = containingSymbol.ContainingSymbol;
            }

            return false;
        }

        private static void ReportReadDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var propertySymbol = propertyReference.Property;
            var propertyName = propertySymbol.Name;
            var propertyTypeName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var diagnostic = Diagnostic.Create(
                ReadDescriptor,
                propertyReference.Syntax.GetLocation(),
                propertyName,
                propertyTypeName);

            context.ReportDiagnostic(diagnostic);
        }

        private static void ReportWriteDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var propertySymbol = propertyReference.Property;
            var propertyName = propertySymbol.Name;
            var propertyTypeName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var diagnostic = Diagnostic.Create(
                WriteDescriptor,
                propertyReference.Syntax.GetLocation(),
                propertyName,
                propertyTypeName);

            context.ReportDiagnostic(diagnostic);
        }

        private static void ReportExternalAccessDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var propertySymbol = propertyReference.Property;
            var propertyName = propertySymbol.Name;

            var diagnostic = Diagnostic.Create(
                ExternalAccessDescriptor,
                propertyReference.Syntax.GetLocation(),
                propertyName);

            context.ReportDiagnostic(diagnostic);
        }
    }
}

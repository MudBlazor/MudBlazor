// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers;

/// <summary>
/// Analyzer that enforces correct usage of the ParameterStateAttribute.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class ParameterStateAnalyzer : DiagnosticAnalyzer
{
    private const string ParameterStateAttributeFullName = "MudBlazor.State.ParameterStateAttribute";
    private const string SetParametersAsyncMethodName = "SetParametersAsync";

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

            if (!HasParameterStateAttribute(propertySymbol))
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
                ReportExternalAccessDiagnostic(context, propertyReference);
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

            if (!HasParameterStateAttribute(propertySymbol))
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

            if (!HasParameterStateAttribute(propertySymbol))
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

            if (!HasParameterStateAttribute(propertySymbol))
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

            if (!HasParameterStateAttribute(propertySymbol))
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

        private bool HasParameterStateAttribute(IPropertySymbol propertySymbol)
        {
            // Check for the ParameterStateAttribute by comparing the fully qualified name
            // This avoids issues with symbol identity across compilation contexts
            foreach (var attribute in propertySymbol.GetAttributes())
            {
                if (attribute.AttributeClass is null)
                    continue;

                var attributeFullName = attribute.AttributeClass.ToDisplayString();
                if (string.Equals(attributeFullName, ParameterStateAttributeFullName, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
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
            if (propertyReference.Instance is IInstanceReferenceOperation instanceRef &&
                instanceRef.ReferenceKind == InstanceReferenceKind.ContainingTypeInstance)
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
            var containingSymbol = context.ContainingSymbol;

            // Check if we're in a constructor
            if (containingSymbol is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind == MethodKind.Constructor)
                {
                    return true;
                }

                // Check if we're in SetParametersAsync method
                if (string.Equals(methodSymbol.Name, SetParametersAsyncMethodName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ReportReadDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var diagnostic = Diagnostic.Create(
                ReadDescriptor,
                propertyReference.Syntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }

        private static void ReportWriteDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var diagnostic = Diagnostic.Create(
                WriteDescriptor,
                propertyReference.Syntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }

        private static void ReportExternalAccessDiagnostic(OperationAnalysisContext context, IPropertyReferenceOperation propertyReference)
        {
            var diagnostic = Diagnostic.Create(
                ExternalAccessDescriptor,
                propertyReference.Syntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }
}

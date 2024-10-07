// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MudBlazor.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MudComponentUnknownParametersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId1 = "MUD0001";
        public const string DiagnosticId2 = "MUD0002";
        public const string ClassNamePropertyKey = "ClassName";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.MUD0001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _parameterMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0001MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _attributeMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0002MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.MUD0001Description), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableResourceString _url = new(nameof(Resources.MUD0001Url), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Attributes/Parameters";
        public const string DebugAnalyzerProperty = "build_property.MudDebugAnalyzer";
        public const string AllowedAttributePatternProperty = "build_property.mudallowedattributepattern";
        public const string IllegalParametersProperty = "build_property.mudillegalparameters";

        public static readonly DiagnosticDescriptor ParameterDescriptor = new(DiagnosticId1, _title, _parameterMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _description, helpLinkUri: _url.ToString());
        public static readonly DiagnosticDescriptor AttributeDescriptor = new(DiagnosticId2, _title, _attributeMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _description, helpLinkUri: _url.ToString());

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics = new[] { ParameterDescriptor, AttributeDescriptor }.ToImmutableArray();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(ctx =>
            {
                var global = ctx.Options.AnalyzerConfigOptionsProvider.GlobalOptions;


                if (global.TryGetValue(DebugAnalyzerProperty, out var debugValue) &&
                    bool.TryParse(debugValue, out var shouldDebug) && shouldDebug)
                {
                    Debugger.Launch();
                }

                if (!global.TryGetValue(AllowedAttributePatternProperty, out var allowPattern)
                   || !Enum.TryParse<AllowedAttributePattern>(allowPattern, out var allowedAttributePattern))
                {
                    allowedAttributePattern = AllowedAttributePattern.LowerCase;
                }

                if (!global.TryGetValue(IllegalParametersProperty, out var deny)
                    || !Enum.TryParse<IllegalParameters>(deny, out var illegalParameters))
                {
                    illegalParameters = IllegalParameters.V7IgnoreCase;
                }

                if (illegalParameters == IllegalParameters.Disabled && allowedAttributePattern == AllowedAttributePattern.Any)
                    return;

                var illegalParameterSet = new IllegalParameterSet(ctx.Compilation, illegalParameters);

                var analyzerContext = new AnalyzerContext(ctx.Compilation, illegalParameterSet, allowedAttributePattern);

                if (analyzerContext.IsValid)
                {
                    ctx.RegisterOperationAction(analyzerContext.AnalyzeBlockOptions, OperationKind.Block);
                }

            });
        }

        private sealed class AnalyzerContext
        {
            private readonly IEqualityComparer<ISymbol?> _symbolComparer = new MetadataSymbolComparer();
            private readonly ConcurrentDictionary<ITypeSymbol, ComponentDescriptor> _componentDescriptors = new(SymbolEqualityComparer.Default);
            private readonly IllegalParameterSet _illegalParameterSet;
            private readonly AllowedAttributePattern _allowedAttributePattern;
            private readonly INamedTypeSymbol? _componentBaseSymbol;
            private readonly INamedTypeSymbol? _parameterSymbol;
            private readonly INamedTypeSymbol? _renderTreeBuilderSymbol;
            private readonly INamedTypeSymbol? _mudComponentBaseType;

            public AnalyzerContext(Compilation compilation, IllegalParameterSet illegalParameterSet, AllowedAttributePattern allowedAttributePattern)
            {
                _illegalParameterSet = illegalParameterSet;
                _allowedAttributePattern = allowedAttributePattern;
                _componentBaseSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase");
                _parameterSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute");
                _renderTreeBuilderSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder");
                _mudComponentBaseType = compilation.GetBestTypeByMetadataName("MudBlazor.MudComponentBase");
            }

            public bool IsValid => _componentBaseSymbol is not null && _parameterSymbol is not null && _renderTreeBuilderSymbol is not null && _mudComponentBaseType is not null;

            public void AnalyzeBlockOptions(OperationAnalysisContext context)
            {
                try
                {
                    var classSymbol = context.Operation.GetClassSymbol(context);
                    if (classSymbol is not null && classSymbol.IsOrInheritFrom(_componentBaseSymbol, _symbolComparer))
                        TraverseTree(context, (IBlockOperation)context.Operation, classSymbol.ToDisplayString());
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            public void TraverseTree(OperationAnalysisContext context, IBlockOperation operations, string className)
            {
                ITypeSymbol? currentComponent = null;
                ComponentDescriptor? currentComponentDescriptor = null;

                foreach (var operation in operations.Operations)
                {
                    if (operation is IExpressionStatementOperation expressionStatement)
                    {
                        if (expressionStatement.Operation is IInvocationOperation invocation)
                        {
                            var targetMethod = invocation.TargetMethod;

                            if (targetMethod.ContainingType.IsEqualTo(_renderTreeBuilderSymbol))
                            {
                                if (string.Equals(targetMethod.Name, "OpenComponent", StringComparison.Ordinal) && targetMethod.TypeArguments.Length == 1)
                                {
                                    var componentType = targetMethod.TypeArguments[0];
                                    if (componentType.IsOrInheritFrom(_mudComponentBaseType))
                                    {
                                        currentComponent = componentType;
                                        currentComponentDescriptor = _componentDescriptors.GetOrAdd(currentComponent, ComponentDescriptor.GetComponentDescriptor(componentType, _parameterSymbol));
                                    }
                                }
                                else if (string.Equals(targetMethod.Name, "CloseComponent", StringComparison.Ordinal))
                                {
                                    currentComponent = null;
                                    currentComponentDescriptor = null;
                                }
                                else if (currentComponent is not null && targetMethod.Name is "AddAttribute" or "AddComponentParameter")
                                {
                                    if (targetMethod.Parameters.Length >= 2 && targetMethod.Parameters[1].Type.IsString())
                                    {
                                        var pName = invocation.Arguments[1].Value.ConstantValue;

                                        if (pName.HasValue && pName.Value is string parameterName)
                                            ValidateAttribute(context, invocation, currentComponentDescriptor, currentComponent, parameterName, className);
                                    }
                                }
                            }
                            else if (string.Equals(targetMethod.ContainingType.MetadataName, "TypeInference", StringComparison.Ordinal))
                            {
                                var methods = context.FilterTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
                                var method = methods?.Where(x => x.Identifier.ValueText == targetMethod.MetadataName).SingleOrDefault();

                                if (method is not null)
                                {
                                    var op = context.Compilation.GetSemanticModel(method.SyntaxTree).GetOperation(method);
                                    if (op is not null)
                                    {
                                        var blockOperation = op.ChildOperations.OfType<IBlockOperation>().Single();
                                        TraverseTree(context, blockOperation, className);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void ValidateAttribute(OperationAnalysisContext context, IInvocationOperation invocation,
                ComponentDescriptor? componentDescriptor, ITypeSymbol componentType, string parameterName, string className)
            {
                if (componentDescriptor is null || componentDescriptor.Parameters.Contains(parameterName))
                    return;
                else
                {
                    //check illegals first                    
                    foreach (var illegalParam in _illegalParameterSet.Parameters)
                    {
                        if (componentType.IsOrInheritFrom(illegalParam.Key, _symbolComparer) && illegalParam.Value.Contains(parameterName, _illegalParameterSet.Comparer))
                        {
                            Report(ParameterDescriptor, context, invocation, parameterName, componentDescriptor, className, _illegalParameterSet.IllegalParameters.ToString());
                            return;
                        }
                    }

                    switch (_allowedAttributePattern)
                    {
                        case AllowedAttributePattern.LowerCase when char.IsLower(parameterName, 0):
                            return;
                        case AllowedAttributePattern.DataAndAria when (parameterName.StartsWith("data-", StringComparison.Ordinal) || parameterName.StartsWith("aria-", StringComparison.Ordinal)):
                            return;
                        case AllowedAttributePattern.Any:
                            return;
                        default:
                            Report(AttributeDescriptor, context, invocation, parameterName, componentDescriptor, className, _allowedAttributePattern.ToString());
                            return;
                    }
                }
            }

            private void Report(DiagnosticDescriptor diagnosticDescriptor, OperationAnalysisContext context, IInvocationOperation invocation,
                string parameterName, ComponentDescriptor componentDescriptor, string className, string pattern)
            {
                var location = invocation.Syntax.GetLocation();
                var mappedLocation = location;

                var razorPath = invocation.GetRazorFilePath();
                if (razorPath is not null)
                {
                    var newLineSpan = new LinePositionSpan(new LinePosition(), new LinePosition());
                    mappedLocation = Location.Create(razorPath, new TextSpan(0, 0), newLineSpan);
                }

                context.ReportDiagnostic(
                 Diagnostic.Create(
                    descriptor: diagnosticDescriptor,
                    location: mappedLocation,
                    additionalLocations: [location],
                    properties: ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string?>(ClassNamePropertyKey, className) }),
                    messageArgs: [parameterName, componentDescriptor.TagName, pattern, location.GetLineSpan().Span]));
            }

        }
    }
}

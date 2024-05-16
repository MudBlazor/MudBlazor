// Copyright (c) Peter Thorpe 2024
// This file is licenced to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

#if ROSLYN_3_8
using System.Collections.Immutable;
#endif

namespace MudBlazor.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MudComponentUnknownParametersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId1 = "MUD0001";
        public const string DiagnosticId2 = "MUD0002";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString s_title = new LocalizableResourceString(nameof(Resources.MUD0001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_parameterMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0001MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_attributeMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0002MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_description = new LocalizableResourceString(nameof(Resources.MUD0001Description), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableResourceString s_url = new LocalizableResourceString(nameof(Resources.MUD0001Url), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Attributes/Parameters";

#pragma warning disable MA0011 // IFormatProvider is missing
        private static readonly DiagnosticDescriptor s_parameterRule = new(DiagnosticId1, s_title, s_parameterMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: s_description, helpLinkUri: s_url.ToString());
        private static readonly DiagnosticDescriptor s_attributeRule = new(DiagnosticId2, s_title, s_attributeMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: s_description, helpLinkUri: s_url.ToString());
#pragma warning restore MA0011 // IFormatProvider is missing

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get => [s_parameterRule, s_attributeRule]; }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(ctx =>
            {
                var global = ctx.Options.AnalyzerConfigOptionsProvider.GlobalOptions;


                if (global.TryGetValue("build_property.MudDebugAnalyzer", out var debugValue) &&
                    bool.TryParse(debugValue, out var shouldDebug) && shouldDebug)
                {
                    Debugger.Launch();
                }

                if (!global.TryGetValue("build_property.mudallowedattributepattern", out var allowPattern)
                   || !Enum.TryParse<AllowedAttributePattern>(allowPattern, out var allowedAttributePattern))
                {
                    allowedAttributePattern = AllowedAttributePattern.LowerCase;
                }

                if (!global.TryGetValue("build_property.mudillegalparameters", out var deny)
                    || !Enum.TryParse<IllegalParameters>(deny, out var illegalParameters))
                {
                    illegalParameters = IllegalParameters.V7IgnoreCase;
                }

                var illegalParameterSet = new IllegalParameterSet(ctx.Compilation, illegalParameters);

                var analyzerContext = new AnalyzerContext(ctx.Compilation, illegalParameterSet, allowedAttributePattern);

                if (analyzerContext.IsValid)
                {
                    ctx.RegisterOperationAction(analyzerContext.AnalyzeBlockOptions, OperationKind.Block);
                }

            });
        }


        private sealed class AnalyzerContext(Compilation compilation, IllegalParameterSet illegalParameterSet, AllowedAttributePattern allowedAttributePattern)
        {
            private IEqualityComparer<ISymbol?> _symbolComparer = new MetadataSymbolComparer();
            private readonly ConcurrentDictionary<ITypeSymbol, ComponentDescriptor> _componentDescriptors = new(SymbolEqualityComparer.Default);

            public bool IsValid => IComponentSymbol is not null && ComponentBaseSymbol is not null && ParameterSymbol is not null && MudComponentBaseType is not null;

            public INamedTypeSymbol? IComponentSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.IComponent");
            public INamedTypeSymbol? ComponentBaseSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase");
            public INamedTypeSymbol? ParameterSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute");
            public INamedTypeSymbol? RenderTreeBuilderSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder");
            public INamedTypeSymbol? MudComponentBaseType { get; } = compilation.GetBestTypeByMetadataName("MudBlazor.MudComponentBase");

            public void AnalyzeBlockOptions(OperationAnalysisContext context)
            {
                try
                {
                    var blockOperation = (IBlockOperation)context.Operation;
                    ITypeSymbol? currentComponent = null;
                    ComponentDescriptor? currentComponentDescriptor = null;
                    RazorHelper? currentRazorHelper = null;

                    foreach (var operation in blockOperation.Operations)
                    {
                        if (operation is IExpressionStatementOperation expressionStatement)
                        {
                            if (expressionStatement.Operation is IInvocationOperation invocation)
                            {
                                var targetMethod = invocation.TargetMethod;
                                if (targetMethod.ContainingType.IsEqualTo(RenderTreeBuilderSymbol))
                                {
                                    if (string.Equals(targetMethod.Name, "OpenComponent", StringComparison.Ordinal) && targetMethod.TypeArguments.Length == 1)
                                    {
                                        if (targetMethod.TypeArguments.Length == 1)
                                        {
                                            var componentType = targetMethod.TypeArguments[0];
                                            if (componentType.IsOrInheritFrom(MudComponentBaseType) /* componentType.IsOrImplements(IComponentSymbol)*/)
                                            {
                                                currentComponent = componentType;
                                                currentComponentDescriptor = _componentDescriptors.GetOrAdd(currentComponent, ComponentDescriptor.GetComponentDescriptor(componentType, ParameterSymbol));
                                            }
                                        }
                                    }
                                    else if (string.Equals(targetMethod.Name, "CloseComponent", StringComparison.Ordinal))
                                    {
                                        currentComponent = null;
                                        currentComponentDescriptor = null;
                                        currentRazorHelper = null;
                                    }
                                    else if (currentComponent is not null && targetMethod.Name is "AddAttribute" or "AddComponentParameter")
                                    {
                                        if (targetMethod.Parameters.Length >= 2 && targetMethod.Parameters[1].Type.IsString())
                                        {
                                            var value = invocation.Arguments[1].Value.ConstantValue;
                                            if (value.HasValue && value.Value is string parameterName)
                                            {
                                                ValidateAttribute(currentRazorHelper, context, invocation, currentComponentDescriptor, currentComponent, parameterName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            private void ValidateAttribute(RazorHelper? razorHelper, OperationAnalysisContext context, IInvocationOperation invocation,
                ComponentDescriptor? componentDescriptor, ITypeSymbol componentType, string parameterName)
            {
                if (componentDescriptor is null || componentDescriptor.Parameters.Contains(parameterName))
                    return;
                else
                {
                    //check illegals first
                    if (illegalParameterSet is not null)
                    {
                        foreach (var illegalParam in illegalParameterSet.Parameters)
                        {
                            if (componentType.IsOrInheritFrom(illegalParam.Key, _symbolComparer) && illegalParam.Value.Contains(parameterName, illegalParameterSet.Comparer))
                            {
                                ReportDiagnosticRazorMapped(razorHelper, s_parameterRule, context, invocation, parameterName, componentDescriptor, illegalParameterSet.IllegalParameters.ToString());
                                return;
                            }
                        }
                    }

                    switch (allowedAttributePattern)
                    {
                        case AllowedAttributePattern.LowerCase when char.IsLower(parameterName, 0):
                            return;
                        case AllowedAttributePattern.DataAndAria when (parameterName.StartsWith("data-", StringComparison.Ordinal) || parameterName.StartsWith("aria-", StringComparison.Ordinal)):
                            return;
                        case AllowedAttributePattern.Any:
                            return;
                        default:
                            ReportDiagnosticRazorMapped(razorHelper, s_attributeRule, context, invocation, parameterName, componentDescriptor, allowedAttributePattern.ToString());
                            return;
                    }

                }

            }

            private void ReportDiagnosticRazorMapped(RazorHelper? razorHelper, DiagnosticDescriptor diagnosticDescriptor, OperationAnalysisContext context, IInvocationOperation invocation,
                string parameterName, ComponentDescriptor componentDescriptor, string pattern)
            {
                razorHelper ??= new RazorHelper(context, invocation, componentDescriptor);
                razorHelper.TryGetRazorLocation(parameterName, out var newLocation);

                context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, newLocation,
                    [newLocation],
                    parameterName, componentDescriptor.TagName, pattern));
            }






        }
    }
}

// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

#if ROSLYN_3_8
using System.Collections.Immutable;
#endif

namespace MudBlazor.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DoNotUseUnknownParameterForRazorComponentAnalyzer : DiagnosticAnalyzer
{

    private static readonly DiagnosticDescriptor s_rule = new(
        "MUD00001",
        title: "Unknown component parameter",
        messageFormat: "The parameter '{0}' does not exist on component '{1}'",
        "Parameters",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "",
        helpLinkUri: "www.mudblazor.com/issue/MUD00001");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(ctx =>
        {
            var analyzerContext = new AnalyzerContext(ctx.Compilation);
            if (analyzerContext.IsValid)
            {
                ctx.RegisterOperationAction(analyzerContext.AnalyzeBlockOptions, OperationKind.Block);
            }
        });
    }

    private sealed class AnalyzerContext(Compilation compilation)
    {
        private bool _ignoreCaptureUnmatchedValues = true;

        private Dictionary<string, object?> IgnoreAttributes { get; set; } = new Dictionary<string, object?>()
        {
            { "aria-label", null },
            { "aria-current", null },
            { "aria-pressed", null },
            { "aria-hidden", null },
            { "href", null },
            { "disabled", null },
            { "tabindex", null },
            { "id", null },
            { "type", null },
            { "target", null },
            { "rel", null },
            { "title", null },
            { "min", null },
            { "max", null },
            { "step", null },
            { "class", null },
            { "dir", null },
            { "autocomplete", null },
            { "colspan", null },
            { "for", null },
            { "src", null },
            { "async", null },
            { "data-consent-category", null },
            { "onclick", null },
            { "onfocus", null },
            { "ondrop", null },
            { "ontouchstart", null },
            { "ontouchmove", null },
            { "ontouchend", null },
            { "oncontextmenu", null },
            { "onmousedown", null },
            { "onmouseenter", null },
            { "onmouseleave", null },
            { "ondragleave", null },
            { "ondragenter", null },
            { "ondragend", null },
            { "Multiple", null }
        };


        private readonly ConcurrentDictionary<ITypeSymbol, ComponentDescriptor> _componentDescriptors = new(SymbolEqualityComparer.Default);

        public bool IsValid => IComponentSymbol is not null && ComponentBaseSymbol is not null && ParameterSymbol is not null;

        public INamedTypeSymbol? IComponentSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.IComponent");
        public INamedTypeSymbol? ComponentBaseSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase");
        public INamedTypeSymbol? ParameterSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute");
        public INamedTypeSymbol? RenderTreeBuilderSymbol { get; } = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder");

        public void AnalyzeBlockOptions(OperationAnalysisContext context)
        {
            var blockOperation = (IBlockOperation)context.Operation;
            //filter to MudComponentBase
            var mudComponentBaseType = context.Compilation.GetTypeByMetadataName("MudBlazor.MudComponentBase");
            ITypeSymbol? currentComponent = null;
            foreach (var operation in blockOperation.Operations)
            {
                if (operation is IExpressionStatementOperation expressionStatement)
                {
                    if (expressionStatement.Operation is IInvocationOperation invocation)
                    {
                        var targetMethod = invocation.TargetMethod;
                        if (targetMethod.ContainingType.IsEqualTo(RenderTreeBuilderSymbol))
                        {
                            if (targetMethod.Name == "OpenComponent" && targetMethod.TypeArguments.Length == 1)
                            {
                                var componentType = targetMethod.TypeArguments[0];
                                if (componentType.IsOrImplements(IComponentSymbol))
                                {
                                    currentComponent = targetMethod.TypeArguments[0];
                                }
                            }
                            else if (targetMethod.Name == "CloseComponent")
                            {
                                currentComponent = null;
                            }
                            else if (currentComponent is not null && currentComponent.InheritsFrom(mudComponentBaseType) && targetMethod.Name is "AddAttribute" or "AddComponentParameter")
                            {
                                if (targetMethod.Parameters.Length >= 2 && targetMethod.Parameters[1].Type.IsString())
                                {
                                    var value = invocation.Arguments[1].Value.ConstantValue;
                                    if (value.HasValue && value.Value is string parameterName)
                                    {
                                        if (!IsValidAttribute(currentComponent, parameterName))
                                        {
                                            //var descriptor = GetComponentDescriptor(currentComponent);
                                            //var allowedParams = string.Join(",", descriptor.Parameters);
                                            context.ReportDiagnostic(s_rule, invocation.Syntax, parameterName, currentComponent.ToDisplayString(NullableFlowState.None)  /*+ $" userattdef: {descriptor.UserAttributesDefinition} allowed: {allowedParams}"*/);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsValidAttribute(ITypeSymbol componentType, string parameterName)
        {
            var descriptor = GetComponentDescriptor(componentType);
            if (descriptor.HasMatchUnmatchedParameters)
                return true;

            if (descriptor.Parameters.Contains(parameterName))
                return true;

            return false;
        }

        //Regex _parameterDictionaryRegex = new(@"""{\s""(?'p'.*?)""""", RegexOptions.Multiline);
        private ComponentDescriptor GetComponentDescriptor(ITypeSymbol typeSymbol)
        {
            return _componentDescriptors.GetOrAdd(typeSymbol, symbol =>
            {
                var descriptor = new ComponentDescriptor();
                var currentSymbol = symbol as INamedTypeSymbol;
                while (currentSymbol is not null)
                {

                    descriptor.Parameters.Add(currentSymbol.Name);
                    foreach (var member in currentSymbol.GetMembers())
                    {
                        if (member is IPropertySymbol property)
                        {
                            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.parameterattribute?view=aspnetcore-6.0&WT.mc_id=DT-MVP-5003978
                            var parameterAttribute = property.GetAttribute(ParameterSymbol, inherits: false); // the attribute is sealed
                            if (parameterAttribute is null)
                                continue;

                            foreach (var attribute in IgnoreAttributes)
                                descriptor.Parameters.Add(attribute.Key);

                            //HACKY READ OF ADDITIONAL UserAttributes
                            /*if (member.Name.Equals("UserAttributes"))
                            {
                                descriptor.Parameters.Add("UserAttributes");

                                if (member.DeclaringSyntaxReferences.Any())
                                {
                                    var propdec = (PropertyDeclarationSyntax)member.DeclaringSyntaxReferences[0].GetSyntax();
                                    var syntax = propdec.Initializer;
                                    if (syntax is null)
                                        continue;

                                    var extraparams = syntax.ToFullString();
                                    descriptor.UserAttributesDefinition = extraparams;
                                    foreach (Match extraParam in _parameterDictionaryRegex.Matches(extraparams))
                                        descriptor.Parameters.Add(extraParam.Value);
                                }

                                foreach (var attribute in IgnoreAttributes)
                                    descriptor.Parameters.Add(attribute.Key);
                            }
                            else
                            {*/
                            if (descriptor.Parameters.Add(member.Name))
                            {
                                if (parameterAttribute.NamedArguments.Any(arg => !_ignoreCaptureUnmatchedValues && arg.Key == "CaptureUnmatchedValues" && arg.Value.Value is true))
                                {
                                    descriptor.HasMatchUnmatchedParameters = true;
                                }
                            }
                            //}
                        }
                    }

                    currentSymbol = currentSymbol.BaseType;
                }

                return descriptor;
            });
        }



        private sealed class ComponentDescriptor
        {
            public HashSet<string> Parameters { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public bool HasMatchUnmatchedParameters { get; set; }
            public string? UserAttributesDefinition { get; set; }
        }
    }
}

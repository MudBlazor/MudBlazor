// Copyright (c) Peter Thorpe 2024
// This file is licenced to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace MudBlazor.Analyzers.Internal
{
    /// <summary>
    /// Attempts to map source generated razor back to the razor file so code issues can point at it. 
    /// There is probbaly a better way but the line directives don't seem to work.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="invocation"></param>
    /// <param name="componentDescriptor"></param>
    internal class RazorHelper(OperationAnalysisContext context, IInvocationOperation invocation, ComponentDescriptor componentDescriptor)
    {
        IEnumerable<LineMapping> _allLineMappings = invocation.Syntax.SyntaxTree.GetLineMappings(context.CancellationToken);


        /// <summary>
        /// Tries to map source generated parameter to .razor location
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="newLocation"></param>
        /// <returns></returns>
        internal bool TryGetRazorLocation(string parameterName, out Location newLocation)
        {
            var originalLocation = invocation.Syntax.GetLocation();

            MapFromTypeInference(originalLocation, out originalLocation);

            var originalLineSpan = originalLocation.GetLineSpan();
            var previousSpan = _allLineMappings.Where(x => x.Span.Start <= originalLineSpan.Span.Start
                                 && x.MappedSpan.HasMappedPath).OrderByDescending(y => y.Span.Start).FirstOrDefault();

            if (previousSpan.MappedSpan.HasMappedPath && previousSpan.MappedSpan.IsValid)
            {
                TryFindTag(previousSpan.MappedSpan, out var newSpan);
                TryFindParameter(parameterName, previousSpan.MappedSpan, out newSpan);
                newLocation = Location.Create(originalLineSpan.Path, originalLocation.SourceSpan,
                    originalLineSpan.Span, newSpan.Path, newSpan.Span);
                return true;
            }
            else
            {
                newLocation = originalLocation;
                return false;
            }
        }

        /// <summary>
        /// Finds the component tag in a .razor file
        /// </summary>
        /// <param name="span">Starting position span to work from</param>
        /// <param name="newSpan"></param>
        /// <returns></returns>
        internal bool TryFindTag(FileLinePositionSpan span, out FileLinePositionSpan newSpan)
        {
            return TryFind(span, out newSpan, [($"<{componentDescriptor.TagName}", 1, componentDescriptor.TagName.Length)]);
        }

        /// <summary>
        /// Finds the parameter/attribute in a razor file
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="span">Starting position span to work from</param>
        /// <param name="newSpan"></param>
        /// <returns></returns>
        internal bool TryFindParameter(string parameterName, FileLinePositionSpan span, out FileLinePositionSpan newSpan)
        {
            return TryFind(span, out newSpan,
                [
                ($" {parameterName}=", 1, parameterName.Length),
                ($" {parameterName} ", 1, parameterName.Length),
                ($" {parameterName}>", 1, parameterName.Length)
                ]);
        }

        /// <summary>
        /// Finds a text pattern in a .razor file
        /// </summary>
        /// <param name="span">Starting position span to work from</param>
        /// <param name="newSpan"></param>
        /// <param name="patterns">The pattern mathcign to try</param>
        /// <returns></returns>
        internal bool TryFind(FileLinePositionSpan span, out FileLinePositionSpan newSpan, params (string Pattern, int StartOffset, int Length)[] patterns)
        {
            var fileContent = context.Options.AdditionalFiles.FirstOrDefault(x => string.Equals(x.Path, span.Path, StringComparison.Ordinal))?.GetText(context.CancellationToken);
            if (fileContent is null)
            {
                newSpan = span;
                return false;
            }

            var lineNo = span.StartLinePosition.Line;

            while (lineNo < fileContent.Lines.Count)
            {
                var line = fileContent.Lines[lineNo].ToString();

                var position = -1;
                foreach (var (pattern, startOffset, length) in patterns)
                {
                    position = line.IndexOf(pattern, StringComparison.Ordinal);
                    if (position > -1)
                    {
                        position += startOffset;

                        var newLineSpan = new LinePositionSpan(new LinePosition(lineNo, position), new LinePosition(lineNo, position + length));

                        newSpan = new FileLinePositionSpan(span.Path, newLineSpan);
                        return true;
                    }
                }

                lineNo++;
            }
            newSpan = span;
            return false;
        }


        /// <summary>
        /// If the component has a generic type the parameters will be create din a TypeInference class so we need to find the method call
        /// </summary>
        /// <param name="originalLocation"></param>
        /// <param name="newLocation"></param>
        /// <returns></returns>
        private bool MapFromTypeInference(Location originalLocation, out Location newLocation)
        {
            var methodSymbol = context.ContainingSymbol;
            var classSymbol = methodSymbol.ContainingSymbol;

            if (methodSymbol is null || classSymbol is null)
            {
                newLocation = originalLocation;
                return false;
            }

            if (string.Equals(classSymbol.Name, "TypeInference", StringComparison.Ordinal))
            {
                var root = originalLocation.SourceTree?.GetCompilationUnitRoot(context.CancellationToken);

                if (root is not null)
                {
                    foreach (var s in root.DescendantNodes().OfType<IdentifierNameSyntax>())
                    {
                        if (string.Equals(s.ToString(), methodSymbol.Name, StringComparison.Ordinal))
                        {
                            newLocation = s.GetLocation();
                            return true;
                        }
                    }
                }
            }

            newLocation = originalLocation;
            return false;
        }
    }
}

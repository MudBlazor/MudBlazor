// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.Analyzers;

namespace MudBlazor.UnitTests.Analyzers.Helpers
{
#nullable enable
    internal static class DiagnosticHelper
    {
        internal static List<Diagnostic> FilterToClass(this IEnumerable<Diagnostic> diagnostics, string? className)
        {
            var results = new List<Diagnostic>();
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Properties.TryGetValue(MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey, out var cn)
                    && string.Equals(cn, className))
                    results.Add(diagnostic);
            }

            return results;
        }

        private static IOrderedEnumerable<Diagnostic> SortToFileOrder(this IEnumerable<Diagnostic> fileLinePositions)
        {
            return fileLinePositions
                .OrderBy(x => x.AdditionalLocations.First().GetLineSpan().StartLinePosition.Line)
                .ThenBy(x => x.AdditionalLocations.First().GetLineSpan().StartLinePosition.Character);
        }

        private static IOrderedEnumerable<FileLinePositionSpan> SortToFileOrder(this IEnumerable<FileLinePositionSpan> fileLinePositions)
        {
            return fileLinePositions
                .OrderBy(x => x.StartLinePosition.Line)
                .ThenBy(x => x.StartLinePosition.Character);
        }

        internal static void CompareLocations(this IEnumerable<Diagnostic> diagnostics, IEnumerable<FileLinePositionSpan> expectedPositions)
        {
            var oderedDiagnostics = diagnostics.SortToFileOrder();
            var orderedExpectedPositions = expectedPositions.SortToFileOrder();

            for (var i = 0; i < oderedDiagnostics.Count(); i++)
                TestLineAndFilePosition(oderedDiagnostics.ElementAt(i), orderedExpectedPositions.ElementAt(i));

        }

        private static void TestLineAndFilePosition(Diagnostic diagnostic, FileLinePositionSpan postion)
        {
            var lineSpan = diagnostic.AdditionalLocations.First().GetLineSpan();
            lineSpan.StartLinePosition.Should().Be(postion.StartLinePosition);
            lineSpan.EndLinePosition.Should().Be(postion.EndLinePosition);
            lineSpan.Path.Should().EndWith(postion.Path);
        }

    }
#nullable restore
}

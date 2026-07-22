// Copyright (c) MudBlazor 2021
// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.Analyzers;
using static MudBlazor.UnitTests.Components.ParametersTests;

namespace MudBlazor.UnitTests.Analyzers.Internal
{
#nullable enable
    internal class ExpectedDiagnostic
    {
        internal ExpectedDiagnostic(DiagnosticDescriptor descriptor, FileLinePositionSpan position, string message)
        {
            Descriptor = descriptor;
            Position = position;
            Message = message;
        }

        internal DiagnosticDescriptor Descriptor { get; private set; }
        internal FileLinePositionSpan Position { get; private set; }
        internal string Message { get; private set; }

        internal static List<Diagnostic> FilterToClass(IEnumerable<Diagnostic> diagnostics, string? className)
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

        private static IOrderedEnumerable<Diagnostic> SortToFileOrder(IEnumerable<Diagnostic> fileLinePositions)
        {
            return fileLinePositions
                .OrderBy(x => x.AdditionalLocations.First().GetLineSpan().StartLinePosition.Line)
                .ThenBy(x => x.AdditionalLocations.First().GetLineSpan().StartLinePosition.Character);
        }

        private static IOrderedEnumerable<ExpectedDiagnostic> SortToFileOrder(IEnumerable<ExpectedDiagnostic> expectedDiagnostics)
        {
            return expectedDiagnostics
                .OrderBy(x => x.Position.StartLinePosition.Line)
                .ThenBy(x => x.Position.StartLinePosition.Character);
        }

        internal static void Compare(IEnumerable<Diagnostic> diagnostics, IEnumerable<ExpectedDiagnostic> expectedDiagnostics)
        {
            var oderedDiagnostics = SortToFileOrder(diagnostics);
            var orderedExpectedDiagnostics = SortToFileOrder(expectedDiagnostics);

            for (var i = 0; i < oderedDiagnostics.Count(); i++)
                TestMessage(oderedDiagnostics.ElementAt(i), orderedExpectedDiagnostics.ElementAt(i));
        }

        private static void TestMessage(Diagnostic diagnostic, ExpectedDiagnostic expectedDiagnostics)
        {
            diagnostic.GetMessage().Should().StartWith(expectedDiagnostics.Message);
        }
    }
#nullable restore
}

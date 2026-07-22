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

namespace MudBlazor.UnitTests.Analyzers.Internal
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
    }
#nullable restore
}

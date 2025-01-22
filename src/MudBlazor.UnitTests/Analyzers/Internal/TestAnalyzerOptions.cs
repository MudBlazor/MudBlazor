using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MudBlazor.Analyzers;

namespace MudBlazor.UnitTests.Analyzers.Internal
{
#nullable enable
    internal sealed class TestAnalyzerOptions : AnalyzerConfigOptionsProvider
    {
        private TestAnalyzerOptions(Dictionary<string, string> values) => _values = values ?? [];

        internal static AnalyzerOptions Create(
            AllowedAttributePattern attributeProviderAttribute,
             ImmutableArray<AdditionalText> additionalText, string? attributeList = null)
        {
            return new AnalyzerOptions(additionalText, new TestAnalyzerOptions(new Dictionary<string, string>()
            {
                [MudComponentUnknownParametersAnalyzer.AllowedAttributePatternProperty] = attributeProviderAttribute.ToString()!,
                [MudComponentUnknownParametersAnalyzer.AllowedAttributeListProperty] = attributeList ?? string.Empty
            }));
        }

        private readonly Dictionary<string, string> _values;

        public override AnalyzerConfigOptions GlobalOptions => new TestAnalyzerConfigOptions(_values);
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new TestAnalyzerConfigOptions(_values);
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new TestAnalyzerConfigOptions(_values);

        private sealed class TestAnalyzerConfigOptions(Dictionary<string, string> values) : AnalyzerConfigOptions
        {
            private readonly Dictionary<string, string> _values = values;

            public override bool TryGetValue(string key, [NotNullWhen(returnValue: true)] out string? value)
            {
                return _values.TryGetValue(key, out value);
            }
        }
    }

#nullable restore
}

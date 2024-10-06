// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MudBlazor.UnitTests.Analyzers.Internal
{
#nullable enable
    public sealed class TestAdditionalText(string path, SourceText text) : AdditionalText
    {
        public TestAdditionalText(string text = "", Encoding? encoding = null, string path = "dummy")
            : this(path, SourceText.From(text, encoding))
        {
        }

        public override string Path { get; } = path;

        public override SourceText GetText(CancellationToken cancellationToken = default) => text;
    }
#nullable restore
}

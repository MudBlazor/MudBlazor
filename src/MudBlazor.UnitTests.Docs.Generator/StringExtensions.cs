using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MudBlazor.UnitTests.Docs.Generator;

#nullable enable
public static partial class StringExtensions
{
    [return: NotNullIfNotNull(nameof(self))]
    public static string? ToLfLineEndings(this string? self)
    {
        return self is null
            ? null
            : NewLineRegularExpression().Replace(self, "\n");
    }

    [GeneratedRegex(@"\r?\n")]
    private static partial Regex NewLineRegularExpression();
}

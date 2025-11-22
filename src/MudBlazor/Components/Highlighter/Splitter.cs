using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable
namespace MudBlazor.Components.Highlighter;

public enum FragmentType { Text, HighlightedText, Markup }
public record FragmentInfo(string Content, FragmentType Type);

public static partial class Splitter
{
    private static readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(5);

    private static readonly Regex _htmlTagRegex = HtmlTagMatcher();

    private static readonly Regex _tagParser = HtmlTagParser();

    private static readonly HashSet<string> _voidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img",
        "input", "link", "meta", "source", "track", "wbr"
    };

    private const string NextBoundary = ".*?\\b";
    private static StringBuilder? _stringBuilderCached;

    public static Memory<string> GetFragments(string? text, string? highlightedText,
                                              IEnumerable<string>? highlightedTexts, out string regex,
                                              bool caseSensitive = false, bool untilNextBoundary = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            regex = string.Empty;
            return Memory<string>.Empty;
        }

        var highlightTerms = BuildHighlightTermsList(highlightedText, highlightedTexts);
        if (highlightTerms.Count == 0)
        {
            regex = string.Empty;
            return new[] { text };
        }

        regex = BuildRegexPattern(highlightTerms, untilNextBoundary);
        var splits = Regex.Split(text, regex, GetRegexOptions(caseSensitive) | RegexOptions.NonBacktracking, _regexTimeout);
        var nonEmpty = splits.Where(s => !string.IsNullOrEmpty(s)).ToArray();

        return new Memory<string>(nonEmpty);
    }

    public static List<FragmentInfo> GetHtmlAwareFragments(string? text, string? highlightedText,
                                                           IEnumerable<string>? highlightedTexts, out string regex,
                                                           bool caseSensitive, bool untilNextBoundary)
    {
        regex = string.Empty;

        if (string.IsNullOrEmpty(text))
            return [];

        var highlightTerms = BuildHighlightTermsList(highlightedText, highlightedTexts);
        var highlightRegex = BuildHighlightRegex(highlightTerms, caseSensitive, untilNextBoundary, out regex);
        var stringComparison = GetStringComparison(caseSensitive);

        var rawFragments = _htmlTagRegex.Split(text);
        var tempFragments = ProcessRawFragments(rawFragments, highlightTerms, highlightRegex, stringComparison);

        return SanitizeFragments(tempFragments, highlightTerms, highlightRegex);
    }

    private static List<string> BuildHighlightTermsList(string? single, IEnumerable<string>? multiple)
    {
        var terms = new List<string>();

        AddTermIfNotEmpty(terms, single);

        if (multiple != null)
        {
            foreach (var term in multiple.Where(s => !string.IsNullOrEmpty(s)))
            {
                AddTermIfNotEmpty(terms, term);
            }
        }

        return terms;

        static void AddTermIfNotEmpty(List<string> terms, string? term)
        {
            if (string.IsNullOrEmpty(term)) return;

            terms.Add(term);

            var encoded = WebUtility.HtmlEncode(term);
            if (encoded != term)
            {
                terms.Add(encoded);
            }
        }
    }

    private static string BuildRegexPattern(List<string> terms, bool untilNextBoundary)
    {
        var builder = GetStringBuilder();

        try
        {
            builder.Append("((?:");

            for (var i = 0; i < terms.Count; i++)
            {
                if (i > 0) builder.Append(")|(?:");
                builder.Append(Regex.Escape(terms[i]));
                if (untilNextBoundary) builder.Append(NextBoundary);
            }

            builder.Append("))");
            return builder.ToString();
        }
        finally
        {
            ReturnStringBuilder(builder);
        }
    }

    private static Regex BuildHighlightRegex(List<string> terms, bool caseSensitive, bool untilNextBoundary, out string regex)
    {
        regex = string.Empty;

        if (terms.Count == 0) return new Regex("^$", RegexOptions.NonBacktracking, _regexTimeout);

        regex = BuildRegexPattern(terms, untilNextBoundary);

        return new Regex(regex, GetRegexOptions(caseSensitive) | RegexOptions.Singleline | RegexOptions.NonBacktracking, _regexTimeout);
    }

    private static List<FragmentInfo> ProcessRawFragments(
        string[] rawFragments,
        List<string> highlightTerms,
        Regex highlightRegex,
        StringComparison stringComparison)
    {
        var tempFragments = new List<FragmentInfo>();

        foreach (var fragment in rawFragments.Where(s => !string.IsNullOrEmpty(s)))
        {
            if (_htmlTagRegex.IsMatch(fragment))
            {
                tempFragments.Add(new FragmentInfo(fragment, FragmentType.Markup));
            }
            else if (IsDirectMatch(fragment, highlightTerms, stringComparison))
            {
                tempFragments.Add(new FragmentInfo(fragment, FragmentType.HighlightedText));
            }
            else
            {
                tempFragments.AddRange(ProcessTextSegment(fragment, highlightRegex));
            }
        }

        return tempFragments;
    }

    private static bool IsDirectMatch(string segment, List<string> highlightTerms, StringComparison comparison)
    {
        return highlightTerms.Any(term => string.Equals(segment, term, comparison));
    }

    private static IEnumerable<FragmentInfo> ProcessTextSegment(string segment, Regex highlightRegex)
    {
        var fragments = new List<FragmentInfo>();
        var lastIndex = 0;

        foreach (Match match in highlightRegex.Matches(segment))
        {
            if (match.Index > lastIndex)
            {
                var textPart = segment.Substring(lastIndex, match.Index - lastIndex);
                fragments.Add(new FragmentInfo(textPart, FragmentType.Text));
            }

            fragments.Add(new FragmentInfo(match.Value, FragmentType.HighlightedText));
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < segment.Length)
        {
            fragments.Add(new FragmentInfo(segment.Substring(lastIndex), FragmentType.Text));
        }

        return fragments;
    }

    private static List<FragmentInfo> SanitizeFragments(
        List<FragmentInfo> fragments,
        List<string> highlightTerms,
        Regex highlightRegex)
    {
        var result = new List<FragmentInfo>();
        var tagStack = new Stack<string>();

        foreach (var fragment in fragments)
        {
            if (fragment.Type != FragmentType.Markup)
            {
                result.Add(fragment);
                continue;
            }

            ProcessMarkupFragment(fragment, result, tagStack);
        }

        ProcessUnmatchedTags(result, tagStack, highlightTerms, highlightRegex);
        return result;
    }

    private static void ProcessMarkupFragment(FragmentInfo fragment, List<FragmentInfo> result, Stack<string> tagStack)
    {
        if (!TryParseTag(fragment.Content, out var tagInfo))
        {
            result.Add(HtmlEncodeFragment(fragment));
            return;
        }

        if (tagInfo.IsSelfClosing)
        {
            result.Add(fragment);
        }
        else if (tagInfo.IsClosing)
        {
            if (tagStack.Count > 0 && string.Equals(tagStack.Peek(), tagInfo.Name, StringComparison.OrdinalIgnoreCase))
            {
                tagStack.Pop();
                result.Add(fragment);
            }
            else
            {
                result.Add(HtmlEncodeFragment(fragment));
            }
        }
        else // opening tag
        {
            tagStack.Push(tagInfo.Name);
            result.Add(fragment);
        }
    }

    private static void ProcessUnmatchedTags(List<FragmentInfo> result, Stack<string> tagStack, List<string> highlightTerms, Regex highlightRegex)
    {
        while (tagStack.Count > 0)
        {
            var unmatchedTag = tagStack.Pop();
            ProcessUnmatchedTag(result, unmatchedTag, highlightTerms, highlightRegex);
        }
    }

    private static void ProcessUnmatchedTag(List<FragmentInfo> result, string unmatchedTag, List<string> highlightTerms, Regex highlightRegex)
    {
        for (var i = result.Count - 1; i >= 0; i--)
        {
            var fragment = result[i];

            if (fragment.Type != FragmentType.Markup) continue;
            if (!TryParseTag(fragment.Content, out var tagInfo)) continue;
            if (tagInfo.IsClosing || !string.Equals(tagInfo.Name, unmatchedTag, StringComparison.OrdinalIgnoreCase)) continue;

            var processedFragments = ProcessTextSegmentForUnmatched(fragment.Content, highlightRegex);

            if (processedFragments.Count > 0)
            {
                result.RemoveAt(i);
                result.InsertRange(i, processedFragments);
            }
            else
            {
                var isHighlight = highlightTerms.Contains(fragment.Content);
                result[i] = new FragmentInfo(fragment.Content, isHighlight ? FragmentType.HighlightedText : FragmentType.Text);
            }
            break;
        }
    }

    private static List<FragmentInfo> ProcessTextSegmentForUnmatched(string segment, Regex highlightRegex)
    {
        var tempFragments = new List<FragmentInfo>();
        var lastIndex = 0;

        foreach (Match match in highlightRegex.Matches(segment))
        {
            if (match.Index > lastIndex)
            {
                var unmatchedSegment = segment.Substring(lastIndex, match.Index - lastIndex);
                tempFragments.Add(new FragmentInfo(unmatchedSegment, FragmentType.Text));
            }

            tempFragments.Add(new FragmentInfo(match.Value, FragmentType.HighlightedText));
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex > 0 && lastIndex < segment.Length)
        {
            tempFragments.Add(new FragmentInfo(segment.Substring(lastIndex), FragmentType.Text));
        }

        return tempFragments;
    }

    private static FragmentInfo HtmlEncodeFragment(FragmentInfo original)
    {
        var encoded = WebUtility.HtmlEncode(original.Content);
        var type = original.Type == FragmentType.HighlightedText ? FragmentType.HighlightedText : FragmentType.Text;
        return new FragmentInfo(encoded, type);
    }

    private static bool TryParseTag(string markup, out TagInfo tagInfo)
    {
        var match = _tagParser.Match(markup);

        if (match.Success)
        {
            var isClosing = match.Groups[1].Success;
            var tagName = match.Groups[2].Value;
            var isSelfClosing = match.Groups[3].Success || _voidElements.Contains(tagName);

            tagInfo = new TagInfo(tagName, isClosing, isSelfClosing);
            return true;
        }

        tagInfo = default;
        return false;
    }

    private static StringBuilder GetStringBuilder()
    {
        return Interlocked.Exchange(ref _stringBuilderCached, null) ?? new StringBuilder();
    }

    private static void ReturnStringBuilder(StringBuilder builder)
    {
        builder.Clear();
        _stringBuilderCached = builder;
    }

    private static RegexOptions GetRegexOptions(bool caseSensitive)
    {
        return caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
    }

    private static StringComparison GetStringComparison(bool caseSensitive)
    {
        return caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
    }

    private readonly record struct TagInfo(string Name, bool IsClosing, bool IsSelfClosing);

    [GeneratedRegex(@"(<\s*/?\s*\w+[^>]*?/?>)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline, "en-US")]
    private static partial Regex HtmlTagMatcher();

    [GeneratedRegex(@"^<\s*(/)?\s*(\w+)[^>]*?(\/)?\s*>$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex HtmlTagParser();
}

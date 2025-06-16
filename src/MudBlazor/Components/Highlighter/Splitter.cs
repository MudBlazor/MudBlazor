using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable enable
namespace MudBlazor.Components.Highlighter;

public enum FragmentType { Text, HighlightedText, Markup }
public record FragmentInfo(string Content, FragmentType Type);

public static class Splitter
{
    private static readonly Regex HtmlTagRegex = new Regex(@"(<\s*/?\s*\w+[^>]*?/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private static readonly Regex TagParser = new Regex(@"^<\s*(/)?\s*(\w+)[^>]*?(\/)?\s*>$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly HashSet<string> VoidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img",
        "input", "link", "meta", "source", "track", "wbr"
    };

    private const string NextBoundary = ".*?\\b";
    private static StringBuilder? s_stringBuilderCached;
    private static List<string> _highlightTerms = [];
    private static Regex _highlightRegex = new("^$");

    public static Memory<string> GetFragments(
    string? text,
    string? highlightedText,
    IEnumerable<string>? highlightedTexts,
    out string regex,
    bool caseSensitive = false,
    bool untilNextBoundary = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            regex = string.Empty;
            return Memory<string>.Empty;
        }

        var builder = Interlocked.Exchange(ref s_stringBuilderCached, null) ?? new StringBuilder();
        builder.Append("((?:");

        bool hasPattern = false;
        AppendIfNotEmpty(highlightedText);

        if (highlightedTexts != null)
        {
            foreach (var ht in highlightedTexts.Where(s => !string.IsNullOrEmpty(s)))
            {
                if (hasPattern) builder.Append(")|(?:");
                AppendPattern(ht);
            }
        }

        if (hasPattern)
        {
            builder.Append("))");
        }
        else
        {
            regex = string.Empty;
            builder.Clear();
            s_stringBuilderCached = builder;
            return new[] { text };
        }

        regex = builder.ToString();
        builder.Clear();
        s_stringBuilderCached = builder;

        var splits = Regex.Split(text, regex, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
        var nonEmpty = splits.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        return new Memory<string>(nonEmpty);

        void AppendIfNotEmpty(string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                AppendPattern(s);
            }
        }

        void AppendPattern(string s)
        {
            hasPattern = true;
            builder.Append(Regex.Escape(s));
            if (untilNextBoundary) builder.Append(NextBoundary);
        }
    }

    public static List<FragmentInfo> GetHtmlAwareFragments(
        string? text,
        string? highlightedText,
        IEnumerable<string>? highlightedTexts,
        bool caseSensitive,
        bool untilNextBoundary)
    {
        var results = new List<FragmentInfo>();
        if (string.IsNullOrEmpty(text)) return results;

        _highlightTerms = BuildHighlightTerms(highlightedText, highlightedTexts);
        _highlightRegex = BuildHighlightRegex(_highlightTerms, caseSensitive, untilNextBoundary);

        var stringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        var rawFragments = HtmlTagRegex.Split(text);
        var tempFragments = new List<FragmentInfo>();

        foreach (var segment in rawFragments)
        {
            if (string.IsNullOrEmpty(segment)) continue;

            if (_highlightTerms.Any(term => string.Equals(segment, term, stringComparison)))
            {
                tempFragments.Add(new FragmentInfo(segment, FragmentType.HighlightedText));
            }
            else if (HtmlTagRegex.IsMatch(segment))
            {
                tempFragments.Add(new FragmentInfo(segment, FragmentType.Markup));
            }
            else
            {
                int last = 0;
                foreach (Match match in _highlightRegex.Matches(segment))
                {
                    if (match.Index > last)
                    {
                        tempFragments.Add(new FragmentInfo(segment.Substring(last, match.Index - last), FragmentType.Text));
                    }
                    tempFragments.Add(new FragmentInfo(match.Value, FragmentType.HighlightedText));
                    last = match.Index + match.Length;
                }
                if (last < segment.Length)
                {
                    tempFragments.Add(new FragmentInfo(segment.Substring(last), FragmentType.Text));
                }
            }
        }

        return SanitizeFragments(tempFragments);
    }

    private static List<string> BuildHighlightTerms(string? single, IEnumerable<string>? multiple)
    {
        var list = new List<string>();
        if (!string.IsNullOrEmpty(single))
        {
            list.Add(single);
            list.Add(WebUtility.HtmlEncode(single));
        }

        if (multiple != null)
        {
            list.AddRange(multiple.Where(str => !string.IsNullOrEmpty(str)));
            list.AddRange(multiple.Where(str => !string.IsNullOrEmpty(WebUtility.UrlEncode(str))));
        }

        return list;
    }

    private static Regex BuildHighlightRegex(List<string> terms, bool caseSensitive, bool untilNextBoundary)
    {
        if (!terms.Any()) return new Regex("^$");

        var builder = new StringBuilder();
        for (int i = 0; i < terms.Count; i++)
        {
            builder.Append("(").Append(Regex.Escape(terms[i]));
            if (untilNextBoundary) builder.Append(NextBoundary);
            builder.Append(")");
            if (i < terms.Count - 1) builder.Append("|");
        }

        return new Regex(builder.ToString(),
            (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase) | RegexOptions.Singleline);
    }

    private static List<FragmentInfo> SanitizeFragments(List<FragmentInfo> fragments)
    {
        var result = new List<FragmentInfo>();
        var tagStack = new Stack<string>();

        foreach (var frag in fragments)
        {
            if (frag.Type != FragmentType.Markup)
            {
                result.Add(frag);
                continue;
            }

            var tag = frag.Content;
            var match = TagParser.Match(tag);

            if (!match.Success)
            {
                result.Add(HtmlEncodeFragment(frag));
                continue;
            }

            bool isClosing = match.Groups[1].Success;
            var tagName = match.Groups[2].Value;
            bool isSelfClosing = match.Groups[3].Success || VoidElements.Contains(tagName);

            if (isSelfClosing)
            {
                result.Add(frag);
            }
            else if (isClosing)
            {
                if (tagStack.Count > 0 && string.Equals(tagStack.Peek(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    tagStack.Pop();
                    result.Add(frag);
                }
                else
                {
                    result.Add(HtmlEncodeFragment(frag));
                }
            }
            else // opening
            {
                tagStack.Push(tagName);
                result.Add(frag);
            }
        }

        // Reclassify any unmatched opening tags left on the stack
        while (tagStack.Count > 0)
        {
            var unmatchedTag = tagStack.Pop(); // e.g., "b", "i"

            // Look backwards for the last matching opening tag fragment
            for (int i = result.Count - 1; i >= 0; i--)
            {
                var frag = result[i];
                if (frag.Type == FragmentType.Markup && TryGetTagName(frag.Content, out var tagName, out bool isClosing, out bool isSelfClosing))
                {
                    if (!isClosing && string.Equals(tagName, unmatchedTag, StringComparison.OrdinalIgnoreCase))
                    {
                        int last = 0;
                        var segment = frag.Content;
                        var tempFragments = new List<FragmentInfo>();

                        foreach (Match match in _highlightRegex.Matches(segment))
                        {
                            if (match.Index > last)
                            {
                                var unmatchedSegment = segment.Substring(last, match.Index - last);
                                tempFragments.Add(new FragmentInfo(WebUtility.HtmlEncode(unmatchedSegment), FragmentType.Text));
                            }
                            tempFragments.Add(new FragmentInfo(WebUtility.HtmlEncode(match.Value), FragmentType.HighlightedText));
                            last = match.Index + match.Length;
                        }

                        if (last == 0 && segment.Length > 0)
                        {
                            // This tag was unmatched — reclassify it
                            var encoded = WebUtility.HtmlEncode(frag.Content);
                            var isHighlight = _highlightTerms.Contains(frag.Content);

                            result[i] = new FragmentInfo(encoded, isHighlight ? FragmentType.HighlightedText : FragmentType.Text);
                            break;
                        }
                        else if (last < segment.Length)
                        {
                            tempFragments.Add(new FragmentInfo(segment.Substring(last), FragmentType.Text));
                        }

                        result.RemoveAt(i);
                        result.InsertRange(i, tempFragments);
                        break;
                    }
                }
            }
        }

        return result;
    }

    private static FragmentInfo HtmlEncodeFragment(FragmentInfo original)
    {
        var encoded = System.Net.WebUtility.HtmlEncode(original.Content);
        return new FragmentInfo(encoded, original.Type == FragmentType.HighlightedText ? FragmentType.HighlightedText : FragmentType.Text);
    }

    private static bool TryGetTagName(string markup, out string tagName, out bool isClosing, out bool isSelfClosing)
    {
        var match = TagParser.Match(markup);

        if (match.Success)
        {
            isClosing = match.Groups[1].Success;
            tagName = match.Groups[2].Value;
            isSelfClosing = match.Groups[3].Success || VoidElements.Contains(tagName);

            return true;
        }

        tagName = string.Empty;
        isClosing = false;
        isSelfClosing = false;

        return false;
    }
}

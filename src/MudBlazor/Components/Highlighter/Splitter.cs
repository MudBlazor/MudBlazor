using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MudBlazor.Components.Highlighter
{
#nullable enable

    /// <summary>
    /// Splits text into fragments based on text to be highlighted.
    /// </summary>
    public static class Splitter
    {
        private const string NextBoundary = ".*?\\b";

        private static StringBuilder? s_stringBuilderCached;

        /// <summary>
        /// Splits text into fragments based on text to be highlighted.
        /// </summary>
        /// <param name="text">The text to examine.</param>
        /// <param name="highlightedText">The text to be highlighted.</param>
        /// <param name="highlightedTexts">The multiple texts to be highlighted.</param>
        /// <param name="regex">The regular expression used to split text into fragments.</param>
        /// <param name="caseSensitive">Uses a case-sensitive check for highlighted text.</param>
        /// <param name="untilNextBoundary">Highlights text until the next regular expression boundary.</param>
        /// <returns>A block of memory with the matched text to highlight.</returns>
        public static Memory<string> GetFragments(string? text,
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

            var builder = Interlocked.Exchange(ref s_stringBuilderCached, null) ?? new();
            //the first brace in the pattern is to keep the patten when splitting,
            //the `(?:` in the pattern is to accept multiple highlightedTexts but not capture them.
            builder.Append("((?:");

            //this becomes true if `AppendPattern` was called at least once.
            var hasAtLeastOnePattern = false;
            if (!string.IsNullOrEmpty(highlightedText))
            {
                AppendPattern(highlightedText);
            }

            if (highlightedTexts is not null)
            {
                foreach (var substring in highlightedTexts)
                {
                    if (string.IsNullOrEmpty(substring))
                        continue;

                    //split pattern if we already added an string to search.
                    if (hasAtLeastOnePattern)
                    {
                        builder.Append(")|(?:");
                    }

                    AppendPattern(substring);
                }
            }

            if (hasAtLeastOnePattern)
            {
                //close the last pattern group and the capture group.
                builder.Append("))");
            }
            else
            {
                builder.Clear();
                s_stringBuilderCached = builder;

                //all patterns were empty or null.
                regex = string.Empty;
                return new[] { text };
            }

            regex = builder.ToString();
            builder.Clear();
            s_stringBuilderCached = builder;

            var splits = Regex
                    .Split(text,
                           regex,
                           caseSensitive
                             ? RegexOptions.None
                             : RegexOptions.IgnoreCase);
            var length = 0;
            for (var i = 0; i < splits.Length; i++)
            {
                var s = splits[i];
                if (!string.IsNullOrEmpty(s))
                {
                    splits[length++] = s;
                }
            }
            Array.Clear(splits, length, splits.Length - length);
            return splits.AsMemory(0, length);

            void AppendPattern(string value)
            {
                hasAtLeastOnePattern = true;
                //escapes the text for regex
                value = Regex.Escape(value);
                builder.Append(value);
                if (untilNextBoundary)
                {
                    builder.Append(NextBoundary);
                }
            }
        }
    }
}

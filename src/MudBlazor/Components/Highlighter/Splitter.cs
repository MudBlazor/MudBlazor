using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor.Components.Highlighter
{
    public static class Splitter
    {
        private const string NextBoundary = ".*?\\b";

        /// <summary>
        /// Splits the text into fragments, according to the
        /// text to be highlighted
        /// </summary>
        /// <param name="text">The whole text</param>
        /// <param name="highlightedText">The text to be highlighted</param>
        /// <param name="highlightedTexts">The texts to be highlighted</param>
        /// <param name="regex">Regex expression that was used to split fragments.</param>
        /// <param name="caseSensitive">Whether it's case sensitive or not</param>
        /// <param name="untilNextBoundary">If true, splits until the next regex boundary</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFragments(string text,
                                                       string highlightedText,
                                                       IEnumerable<string> highlightedTexts,
                                                       out string regex,
                                                       bool caseSensitive = false,
                                                       bool untilNextBoundary = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                regex = "";
                return new List<string>();
            }

            StringBuilder builder = new();
            //the first brace in the pattern is to keep the patten when splitting,
            //the `(?:` in the pattern is to accept multiple highlightedTexts but not capture them.
            builder.Append("((?:");

            //this becomes true if `AppendPattern` was called at least once.
            bool hasAtLeastOnePattern = false;
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
                //all patterns were empty or null.
                regex = "";
                return new List<string>() { text };
            }

            regex = builder.ToString();
            return Regex
                    .Split(text,
                           regex,
                           caseSensitive
                             ? RegexOptions.None
                             : RegexOptions.IgnoreCase)
                    .Where(s => !string.IsNullOrEmpty(s));

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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudBlazor.Components.Highlighter
{
    public static class Splitter
    {
        private const string nextBoundary = ".*?\\b";

        /// <summary>
        /// Splits the text into fragments, according to the
        /// text to be highlighted
        /// </summary>
        /// <param name="text">The whole text</param>
        /// <param name="highlightedText">The text to be highlighted</param>
        /// <param name="caseSensitive">Whether it's case sensitive or not</param>
        /// <param name="untilNextBoundary">If true, splits until the next regex boundary</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFragments(string text,
                                                       string highlightedText,
                                                       bool caseSensitive = false,
                                                       bool untilNextBoundary = false)
        {
            if (string.IsNullOrWhiteSpace(highlightedText))
            {
                return new List<string> { text };
            }
            else
            {
                //escapes the text for regex             
                highlightedText = Regex.Escape(highlightedText);
                if (untilNextBoundary)
                {
                    highlightedText += nextBoundary;
                }

                //using braces in the pattern keeps the pattern when splitting
                return Regex
                     .Split(text,
                            $"({highlightedText})",
                            caseSensitive
                              ? RegexOptions.None
                              : RegexOptions.IgnoreCase)
                     .Where(s => !string.IsNullOrEmpty(s));
            }
        }
    }
}

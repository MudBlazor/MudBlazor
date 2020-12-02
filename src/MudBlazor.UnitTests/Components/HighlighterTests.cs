using FluentAssertions;

using NUnit.Framework;

using System.Collections.Generic;

using static MudBlazor.Components.Highlighter.Splitter;

namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class HighlighterTests
    {
        private const string TEXT = "This is the first item";

        [Test]
        public void ShouldSplitTest()
        {
            var highlightedText = "item";
            var result = GetFragments(TEXT, highlightedText);
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
        }

        [Test]
        public void ShouldUseUntilNextBoundaryTest()
        {
            var highlightedText = "it";
            var result = GetFragments(TEXT, highlightedText, false, true);
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
        }

        [Test]
        public void ShouldBeCaseSensitiveTest()
        {
            var highlightedText = "It";
            var result = GetFragments(TEXT, highlightedText, true, false);
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new List<string> { TEXT });
        }

        [Test]
        public void DontMessWithRegexSpecialCharacters()
        {
            //regex characters are properly escaped in GetFragments
            var highlightedText = ".";
            var result = GetFragments(TEXT, highlightedText, true, false);
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new List<string> { TEXT });
        }
    }
}

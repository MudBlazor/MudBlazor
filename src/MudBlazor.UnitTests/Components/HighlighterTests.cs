using Bunit;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

using static Bunit.ComponentParameterFactory;
using static MudBlazor.Components.Highlighter.Splitter;

namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class HighlighterTests
    {
        private const string TEXT = "This is the first item";

        #region Splitter
        
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
        
        #endregion


        #region Markup

        /// <summary>
        /// Check markup whith regular text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupTest()
        {
            using var ctx = new Bunit.TestContext();
            var text = Parameter(nameof(MudHighlighter.Text), TEXT);
            var highlightedText = Parameter(nameof(MudHighlighter.HighlightedText), "item");
            
           var comp = ctx.RenderComponent<MudHighlighter>(text, highlightedText);
            comp.MarkupMatches("This is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup whith regex text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupWithRegexTextTest()
        {
            using var ctx = new Bunit.TestContext();
            var text = Parameter(nameof(MudHighlighter.Text), TEXT);
            var highlightedText = Parameter(nameof(MudHighlighter.HighlightedText), "[");

            var comp = ctx.RenderComponent<MudHighlighter>(text, highlightedText);
            comp.MarkupMatches("This is the first item");
        }


        /// <summary>
        /// Check markup whith property 
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUntilNextBoundaryTest()
        {
            using var ctx = new Bunit.TestContext();
            var text = Parameter(nameof(MudHighlighter.Text), TEXT);
            var highlightedText = Parameter(nameof(MudHighlighter.HighlightedText), "it");
            var untilNextBoundary= Parameter(nameof(MudHighlighter.UntilNextBoundary), true);

            var comp = ctx
                .RenderComponent<MudHighlighter>(text, highlightedText, untilNextBoundary);
            
            comp.MarkupMatches("This is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup whith property 
        /// </summary>
        [Test]
        public void MudHighlighterMarkupCaseSensitiveTest()
        {
            using var ctx = new Bunit.TestContext();
            var text = Parameter(nameof(MudHighlighter.Text), "This is this");
            var highlightedText = Parameter(nameof(MudHighlighter.HighlightedText), "this");
            var caseSensitive = Parameter(nameof(MudHighlighter.CaseSensitive), true);
            var caseInSensitive = Parameter(nameof(MudHighlighter.CaseSensitive), false);

            var comp = ctx
                .RenderComponent<MudHighlighter>(text, highlightedText, caseSensitive);

            //Case sensitive
            comp.MarkupMatches("This is <mark>this</mark>");

            //Case insensitive
            comp.SetParametersAndRender(text, highlightedText, caseInSensitive);
            comp.MarkupMatches("<mark>This</mark> is <mark>this</mark>");

        }
        #endregion
    }
}

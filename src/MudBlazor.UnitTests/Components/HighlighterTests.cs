using Bunit;
using FluentAssertions;
using MudBlazor.Components.Highlighter;
using MudBlazor.UnitTests.TestComponents.Highlighter;
using NUnit.Framework;
using static MudBlazor.Components.Highlighter.Splitter;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class HighlighterSplitterTests
    {
        private const string TEXT = "This is the first item";

        [Test]
        public void ShouldSplitUsingHighlightedTextParameterTest()
        {
            var highlightedText = "item";
            var result = GetFragments(TEXT, highlightedText, null, out var regex).ToArray();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
            regex.Should().Be("((?:item))");
        }

        [Test]
        public void ShouldSplitUsingHighlightedTextsParameterWithOneElementTest()
        {
            var highlightedTexts = new string[] { "item" };
            var result = GetFragments(TEXT, null, highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
            regex.Should().Be("((?:item))");
        }

        [Test]
        public void ShouldSplitUsingHighlightedTextsParameterWithMultipleElementsTest()
        {
            var highlightedTexts = new string[] { "item", "the" };
            var result = GetFragments(TEXT, null, highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(4);
            result.Should().BeEquivalentTo(new List<string> { "This is ", "the", " first ", "item" });
            regex.Should().Be("((?:item)|(?:the))");
        }

        [Test]
        public void ShouldSplitUsingHighlightedTextParameterAndHighlightedTextsParameterWithOneElementTest()
        {
            var highlightedTexts = new string[] { "the" };
            var result = GetFragments(TEXT, "item", highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(4);
            result.Should().BeEquivalentTo(new List<string> { "This is ", "the", " first ", "item" });
            regex.Should().Be("((?:item)|(?:the))");
        }

        [Test]
        public void ShouldSplitUsingHighlightedTextParameterAndHighlightedTextsParameterWithMultipleElementsTest()
        {
            var highlightedTexts = new string[] { "first", "the" };
            var result = GetFragments(TEXT, "item", highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(6);
            result.Should().BeEquivalentTo(new List<string> { "This is ", "the", " ", "first", " ", "item" });
            regex.Should().Be("((?:item)|(?:first)|(?:the))");
        }

        [Test]
        public void ShouldUseUntilNextBoundaryTest()
        {
            var highlightedText = "it";
            var result = GetFragments(TEXT, highlightedText, null, out var regex, false, true).ToArray();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
            regex.Should().Be("((?:it.*?\\b))");
        }

        [Test]
        public void ShouldBeCaseSensitiveTest()
        {
            var highlightedText = "It";
            var result = GetFragments(TEXT, highlightedText, null, out var regex, true, false).ToArray();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new List<string> { TEXT });
            regex.Should().Be("((?:It))");
        }

        [Test]
        public void DontMessWithRegexSpecialCharacters()
        {
            //regex characters are properly escaped in GetFragments
            var highlightedText = ".";
            var result = GetFragments(TEXT, highlightedText, null, out var regex, true, false).ToArray();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new List<string> { TEXT });
            regex.Should().Be("((?:\\.))");
        }

        [Test]
        public void DontMessWithDuplicatedHighlightPatternsInHighlightedTextsParameterTest()
        {
            var highlightedTexts = new string[] { "item", "item" };
            var result = GetFragments(TEXT, null, highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
            regex.Should().Be("((?:item)|(?:item))");
        }

        [Test]
        public void DontMessWithDuplicatedHighlightPatternsInHighlightedTextParameterAndHighlightedTextsParameterTest()
        {
            var highlightedTexts = new string[] { "item" };
            var result = GetFragments(TEXT, "item", highlightedTexts, out var regex).ToArray();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<string> { "This is the first ", "item" });
            regex.Should().Be("((?:item)|(?:item))");
        }

        [Test]
        public void GetFragments_NoHighlightTerms_ReturnsOriginalTextAsSingleFragment()
        {
            var inputText = "This is some sample text.";
            string highlightedText = null;
            string[] highlightedTexts = null;

            var result = GetFragments(inputText, highlightedText, highlightedTexts, out var outRegex).ToArray();

            result.Should().HaveCount(1);
            result[0].Should().Be(inputText);
            outRegex.Should().Be(string.Empty);
        }

        [Test]
        public void GetHtmlAwareFragments_NullOrEmptyText_ReturnsEmptyList()
        {
            var resultNull = GetHtmlAwareFragments(null, "any", null, out var outRegex, false, false);
            resultNull.Should().BeEmpty();
            outRegex.Should().Be(string.Empty);


            // Test with empty text
            var resultEmpty = GetHtmlAwareFragments(string.Empty, "any", null, out outRegex, false, false);
            resultEmpty.Should().BeEmpty();

            outRegex.Should().Be(string.Empty);
        }

        [Test]
        public void GetHtmlAwareFragments_InvalidTag_IsEncodedAsText()
        {
            var text = "This is <notatag an attribute> text.";
            var highlightedText = "text";

            var fragments = GetHtmlAwareFragments(text, highlightedText, null, out var outRegex, false, false);

            fragments.Should().Contain(new FragmentInfo("<notatag an attribute>", FragmentType.Text));

            // Verify surrounding text and highlight
            var expectedFullSequence = new[] {
                new FragmentInfo("This is ", FragmentType.Text),
                new FragmentInfo("<notatag an attribute>", FragmentType.Text),
                new FragmentInfo(" ", FragmentType.Text),
                new FragmentInfo("text", FragmentType.HighlightedText),
                new FragmentInfo(".", FragmentType.Text)
            };
            fragments.Should().BeEquivalentTo(expectedFullSequence, options => options.WithStrictOrdering());
        }

        [Test]
        public void GetHtmlAwareFragments_SelfClosingTag_IsPreservedAsMarkup()
        {
            var text = "Text with <br /> a line break.";
            var highlightedText = "Text";

            var fragments = GetHtmlAwareFragments(text, highlightedText, null, out var outRegex, false, false);

            var expectedFullSequence = new[] {
                new FragmentInfo("Text", FragmentType.HighlightedText),
                new FragmentInfo(" with ", FragmentType.Text),
                new FragmentInfo("<br />", FragmentType.Markup),
                new FragmentInfo(" a line break.", FragmentType.Text)
            };
            fragments.Should().BeEquivalentTo(expectedFullSequence, options => options.WithStrictOrdering());
        }

        [Test]
        public void GetHtmlAwareFragments_MismatchedClosingTag_IsEncodedAsText()
        {
            var text = "<div><span>Content</div></span>";
            var highlightedText = "Content";
            var fragments = GetHtmlAwareFragments(text, highlightedText, null, out var outRegex, false, false);

            var expectedFullSequence = new[] {
                new FragmentInfo("<div>", FragmentType.Text),
                new FragmentInfo("<span>", FragmentType.Markup),
                new FragmentInfo("Content", FragmentType.HighlightedText),
                new FragmentInfo(System.Net.WebUtility.HtmlEncode("</div>"), FragmentType.Text),
                new FragmentInfo("</span>", FragmentType.Markup)
            };
            fragments.Should().BeEquivalentTo(expectedFullSequence, options => options.WithStrictOrdering());
        }

        [Test]
        public void GetHtmlAwareFragments_UnmatchedTagWithHighlightAndTrailingText_CorrectlyFragments()
        {
            var text = "<b>unclosed highlight then_text";
            var highlightedText = "highlight";

            var fragments = GetHtmlAwareFragments(text, highlightedText, null, out var outRegex, caseSensitive: false, untilNextBoundary: false);

            var expectedSequence = new[] {
                new FragmentInfo("<b>", FragmentType.Text), // The unmatched opening tag becomes text
                new FragmentInfo("unclosed ", FragmentType.Text),
                new FragmentInfo("highlight", FragmentType.HighlightedText),
                new FragmentInfo(" then_text", FragmentType.Text)
            };

            fragments.Should().BeEquivalentTo(expectedSequence, options => options.WithStrictOrdering());
        }

        [Test]
        public void GetFragments_WithManyTerms_ExercisesStringBuilderCaching()
        {
            var text = "The quick brown fox jumps over the lazy dog and repeats.";
            var highlightedTexts1 = new[] { "quick", "brown", "fox", "jumps", "lazy", "dog", "repeats" };

            var fragments1 = GetFragments(text, null, highlightedTexts1, out var outRegex1, caseSensitive: false, untilNextBoundary: false).ToArray();

            fragments1.Should().NotBeEmpty();
            outRegex1.Should().NotBeEmpty();
            outRegex1.Should().ContainAll(highlightedTexts1);
            fragments1.Length.Should().Be(highlightedTexts1.Length * 2 - 1 + 2);

            var text2 = "Another test sentence with new words like example and cache.";
            var highlightedTexts2 = new[] { "sentence", "words", "example", "cache" };
            var fragments2 = GetFragments(text2, null, highlightedTexts2, out var outRegex2, caseSensitive: false, untilNextBoundary: false).ToArray();

            fragments2.Should().NotBeEmpty();
            outRegex2.Should().NotBeEmpty();
            outRegex2.Should().ContainAll(highlightedTexts2);
            fragments2.Length.Should().Be(highlightedTexts2.Length * 2 - 1 + 2);

            var text3 = "No highlights here.";
            var fragments3 = GetFragments(text3, null, [], out var outRegex3, caseSensitive: false, untilNextBoundary: false).ToArray();
            fragments3.Should().HaveCount(1);
            fragments3[0].Should().Be(text3);
            outRegex3.Should().BeEmpty();
        }

        [Test]
        public void GetHtmlAwareFragments_CaseSensitivity_ProducesCorrectFragments()
        {
            var text = "Highlight highlight";
            var highlightTerm = "Highlight";

            // Case Sensitive
            var fragmentsSensitive = GetHtmlAwareFragments(text, highlightTerm, null, out var outRegex, caseSensitive: true, untilNextBoundary: false);
            var expectedSensitive = new[] {
                new FragmentInfo("Highlight", FragmentType.HighlightedText),
                new FragmentInfo(" highlight", FragmentType.Text)
            };
            fragmentsSensitive.Should().BeEquivalentTo(expectedSensitive, options => options.WithStrictOrdering());

            // Case Insensitive
            var fragmentsInsensitive = GetHtmlAwareFragments(text, highlightTerm, null, out outRegex, caseSensitive: false, untilNextBoundary: false);
            var expectedInsensitive = new[] {
                new FragmentInfo("Highlight", FragmentType.HighlightedText),
                new FragmentInfo(" ", FragmentType.Text),
                new FragmentInfo("highlight", FragmentType.HighlightedText)
            };
            fragmentsInsensitive.Should().BeEquivalentTo(expectedInsensitive, options => options.WithStrictOrdering());
        }
    }

    [TestFixture]
    public class HighlighterTests : BunitTest
    {
        private const string TEXT = "This is the first item";

        /// <summary>
        /// Check markup with regular text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUsingHighlightedTextParameterTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedText, "item"));
            comp.MarkupMatches("This is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup with multiple regular texts, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUsingHighlightedTextsParameterWithOneElementTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedTexts, new string[] { "item" }));
            comp.MarkupMatches("This is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup with multiple regular texts, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUsingHighlightedTextsParameterWithMultipleElementsTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedTexts, new string[] { "item", "This" }));
            comp.MarkupMatches("<mark>This</mark> is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup with multiple regular text and a single regular text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUsingHighlightedTextParameterAndHighlightedTextsParameterWithOneElementTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedText, "This")
                .Add(x => x.HighlightedTexts, new string[] { "item" }));
            comp.MarkupMatches("<mark>This</mark> is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup with multiple regular text and a single regular text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUsingHighlightedTextParameterAndHighlightedTextsParameterWithMultipleElementsTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedText, "This")
                .Add(x => x.HighlightedTexts, new string[] { "item", "first" }));
            comp.MarkupMatches("<mark>This</mark> is the <mark>first</mark> <mark>item</mark>");
        }

        /// <summary>
        /// Check nulls
        /// </summary>
        [Test]
        public void MudHighlighter_Nulls_Test()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, null)
                .Add(x => x.HighlightedText, null));
            comp.MarkupMatches(string.Empty);
        }

        /// <summary>
        /// Check markup with regex text, no regex
        /// </summary>
        [Test]
        public void MudHighlighterMarkupWithRegexTextTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedText, "["));
            comp.MarkupMatches("This is the first item");
        }

        /// <summary>
        /// Check markup with property 
        /// </summary>
        [Test]
        public void MudHighlighterMarkupUntilNextBoundaryTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, TEXT)
                .Add(x => x.HighlightedText, "it")
                .Add(x => x.UntilNextBoundary, true));
            comp.MarkupMatches("This is the first <mark>item</mark>");
        }

        /// <summary>
        /// Check markup with property 
        /// </summary>
        [Test]
        public async Task MudHighlighterMarkupCaseSensitiveTest()
        {
            var comp = Context.Render<MudHighlighter>(parameters => parameters
                .Add(x => x.Text, "This is this")
                .Add(x => x.HighlightedText, "this")
                .Add(x => x.CaseSensitive, true));
            //Case sensitive
            comp.MarkupMatches("This is <mark>this</mark>");
            //Case insensitive
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Text, "This is this")
                .Add(x => x.HighlightedText, "this")
                .Add(x => x.CaseSensitive, false));
            comp.MarkupMatches("<mark>This</mark> is <mark>this</mark>");
        }

        /// <summary>
        /// Check RenderFragment output using Markup property 
        /// </summary>
        [Test]
        public void MudHighlighterMarkupRenderFragmentTest()
        {
            var searchFor = "mud";
            var markupText = $"<i>MudBlazor</i>";
            var rawOutput = "&lt;i&gt;<mark>Mud</mark>Blazor&lt;/i&gt;";
            var formattedOutput = "<i><mark>Mud</mark>Blazor</i>";

            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, markupText)
                .Add(p => p.HighlightedText, searchFor)
                .Add(p => p.Markup, false)
            );

            comp.Markup.Should().Contain(rawOutput);

            comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, markupText)
                .Add(p => p.HighlightedText, searchFor)
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain(formattedOutput);
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HtmlInText_ShouldHighlightCorrectly()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "<span>Hello</span> World")
                .Add(p => p.HighlightedText, "Hello")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("<span><mark>Hello</mark></span> World");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HtmlSensitiveCharInHighlightedText_ShouldEncodeAndHighlight()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello <World>")
                .Add(p => p.HighlightedText, "<World>")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello <mark>&lt;World&gt;</mark>");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HtmlInText_ShouldNotHighlightInTags()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "<div class='foo'>div content div</div>")
                .Add(p => p.HighlightedText, "div")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("<div class='foo'><mark>div</mark> content <mark>div</mark></div>");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HtmlTag_ShouldNotHighlight()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello <i>Mud</i> World")
                .Add(p => p.HighlightedText, "<i>")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello <i>Mud</i> World");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_TextWithHtmlEntities_HighlightedTextIsEntityText()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello &amp; World")
                .Add(p => p.HighlightedText, "&amp;")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello <mark>&amp;amp;</mark> World");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HighlightedTextWithSingleQuotes_ShouldHighlight()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "This is a 'quoted' text and a \"double quoted\" text.")
                .Add(p => p.HighlightedText, "'quoted'")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("This is a <mark>&#39;quoted&#39;</mark> text and a &quot;double quoted&quot; text.");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HighlightedTextWithDoubleQuotes_ShouldHighlight()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "This is a 'quoted' text and a \"double quoted\" text.")
                .Add(p => p.HighlightedText, "\"double quoted\"")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("This is a &#39;quoted&#39; text and a <mark>&quot;double quoted&quot;</mark> text.");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_HighlightedTextAsAttributeValue_ShouldNotHighlightInAttribute()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "<i>MudBlazor</i> is <span style='color:red'>important</span>")
                .Add(p => p.HighlightedText, "nothing")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("<i>MudBlazor</i> is <span style='color:red'>important</span>");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_FormattingPreservation_ItalicsAndColor()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "<i>MudBlazor</i> is <span style='color:red'>important</span>")
                .Add(p => p.HighlightedText, "MudBlazor")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("<i><mark>MudBlazor</mark></i> is <span style='color:red'>important</span>");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_FormattingPreservation_Bold()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Normal <b>bold</b> normal")
                .Add(p => p.HighlightedText, "bold")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Normal <b><mark>bold</mark></b> normal");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_NonStandardTag_NoHighlight()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello <ambitious> world")
                .Add(p => p.HighlightedText, "")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello &lt;ambitious&gt; world");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_NonStandardTag_WithHighlightAfterTag()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello <ambitious> world")
                .Add(p => p.HighlightedText, "world")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello &lt;ambitious&gt; <mark>world</mark>");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_NonStandardTag_WithHighlightInsideTag()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Hello <ambitious> world")
                .Add(p => p.HighlightedText, "bit")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Hello &lt;am<mark>bit</mark>ious&gt; world");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_NoFragments_RendersTextAsMarkupString()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Some text")
                .Add(p => p.HighlightedText, "zip")
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Some text");
        }

        [Test]
        public void MudHighlighter_MarkupTrue_WithClass_RendersMarkWithClass()
        {
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, "Highlight this")
                .Add(p => p.HighlightedText, "Highlight")
                .Add(p => p.Markup, true)
                .Add(p => p.CustomClass, "my-custom-class")
            );

            comp.Markup.Should().Contain("<mark class=\"my-custom-class\" >Highlight</mark> this");
        }

        [Test]
        public async Task MudHighlighter_MarkupFalse_AfterMarkupTrue_ClearsHtmlAwareFragmentsAndRendersCorrectly()
        {
            var initialText = "Test with <b>HTML</b> and highlight";
            var initialHighlightedText = "highlight";

            // 1. Render initially with Markup = true
            var comp = Context.Render<BasicHighlighterTest>(parameters => parameters
                .Add(p => p.Text, initialText)
                .Add(p => p.HighlightedText, initialHighlightedText)
                .Add(p => p.Markup, true)
            );

            comp.Markup.Should().Contain("Test with <b>HTML</b> and <mark>highlight</mark>");

            // 2. Re-render with Markup = false
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Markup, false)
            );

            var expectedMarkup = "Test with &lt;b&gt;HTML&lt;/b&gt; and <mark>highlight</mark>";
            comp.Markup.Should().Contain(expectedMarkup);
        }
    }
}

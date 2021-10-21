using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace UtilityTests
{
    public class StyleBuilderTests
    {
        [Test]
        public void ShouldBuildConditionalInlineStyles()
        {
            //arrange
            var hasBorder = true;
            var isOnTop = false;
            var top = 2;
            var bottom = 10;
            var left = 4;
            var right = 20;

            //act
            var classToRender = new StyleBuilder("background-color", "DodgerBlue")
                            .AddStyle("border-width", $"{top}px {right}px {bottom}px {left}px", when: hasBorder)
                            .AddStyle("z-index", "999", when: isOnTop)
                            .AddStyle("z-index", "-1", when: !isOnTop)
                            .AddStyle("padding", "35px")
                            .Build();
            //assert
            classToRender.Should().Be("background-color:DodgerBlue;border-width:2px 20px 10px 4px;z-index:-1;padding:35px;");
        }

        [Test]
        public void ShouldBuildConditionalInlineStylesFromAttributes()
        {

            //arrange
            var hasBorder = true;
            var isOnTop = false;
            var top = 2;
            var bottom = 10;
            var left = 4;
            var right = 20;

            //act
            var styleToRender = new StyleBuilder("background-color", "DodgerBlue")
                            .AddStyle("border-width", $"{top}px {right}px {bottom}px {left}px", when: hasBorder)
                            .AddStyle("z-index", "999", when: isOnTop)
                            .AddStyle("z-index", "-1", when: !isOnTop)
                            .AddStyle("padding", "35px")
                            .Build();

            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "style", styleToRender } };

            var classToRender = new StyleBuilder().AddStyleFromAttributes(attributes).Build();
            //assert
            classToRender.Should().Be("background-color:DodgerBlue;border-width:2px 20px 10px 4px;z-index:-1;padding:35px;");
        }

        [Test]
        public void ShouldAddExistingStyle()
        {
            var styleToRender = StyleBuilder.Empty()
                .AddStyle("background-color:DodgerBlue;")
                .AddStyle("padding", "35px")
                .Build();

            var styleToRenderFromDefaultConstructor = StyleBuilder.Default(styleToRender).Build();

            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleToRender.Should().Be("background-color:DodgerBlue;;padding:35px;");
            styleToRenderFromDefaultConstructor.Should().Be("background-color:DodgerBlue;;padding:35px;;");

        }

        [Test]
        public void ShouldNotAddEmptyStyle()
        {
            var styleToRender = StyleBuilder.Empty().AddStyle("");

            styleToRender.NullIfEmpty().Should().BeNull();

        }

        [Test]
        public void ShouldAddNestedStyles()
        {


            var child = StyleBuilder.Empty()
                .AddStyle("background-color", "DodgerBlue")
                .AddStyle("padding", "35px");

            var styleToRender = StyleBuilder.Empty()
                .AddStyle(child)
                .AddStyle("z-index", "-1")
                .Build();

            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleToRender.Should().Be("background-color:DodgerBlue;padding:35px;z-index:-1;");

        }

        [Test]
        public void ShouldAddComplexStyles()
        {
            //var td = new StringBuilder();
            //if (hasStyle.Font_Underline) td.Append("underline ");
            //if (hasStyle.Font_Overline) td.Append("overline ");
            //if (hasStyle.Font_Strikeout) td.Append("line-through");

            //bool HasFontUnderline = true;
            //bool HasOverline = false;
            //bool HasStrikeout = true;

            var styleToRender = StyleBuilder.Empty()
                .AddStyle("text-decoration", v => v
                            .AddValue("underline", true)
                            .AddValue("overline", false)
                            .AddValue("line-through", true),
                            when: true)
                .AddStyle("z-index", "-1")
                .Build();

            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleToRender.Should().Be("text-decoration:underline line-through;z-index:-1;");

        }

        [Test]
        public void ShouldBuildStyleWithFunc()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

                //act
                var styleToRender = StyleBuilder.Empty()
                                .AddStyle("background-color", () => attributes["style"].ToString(), when: attributes.ContainsKey("style"))
                                .AddStyle("background-color", "black")
                                .Build();
                //assert
                styleToRender.Should().Be("background-color:black;");
            }
        }
    }
}

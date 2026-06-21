using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace UtilityTests
{
#nullable enable
    [TestFixture]
    public class StyleBuilderTests
    {
        [Test]
        public void AddStyle_With_Condition_Func_And_Nullable_Value_Adds_Style_Correctly()
        {
            // Arrange
            var styleBuilder = new StyleBuilder();

            // Act
            styleBuilder.AddStyle("color", "red", true);
            styleBuilder.AddStyle("font-size", "12px", false);
            styleBuilder.AddStyle("background-color", () => "blue", true);
            styleBuilder.AddStyle("border", () => "1px solid black", false);

            // Assert
            styleBuilder.Build().Should().Be("color:red;background-color:blue;");
        }

        [Test]
        public void AddStyle_With_Func_Value_And_Condition_Adds_Style_Correctly()
        {
            // Arrange
            var styleBuilder = new StyleBuilder();
            static bool Condition1() => true;
            static bool Condition2() => false;

            // Act
            styleBuilder.AddStyle("color", () => "red", Condition1);
            styleBuilder.AddStyle("font-size", () => "12px", Condition2);
            styleBuilder.AddStyle("background-color", () => "blue", Condition1);
            styleBuilder.AddStyle("border", () => "1px solid black", Condition2);

            // Assert
            styleBuilder.Build().Should().Be("color:red;background-color:blue;");
        }

        [Test]
        public void AddStyle_With_Builder_And_Condition_Func_Adds_Style_Correctly()
        {
            // Arrange
            var styleBuilder = new StyleBuilder();
            var nestedBuilder1 = new StyleBuilder().AddStyle("font-weight", "bold");
            var nestedBuilder2 = new StyleBuilder().AddStyle("text-decoration", "underline");
            static bool Condition() => true;

            // Act
            styleBuilder.AddStyle(nestedBuilder1, Condition);
            styleBuilder.AddStyle(nestedBuilder2, () => false);

            // Assert
            styleBuilder.Build().Should().Be("font-weight:bold;");
        }

        [Test]
        public void AddStyle_With_Value_And_Null_Condition()
        {
            // Arrange
            const string Prop = "color";
            const string Value = "red";

            // Act
            var styleBuilder = new StyleBuilder()
                .AddStyle(Prop, Value, null);

            // Assert
            styleBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void AddStyle_With_Value_And_Condition_Func_Adds_Style_Conditionally()
        {
            // Arrange
            const string Prop = "font-size";
            const string Value = "12px";
            bool Condition() => true;

            // Act
            var styleBuilder = new StyleBuilder()
                .AddStyle(Prop, Value, Condition);

            // Assert
            styleBuilder.Build().Should().Be("font-size:12px;");
        }

        [Test]
        public void AddStyle_With_Func_Value_And_Null_Condition()
        {
            // Arrange
            const string Prop = "color";
            string? Value() => "red";

            // Act
            var styleBuilder = new StyleBuilder()
                .AddStyle(Prop, Value, null);

            // Assert
            styleBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void AddStyle_With_Condition_Func_Null_Condition()
        {
            // Arrange
            var nestedBuilder = new StyleBuilder().AddStyle("font-weight", "bold");

            // Act
            var styleBuilder = new StyleBuilder()
                .AddStyle(nestedBuilder, null);

            // Assert
            styleBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void ShouldBuildConditionalInlineStyles()
        {
            // Arrange
            var hasBorder = true;
            var isOnTop = false;
            var top = 2;
            var bottom = 10;
            var left = 4;
            var right = 20;

            // Act
            var styleBuilder = new StyleBuilder("background-color", "DodgerBlue")
                            .AddStyle("border-width", $"{top}px {right}px {bottom}px {left}px", when: hasBorder)
                            .AddStyle("z-index", "999", when: isOnTop)
                            .AddStyle("z-index", "-1", when: !isOnTop)
                            .AddStyle("padding", "35px")
                            .Build();

            // Assert
            styleBuilder.Should().Be("background-color:DodgerBlue;border-width:2px 20px 10px 4px;z-index:-1;padding:35px;");
        }

        [Test]
        public void ShouldBuildConditionalInlineStylesFromAttributes()
        {
            // Arrange
            var hasBorder = true;
            var isOnTop = false;
            var top = 2;
            var bottom = 10;
            var left = 4;
            var right = 20;

            // Act
            var styleBuilder1 = new StyleBuilder("background-color", "DodgerBlue")
                            .AddStyle("border-width", $"{top}px {right}px {bottom}px {left}px", when: hasBorder)
                            .AddStyle("z-index", "999", when: isOnTop)
                            .AddStyle("z-index", "-1", when: !isOnTop)
                            .AddStyle("padding", "35px")
                            .Build();

            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "style", styleBuilder1 } };

            var styleBuilder2 = new StyleBuilder().AddStyleFromAttributes(attributes).Build();

            // Assert
            styleBuilder2.Should().Be("background-color:DodgerBlue;border-width:2px 20px 10px 4px;z-index:-1;padding:35px;");
        }

        [Test]
        public void ShouldAddExistingStyle()
        {
            // Arrange
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("background-color:DodgerBlue;")
                .AddStyle("padding", "35px")
                .Build();

            // Act
            var styleToRenderFromDefaultConstructor = StyleBuilder.Default(styleBuilder).Build();

            // Assert
            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleBuilder.Should().Be("background-color:DodgerBlue;;padding:35px;");
            styleToRenderFromDefaultConstructor.Should().Be("background-color:DodgerBlue;;padding:35px;;");

        }

        [Test]
        public void ShouldNotAddEmptyStyle()
        {
            // Act
            var styleBuilder = StyleBuilder.Empty().AddStyle("");

            // Assert
            styleBuilder.NullIfEmpty().Should().BeNull();
        }

        [Test]
        public void ShouldAddNestedStyles()
        {
            // Act
            var child = StyleBuilder.Empty()
                .AddStyle("background-color", "DodgerBlue")
                .AddStyle("padding", "35px");

            var styleBuilder = StyleBuilder.Empty()
                .AddStyle(child)
                .AddStyle("z-index", "-1")
                .Build();

            // Assert
            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleBuilder.Should().Be("background-color:DodgerBlue;padding:35px;z-index:-1;");
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

            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("text-decoration", v => v
                            .AddValue("underline", true)
                            .AddValue("overline", false)
                            .AddValue("line-through", true),
                            when: true)
                .AddStyle("z-index", "-1")
                .Build();

            // Assert
            // Double ;; is valid HTML.
            // The CSS syntax allows for empty declarations, which means that you can add leading and trailing semicolons as you like. For instance, this is valid CSS
            // .foo { ;;;display:none;;;color:black;;; }
            // Trimming is possible, but is it worth the operations for a non-issue?
            styleBuilder.Should().Be("text-decoration:underline line-through;z-index:-1;");

        }

        [Test]
        public void ShouldBuildStyleWithFunc()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("background-color", () => attributes["style"].ToString(), when: attributes.ContainsKey("style"))
                .AddStyle("background-color", "black")
                .Build();

            // Assert
            styleBuilder.Should().Be("background-color:black;");
        }

        [Test]
        public void ShouldAddStyleWithFunc()
        {
            // Arrange
            var styleToAdd = "background-color: green";

            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle(styleToAdd, () => true)
                .Build();

            // Assert
            styleBuilder.Should().Be("background-color: green;");
        }

        [Test]
        public void ShouldAddConditionalStyle()
        {
            // Arrange
            var styleToAdd = "background-color: green";

            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle(styleToAdd, true)
                .Build();

            // Assert
            styleBuilder.Should().Be("background-color: green;");
        }

        [Test]
        public void ShouldAddConditionalStyleWithNullFunc()
        {
            // Arrange
            var styleToAdd = "background-color: green";

            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle(styleToAdd, when: null)
                .Build();

            // Assert
            styleBuilder.Should().Be(string.Empty);
        }

        [Test]
        public void Default_With_Prop_And_Value_Seeds_Builder()
        {
            // Act
            var styleBuilder = StyleBuilder.Default("color", "red")
                .AddStyle("padding", "35px")
                .Build();

            // Assert
            styleBuilder.Should().Be("color:red;padding:35px;");
        }

        [Test]
        public void AddStyleFromAttributes_With_Null_Dictionary_Is_NoOp()
        {
            // Act
            var styleBuilder = StyleBuilder.Default("color", "red")
                .AddStyleFromAttributes(null)
                .Build();

            // Assert
            styleBuilder.Should().Be("color:red;");
        }

        [Test]
        public void AddStyleFromAttributes_Without_Style_Key_Is_NoOp()
        {
            // Arrange
            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-class" } };

            // Act
            var styleBuilder = StyleBuilder.Default("color", "red")
                .AddStyleFromAttributes(attributes)
                .Build();

            // Assert
            styleBuilder.Should().Be("color:red;");
        }

        [Test]
        public void AddStyle_With_ValueBuilder_When_False_Skips_Property()
        {
            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("text-decoration", v => v.AddValue("underline", true), when: false)
                .AddStyle("z-index", "-1")
                .Build();

            // Assert
            styleBuilder.Should().Be("z-index:-1;");
        }

        [Test]
        public void AddStyle_With_ValueBuilder_Producing_No_Value_Skips_Property()
        {
            // Act
            // All values are conditionally excluded, so HasValue is false and the property is dropped.
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("text-decoration", v => v
                            .AddValue("underline", false)
                            .AddValue("overline", false),
                            when: true)
                .AddStyle("z-index", "-1")
                .Build();

            // Assert
            styleBuilder.Should().Be("z-index:-1;");
        }

        [Test]
        public void AddStyle_With_Func_Value_And_True_Condition_Func_Adds_Style()
        {
            // Act
            var styleBuilder = StyleBuilder.Empty()
                .AddStyle("color", () => "red", () => true)
                .Build();

            // Assert
            styleBuilder.Should().Be("color:red;");
        }

        [Test]
        public void ToString_Returns_Same_As_Build()
        {
            // Arrange
            var styleBuilder = StyleBuilder.Default("color", "red")
                .AddStyle("padding", "35px");

            // Act & Assert
            styleBuilder.ToString().Should().Be("color:red;padding:35px;");
        }
    }
}

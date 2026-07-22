// Copyright (c) 2011 - 2019 Ed Charbeneau
// License: MIT
// See https://github.com/EdCharbeneau

using System;
using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace UtilityTests
{
#nullable enable
    [TestFixture]
    public class CssBuilderTests
    {
        [Test]
        public void Default_Returns_Instance_With_Prop_And_Value()
        {
            // Arrange
            var prop = "color";
            var value = "red";

            // Act
            var styleBuilder = StyleBuilder.Default(prop, value).Build();

            // Assert
            styleBuilder.Should().Be("color:red;");
        }

        [Test]
        public void Default_Returns_Instance_With_Value()
        {
            // Arrange
            var value = "test-class";

            // Act
            var cssBuilder = CssBuilder.Default(value).Build();

            // Assert
            cssBuilder.Should().Be("test-class");
        }

        [Test]
        public void Empty_Returns_Instance_With_Empty_Value()
        {
            // Act
            var cssBuilder = CssBuilder.Empty();

            // Assert
            cssBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void Empty_Returns_Instance_With_Null_Value()
        {
            // Act
            var classToRender = CssBuilder.Empty().NullIfEmpty();

            // Assert
            classToRender.Should().BeNull();
        }

        [Test]
        public void AddClass_Adds_Class_Correctly()
        {
            // Arrange
            var cssBuilder = new CssBuilder();

            // Act
            cssBuilder.AddClass("class1");
            cssBuilder.AddClass("class2");
            cssBuilder.AddClass("class3");

            // Assert
            cssBuilder.Build().Should().Be("class1 class2 class3");
        }

        [Test]
        public void AddClass_With_Condition_Adds_Class_Correctly()
        {
            // Arrange
            const bool HasTwo = false;
            const bool HasThree = true;
            static bool HasFive() => false;

            // Act
            var classToRender = new CssBuilder("item-one")
                            .AddClass("item-two", when: HasTwo)
                            .AddClass("item-three", when: HasThree)
                            .AddClass("item-four")
                            .AddClass("item-five", when: HasFive)
                            .Build();


            // Assert
            classToRender.Should().Be("item-one item-three item-four");
        }

        [Test]
        public void AddClass_With_Condition_Func_Adds_Class_Correctly()
        {
            // Arrange
            var cssBuilder = new CssBuilder();
            static bool Condition1() => true;
            static bool Condition2() => false;

            // Act
            cssBuilder.AddClass("class1", Condition1);
            cssBuilder.AddClass("class2", Condition2);
            cssBuilder.AddClass("class3", Condition1);

            // Assert
            cssBuilder.Build().Should().Be("class1 class3");
        }

        [Test]
        public void AddClass_With_Value_And_Condition_Func_Adds_Class_Correctly()
        {
            // Arrange
            const bool ConditionResult = true;

            // Act
            var cssBuilder = new CssBuilder()
                .AddClass("class1", () => ConditionResult)
                .AddClass("class2", () => !ConditionResult)
                .AddClass("class3", () => ConditionResult);

            // Assert
            cssBuilder.Build().Should().Be("class1 class3");
        }

        [Test]
        public void AddClass_With_Value_Function_And_Condition_Func_Adds_Class_Correctly()
        {
            // Arrange
            const bool ConditionResult = true;
            Func<string?> valueFunction = () => "class1";

            // Act
            var cssBuilder = new CssBuilder()
                .AddClass(valueFunction, () => ConditionResult)
                .AddClass(valueFunction, () => !ConditionResult);

            // Assert
            cssBuilder.Build().Should().Be("class1");
        }

        [Test]
        public void AddClass_With_CssBuilder_And_Condition_Func_Adds_Class_Correctly()
        {
            // Arrange
            const bool ConditionResult = true;
            var nestedBuilder = new CssBuilder().AddClass("nested-class");

            // Act
            var cssBuilder = new CssBuilder()
                .AddClass(nestedBuilder, () => ConditionResult)
                .AddClass(nestedBuilder, () => !ConditionResult);

            // Assert
            cssBuilder.Build().Should().Be("nested-class");
        }

        [Test]
        public void AddClass_With_Value_Function_When_Null()
        {
            // Arrange
            string? ValueFunction() => "class1";

            // Act
            var cssBuilder = new CssBuilder()
                .AddClass(ValueFunction, null);

            // Assert
            cssBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void AddClass_With_CssBuilder_When_Null()
        {
            // Arrange
            var nestedBuilder = new CssBuilder().AddClass("nested-class");

            // Act
            var cssBuilder = new CssBuilder()
                .AddClass(nestedBuilder, null);

            // Assert
            cssBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void AddClassFromAttributes_With_Null_Dictionary()
        {
            // Act
            var cssBuilder = new CssBuilder().AddClassFromAttributes(null);

            // Assert
            cssBuilder.Build().Should().BeEmpty();
        }

        [Test]
        public void Should_Build_Conditional_CssBuilder_Classes()
        {
            // Arrange
            const bool HasTwo = false;
            const bool HasThree = true;
            static bool HasFive() => false;

            // Act
            var cssBuilder = new CssBuilder("item-one")
                            .AddClass("item-two", when: HasTwo)
                            .AddClass(new CssBuilder("item-three")
                                            .AddClass("item-foo", false)
                                            .AddClass("item-sub-three"),
                                            when: HasThree)
                            .AddClass("item-four")
                            .AddClass("item-five", when: HasFive)
                            .Build();

            // Assert
            cssBuilder.Should().Be("item-one item-three item-sub-three item-four");
        }

        [Test]
        public void Should_Build_Empty_Classes()
        {
            // Arrange
            const bool ShouldShow = false;

            // Act
            var cssBuilder = new CssBuilder()
                            .AddClass("some-class", ShouldShow)
                            .Build();

            // Assert
            cssBuilder.Should().Be(string.Empty);
        }

        [Test]
        public void Should_Build_Classes_WithFunc()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

            // Act
            var cssBuilder = new CssBuilder("item-one")
                .AddClass(() => attributes["class"].ToString(), when: attributes.ContainsKey("class"))
                .Build();

            // Assert
            cssBuilder.Should().Be("item-one my-custom-class-1");
        }

        [Test]
        public void Should_Build_Classes_FromAttributes()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

            // Act
            var cssBuilder = new CssBuilder("item-one")
                .AddClassFromAttributes(attributes)
                .Build();

            // Assert
            cssBuilder.Should().Be("item-one my-custom-class-1");
        }

        [Test]
        public void Should_NotThrow_WhenNull_For_BuildClasses_FromAttributes()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object>? attributes = null;

            // Act
            var cssBuilder = new CssBuilder("item-one")
                .AddClassFromAttributes(attributes)
                .Build();

            // Assert
            cssBuilder.Should().Be("item-one");
        }

        [Test]
        public void ForceNullForWhitespace_BuildClassesFromAttributes()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object>? attributes = null;

            // Act
            var cssBuilder = new CssBuilder()
                .AddClassFromAttributes(attributes)
                .NullIfEmpty();

            // Assert
            cssBuilder.Should().BeNull();
        }

        [Test]
        public void Should_NotThrowNoKeyException_WithDictionary()
        {
            // Arrange
            // Simulates Razor Components attribute splatting feature
            IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "foo", "bar" } };

            // Act
            var classToRender = new CssBuilder("item-one")
                .AddClass(() => attributes["string"].ToString(), when: attributes.ContainsKey("class"))
                .Build();

            // Assert
            classToRender.Should().Be("item-one");
        }
    }
}

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
    public class CssBuilderTests
    {
        [Test]
        public void ShouldConstructWithDefaultValue()
        {
            //arrange
            var classToRender = CssBuilder.Default("item-one").Build();

            //assert
            classToRender.Should().Be("item-one");
        }


        [Test]
        public void ShouldConstructWithEmpty()
        {
            //arrange
            var classToRender = CssBuilder.Empty().NullIfEmpty();

            //assert
            classToRender.Should().BeNull();
        }

        [Test]
        public void ShouldBuildConditionalCssClasses()
        {
            //arrange
            var hasTwo = false;
            var hasThree = true;
            Func<bool> hasFive = () => false;

            //act
            var classToRender = new CssBuilder("item-one")
                            .AddClass("item-two", when: hasTwo)
                            .AddClass("item-three", when: hasThree)
                            .AddClass("item-four")
                            .AddClass("item-five", when: hasFive)
                            .Build();
            //assert
            classToRender.Should().Be("item-one item-three item-four");
        }
        [Test]
        public void ShouldBuildConditionalCssBuilderClasses()
        {
            //arrange
            var hasTwo = false;
            var hasThree = true;
            Func<bool> hasFive = () => false;

            //act
            var classToRender = new CssBuilder("item-one")
                            .AddClass("item-two", when: hasTwo)
                            .AddClass(new CssBuilder("item-three")
                                            .AddClass("item-foo", false)
                                            .AddClass("item-sub-three"),
                                            when: hasThree)
                            .AddClass("item-four")
                            .AddClass("item-five", when: hasFive)
                            .Build();
            //assert
            classToRender.Should().Be("item-one item-three item-sub-three item-four");
        }
        [Test]
        public void ShouldBuildEmptyClasses()
        {
            //arrange
            var shouldShow = false;

            //act
            var classToRender = new CssBuilder()
                            .AddClass("some-class", shouldShow)
                            .Build();
            //assert
            classToRender.Should().Be(string.Empty);
        }

        [Test]
        public void ShouldBuildClassesWithFunc()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

                //act
                var classToRender = new CssBuilder("item-one")
                                .AddClass(() => attributes["class"].ToString(), when: attributes.ContainsKey("class"))
                                .Build();
                //assert
                classToRender.Should().Be("item-one my-custom-class-1");
            }
        }

        [Test]
        public void ShouldBuildClassesFromAttributes()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "class", "my-custom-class-1" } };

                //act
                var classToRender = new CssBuilder("item-one")
                                .AddClassFromAttributes(attributes)
                                .Build();
                //assert
                classToRender.Should().Be("item-one my-custom-class-1");
            }
        }

        [Test]
        public void ShouldNotThrowWhenNullFor_BuildClassesFromAttributes()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = null;

                //act
                var classToRender = new CssBuilder("item-one")
                                .AddClassFromAttributes(attributes)
                                .Build();
                //assert
                classToRender.Should().Be("item-one");
            }
        }

        [Test]
        public void ForceNullForWhitespace_BuildClassesFromAttributes()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = null;

                //act
                var classToRender = new CssBuilder()
                                .AddClassFromAttributes(attributes)
                                .NullIfEmpty();
                //assert
                classToRender.Should().BeNull();
            }
        }

        [Test]
        public void ShouldNotThrowNoKeyExceptionWithDictionary()
        {
            {
                //arrange
                // Simulates Razor Components attribute splatting feature
                IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object> { { "foo", "bar" } };

                //act
                var classToRender = new CssBuilder("item-one")
                                .AddClass(() => attributes["string"].ToString(), when: attributes.ContainsKey("class"))
                                .Build();
                //assert
                classToRender.Should().Be("item-one");
            }
        }
    }
}

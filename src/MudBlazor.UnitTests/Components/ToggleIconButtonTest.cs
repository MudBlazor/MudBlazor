﻿#pragma warning disable IDE1006 // leading underscore

using System;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleIconButtonTest
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void DefaultValueTest()
        {
            using var comp = ctx.RenderComponent<MudToggleIconButton>();
            comp.Instance.Toggled.Should().BeFalse();
        }

        [Test]
        public void ToggleTest()
        {
            var boundValue = false;
            using var comp = ctx.RenderComponent<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Toggled, boundValue)
                .Add(p => p.ToggledChanged, (toggleValue) => boundValue = toggleValue)
                );
            comp.Find("button").Click();
            boundValue.Should().BeTrue();
            comp.Find("button").Click();
            boundValue.Should().BeFalse();
            comp.RenderCount.Should().Be(3);
        }

        [Test]
        public void ShouldSynchronizeStateWithOtherComponent()
        {
            using var comp = ctx.RenderComponent<ToggleIconButtonTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponents<MudToggleIconButton>();
            var comp1 = group[0];
            var comp2 = group[1];
            // check initial state
            comp1.Instance.Toggled.Should().BeFalse();
            comp2.Instance.Toggled.Should().BeFalse();
            // click first button
            comp1.Find("button").Click();
            // make sure both buttons state changed
            comp1.Instance.Toggled.Should().BeTrue();
            comp2.Instance.Toggled.Should().BeTrue();
        }

        /// <summary>
        /// MudToggledIconButton should change title if specified
        /// </summary>
        [Test]
        public void ShouldRenderToggledTitle()
        {
            var title = "Title and tooltip";
            var toggledTitle = "toggled!";
            var icon = Parameter(nameof(MudToggleIconButton.Icon), Icons.Filled.Add);
            var toggledIcon = Parameter(nameof(MudToggleIconButton.ToggledIcon), Icons.Filled.Remove);
            var titleParam = Parameter(nameof(MudToggleIconButton.Title), title);
            var toggledTitleParam = Parameter(nameof(MudToggleIconButton.ToggledTitle), toggledTitle);
            using var comp = ctx.RenderComponent<MudToggleIconButton>(icon, toggledIcon, titleParam, toggledTitleParam);
            comp.Find("svg Title").TextContent.Should().Be(title);
            comp.Find("button").Click();
            comp.Find("svg Title").TextContent.Should().Be(toggledTitle);
            comp.Find("button").Click();
            comp.Find("svg Title").TextContent.Should().Be(title);
        }
    }
}

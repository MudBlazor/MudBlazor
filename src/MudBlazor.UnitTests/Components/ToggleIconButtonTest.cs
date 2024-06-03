﻿
using System;
using System.Collections.Generic;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleIconButtonTest : BunitTest
    {
        [Test]
        public void DefaultValueTest()
        {
            var comp = Context.RenderComponent<MudToggleIconButton>();
            comp.Instance.Toggled.Should().BeFalse();
        }

        [Test]
        public void ToggleTest()
        {
            var boundValue = false;
            var comp = Context.RenderComponent<MudToggleIconButton>(parameters => parameters
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
            var comp = Context.RenderComponent<ToggleIconButtonTest1>();
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

        [Test]
        public void ToggleButton_ShouldToggleUserAttributes()
        {
            // Arrange
            var userAttributes = new Dictionary<string, object>
            {
                { "title", "Untoggled Button" },
                { "aria-label", "untoggled" }
            };

            var toggledUserAttributes = new Dictionary<string, object>
            {
                { "title", "Toggled Button" },
                { "aria-label", "toggled" }
            };

            var component = Context.RenderComponent<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Toggled, false)
                .Add(p => p.UserAttributes, userAttributes)
                .Add(p => p.ToggledUserAttributes, toggledUserAttributes)
            );

            // Assert untoggled.
            component.Find("button").GetAttribute("title").Should().Be("Untoggled Button");
            component.Find("button").GetAttribute("aria-label").Should().Be("untoggled");

            component.Find("button").Click();

            // Assert toggled.
            component.Find("button").GetAttribute("title").Should().Be("Toggled Button");
            component.Find("button").GetAttribute("aria-label").Should().Be("toggled");
        }
    }
}

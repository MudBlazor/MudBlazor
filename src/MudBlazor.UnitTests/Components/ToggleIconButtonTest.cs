
using System;
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

        /// <summary>
        /// MudToggledIconButton should change title if specified
        /// </summary>
        [Test]
        public void ShouldRenderToggledTitle()
        {
            var title = "Title and tooltip";
            var toggledTitle = "toggled!";
            var icon = Parameter(nameof(MudToggleIconButton.Icon), Icons.Material.Filled.Add);
            var toggledIcon = Parameter(nameof(MudToggleIconButton.ToggledIcon), Icons.Material.Filled.Remove);
            var titleParam = Parameter(nameof(MudToggleIconButton.Title), title);
            var toggledTitleParam = Parameter(nameof(MudToggleIconButton.ToggledTitle), toggledTitle);
            var comp = Context.RenderComponent<MudToggleIconButton>(icon, toggledIcon, titleParam, toggledTitleParam);
            comp.Find($"button[title=\"{title}\"]");
            comp.Find("button").Click();
            comp.Find($"button[title=\"{toggledTitle}\"]");
            comp.Find("button").Click();
            comp.Find($"button[title=\"{title}\"]");
        }
    }
}

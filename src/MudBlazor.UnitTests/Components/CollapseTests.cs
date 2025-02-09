// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Collapse;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CollapseTests : BunitTest
    {
        [Test]
        public void Collapse_TwoWayBinding_Test1()
        {
            var comp = Context.RenderComponent<CollapseBindingTest>();
            IElement Button() => comp.Find("#outside_btn");

            IRenderedComponent<MudSwitch<bool>> MudSwitch() => comp.FindComponent<MudSwitch<bool>>();
            // Initial state is expanded
            MudSwitch().Find("input").GetAttribute("aria-checked").Should().Be("true");

            // Collapse via button
            Button().Click();
            MudSwitch().Find("input").GetAttribute("aria-checked").Should().Be("false");

            // Expand via button
            Button().Click();
            MudSwitch().Find("input").GetAttribute("aria-checked").Should().Be("true");

            // Collapse via switch
            MudSwitch().Find("input").Change(false);
            MudSwitch().Find("input").GetAttribute("aria-checked").Should().Be("false");

            // Expand via switch
            MudSwitch().Find("input").Change(true);
            MudSwitch().Find("input").GetAttribute("aria-checked").Should().Be("true");
        }
    }
}

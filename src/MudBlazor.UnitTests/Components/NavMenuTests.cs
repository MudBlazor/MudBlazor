
using System;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavMenuTests : BunitTest
    {

        [Test]
        /// <summary>
        /// Change all styling parameters so that all default values have the correct classes.
        /// </summary>
        public void NavMenuTests_DefaultValues()
        {
            var comp = Context.RenderComponent<MudNavMenu>();

            comp.Instance.Bordered.Should().Be(false);
            comp.Instance.Color.Should().Be(Color.Default);
            comp.Instance.Dense.Should().Be(false);
            comp.Instance.Margin.Should().Be(Margin.None);
            comp.Instance.Rounded.Should().Be(false);

            comp.FindAll("mud-navmenu-bordered").Count.Should().Be(0);
            comp.FindAll("mud-navmenu-success").Count.Should().Be(0);
            comp.FindAll("mud-navmenu-dense").Count.Should().Be(0);
            comp.FindAll("mud-navmenu-margin-dense").Count.Should().Be(0);
            comp.FindAll("mud-navmenu-rounded").Count.Should().Be(0);
        }

        [Test]
        /// <summary>
        /// Change all styling parameters from its default values and check that the correct classes are added.
        /// </summary>
        public void NavMenuTests_CheckAllStyling()
        {
            var comp = Context.RenderComponent<MudNavMenu>(x =>
            {
                x.Add(p => p.Bordered, true);
                x.Add(p => p.Color, Color.Success);
                x.Add(p => p.Dense, true);
                x.Add(p => p.Margin, Margin.Dense);
                x.Add(p => p.Rounded, true);
            });

            comp.Markup.Should().Contain("mud-navmenu-bordered");
            comp.Markup.Should().Contain("mud-navmenu-success");
            comp.Markup.Should().Contain("mud-navmenu-dense");
            comp.Markup.Should().Contain("mud-navmenu-margin-dense");
            comp.Markup.Should().Contain("mud-navmenu-rounded");
        }


        [Test]
        /// <summary>
        /// This component is initially Expanded with the property Expand set to imutable true <c>Expand=true</c>
        /// And even so, he changes when clicked
        /// </summary>
        public void One_Way_Bindable()
        {
            var comp = Context.RenderComponent<NavMenuOneWay>();
            comp.Markup.Should().Contain("expanded");

            var navgroup = comp.Find(".mud-nav-group>button");
            navgroup.Click();

            comp.Markup.Should().NotContain("expanded");
        }

        /// <summary>
        /// This component has a field _isExpanded two-way bound to Expanded property
        /// Initially is set to false and after clicking the navgroup should change to true
        /// </summary>
        [Test]
        public void Two_Way_Bindable()
        {
            var comp = Context.RenderComponent<NavMenuTwoWay>();
            comp.Markup.Should().NotContain("expanded");
            var isExpanded = comp.Instance._isExpanded;
            isExpanded.Should().BeFalse();

            var navgroup = comp.Find(".mud-nav-group>button");
            navgroup.Click();

            isExpanded = comp.Instance._isExpanded;
            isExpanded.Should().BeTrue();
            comp.Markup.Should().Contain("expanded");
        }
    }
}

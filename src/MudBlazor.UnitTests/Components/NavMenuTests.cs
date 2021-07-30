
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavMenuTexts : BunitTest
    {
        /// <summary>
        /// This component is initially Expanded with the property Expand set to imutable true <c>Expand=true</c>
        /// And even so, he changes when clicked
        /// </summary>
        [Test]
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

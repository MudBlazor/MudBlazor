#pragma warning disable CS1998 // async without await

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.NavMenu;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavGroupTests : BunitTest
    {
        /// <summary>
        /// Checking the disable group button disables the group and it's children
        /// Adding the mud-nav-group-disabled css tag to the group
        /// </summary>
        [Test]
        public async Task Two_Way_Bindable_Disabled()
        {
            var comp = Context.RenderComponent<NavMenuGroupDisabledTest>();

            comp.Markup.Should().NotContain("mud-nav-group-disabled");
            comp.Markup.Should().NotContain("mud-expanded");

            comp.Find("input").Change(true);

            comp.Markup.Should().Contain("mud-nav-group-disabled");
        }

        /// <summary>
        /// NavGroup should generate a nav tag with an aria-label.
        /// </summary>
        [Test]
        public void NavGroup_Should_UseNavTag()
        {
            var expectedTitle = "navgroup-title";
            var comp = Context.RenderComponent<MudNavGroup>(parameters =>
                    parameters.Add(p => p.Title, expectedTitle));

            comp.FindAll("nav").Should().Contain(navNode => navNode.GetAttribute("aria-label") == expectedTitle);
        }
    }
}

#pragma warning disable CS1998 // async without await

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
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
        /// <returns></returns>
        [Test]
        public async Task Two_Way_Bindable_Disabled()
        {
            var comp = Context.RenderComponent<NavMenuGroupDisabledTest>();

            comp.Markup.Should().NotContain("mud-nav-group-disabled");
            comp.Markup.Should().NotContain("expanded");

            var input = comp.Find("input"); // Change IsDisabled to True
            input.Change(true);

            comp.Markup.Should().Contain("mud-nav-group-disabled");
        }
    }
}

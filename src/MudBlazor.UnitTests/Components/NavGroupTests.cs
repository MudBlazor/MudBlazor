using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

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
        public void Two_Way_Bindable_Disabled()
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

        /// <summary>
        /// NavGroup should expand and collapse via Expanded binding.
        /// </summary>
        [Test]
        public async Task NavGroup_Should_Expand_Via_Expanded_Binding()
        {
            var comp = Context.RenderComponent<NavGroupWithExpandedBindingTest>();
            GetExpandedState().Should().BeFalse();

            await comp.InvokeAsync(() => comp.Find("#navgroup-switch").Change(true));

            GetExpandedState().Should().BeTrue();

            await comp.InvokeAsync(() => comp.Find("#navgroup-switch").Change(false));

            GetExpandedState().Should().BeFalse();
            return;

            bool GetExpandedState() => comp.FindComponent<MudCollapse>().Instance.Expanded;
        }
    }
}

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.NavMenu;
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
        public async Task Two_Way_Bindable_Disabled()
        {
            var comp = Context.Render<NavMenuGroupDisabledTest>();

            comp.Markup.Should().NotContain("mud-nav-group-disabled");
            comp.Markup.Should().NotContain("mud-expanded");

            await comp.Find("input").ChangeAsync(true);

            comp.Markup.Should().Contain("mud-nav-group-disabled");
        }

        /// <summary>
        /// NavGroup should generate a nav tag with an aria-label.
        /// </summary>
        [Test]
        public void NavGroup_Should_UseNavTag()
        {
            var expectedTitle = "navgroup-title";
            var comp = Context.Render<MudNavGroup>(parameters =>
                    parameters.Add(p => p.Title, expectedTitle));

            comp.FindAll("nav").Should().Contain(navNode => navNode.GetAttribute("aria-label") == expectedTitle);
        }

        /// <summary>
        /// NavGroup should expand and collapse via Expanded binding.
        /// </summary>
        [Test]
        public async Task NavGroup_Should_Expand_Via_Expanded_Binding()
        {
            var comp = Context.Render<NavGroupWithExpandedBindingTest>();
            GetExpandedState().Should().BeFalse();

            await comp.Find("#navgroup-switch").ChangeAsync(true);

            GetExpandedState().Should().BeTrue();

            await comp.Find("#navgroup-switch").ChangeAsync(false);

            GetExpandedState().Should().BeFalse();
            return;

            bool GetExpandedState() => comp.FindComponent<MudCollapse>().Instance.Expanded;
        }

        /// <summary>
        /// The nav landmark takes its accessible name from Title, which a caller must be able to override.
        /// </summary>
        [Test]
        public void NavGroup_Should_LetUserAttributesOverrideAriaLabel()
        {
            var comp = Context.Render<MudNavGroup>(parameters => parameters
                .Add(p => p.Title, "Reports")
                .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "aria-label", "Quarterly reports" } }));

            comp.Find("nav").GetAttribute("aria-label").Should().Be("Quarterly reports");
        }

        /// <summary>
        /// Title still names the landmark when the caller does not supply an aria-label.
        /// </summary>
        [Test]
        public void NavGroup_Should_UseTitleAsAriaLabel()
        {
            var comp = Context.Render<MudNavGroup>(parameters => parameters
                .Add(p => p.Title, "Reports"));

            comp.Find("nav").GetAttribute("aria-label").Should().Be("Reports");
        }
    }
}


using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents.NavMenu;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavMenuTests : BunitTest
    {
        /// <summary>
        /// Change all styling parameters so that all default values have the correct classes.
        /// </summary>
        [Test]
        public void NavMenuTests_DefaultValues()
        {
            var comp = Context.Render<MudNavMenu>();

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
        public async Task Exclusive_OnlyOneOpen()
        {
            var comp = Context.Render<NavMenuExclusive>();

            var buttons = comp.FindAll(".mud-nav-group>button");
            buttons.Count.Should().BeGreaterThanOrEqualTo(2);

            // Expand first
            await buttons[0].ClickAsync();
            comp.Markup.Should().Contain("mud-expanded");
            // Expand second. Should collapse first because MultiExpansion==false by default in component
            await buttons[1].ClickAsync();
            int expandedCount = comp.FindAll(".mud-expanded").Count;
            expandedCount.Should().Be(1);
        }

        [Test]
        public async Task NonExclusive_AllowsMultipleOpen()
        {
            var comp = Context.Render<NavMenuExclusive>(ps => ps.Add(p => p.MultiExpansion, true));

            var buttons = comp.FindAll(".mud-nav-group>button");
            buttons.Count.Should().BeGreaterThanOrEqualTo(2);

            // Expand first
            await buttons[0].ClickAsync();
            // Expand second. Should not collapse first because MultiExpansion==true
            await buttons[1].ClickAsync();
            int expandedCount = comp.FindAll(".mud-expanded").Count;
            expandedCount.Should().Be(2);
        }

        [Test]
        public async Task DefaultMultiExpansion_AllowsMultipleGroupsOpen()
        {
            static void CreateGroups(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<MudNavGroup>(0);
                builder.AddAttribute(1, nameof(MudNavGroup.Title), "Group 1");
                builder.CloseComponent();

                builder.OpenComponent<MudNavGroup>(2);
                builder.AddAttribute(3, nameof(MudNavGroup.Title), "Group 2");
                builder.CloseComponent();
            }

            // Intentionally omit MultiExpansion. This protects existing menus from
            // silently becoming exclusive if the component default changes.
            var comp = Context.Render<MudNavMenu>(parameters =>
                parameters.Add(p => p.ChildContent, CreateGroups));

            comp.Instance.MultiExpansion.Should().BeTrue();

            var buttons = comp.FindAll(".mud-nav-group > button");
            await buttons[0].ClickAsync();
            await buttons[1].ClickAsync();

            comp.FindAll(".mud-nav-group > button.mud-expanded")
                .Should()
                .HaveCount(2);
        }

        /// <summary>
        /// Change all styling parameters from its default values and check that the correct classes are added.
        /// </summary>
        [Test]
        public void NavMenuTests_CheckAllStyling()
        {
            var comp = Context.Render<MudNavMenu>(x =>
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

        /// <summary>
        /// This component is initially Expanded with the property Expand set to immutable true <c>Expand=true</c>
        /// And even so, he changes when clicked
        /// </summary>
        [Test]
        public async Task One_Way_Bindable()
        {
            var comp = Context.Render<NavMenuOneWay>();
            comp.Markup.Should().Contain("mud-expanded");
            comp.Markup.Should().Contain("aria-hidden=\"false\"");

            var navgroup = comp.Find(".mud-nav-group>button");
            await navgroup.ClickAsync();

            comp.Markup.Should().NotContain("mud-expanded");
            comp.Markup.Should().Contain("aria-hidden=\"true\"");
        }

        /// <summary>
        /// This component has a field _expanded two-way bound to Expanded property
        /// Initially is set to false and after clicking the navgroup should change to true
        /// </summary>
        [Test]
        public async Task Two_Way_Bindable()
        {
            var comp = Context.Render<NavMenuTwoWay>();
            comp.Markup.Should().NotContain("mud-expanded");
            comp.Markup.Should().Contain("aria-hidden=\"true\"");
            var expanded = comp.Instance.Expanded;
            expanded.Should().BeFalse();

            var navgroup = comp.Find(".mud-nav-group>button");
            await navgroup.ClickAsync();

            expanded = comp.Instance.Expanded;
            expanded.Should().BeTrue();
            comp.Markup.Should().Contain("mud-expanded");
            comp.Markup.Should().Contain("aria-hidden=\"false\"");
        }

        /// <summary>
        /// A caller-supplied id used to be erased by the trailing null literal, leaving the nav landmark with no id at all.
        /// </summary>
        [Test]
        public void NavMenu_Should_KeepUserSuppliedId()
        {
            var comp = Context.Render<MudNavMenu>(parameters => parameters
                .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "id", "main-nav" } }));

            comp.Find("nav").GetAttribute("id").Should().Be("main-nav");
        }

        /// <summary>
        /// Inside a nav group the generated menu id must still win so the toggle button's aria-controls stays wired.
        /// </summary>
        [Test]
        public void NavMenu_Should_KeepGeneratedMenuIdInsideNavGroup()
        {
            var comp = Context.Render<MudNavGroup>(parameters => parameters
                .Add(p => p.Expanded, true));

            var controls = comp.Find("button").GetAttribute("aria-controls");
            controls.Should().NotBeNullOrEmpty();
            comp.FindAll("nav").Should().Contain(nav => nav.GetAttribute("id") == controls);
        }
    }
}

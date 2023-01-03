
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BreadcrumbTests : BunitTest
    {
        [Test]
        public void MudBreadcrumbs_ShouldRenderItemsWithSeparators()
        {
            var comp = Context.RenderComponent<MudBreadcrumbs>(Parameter("Items", new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Link 1", "link1"),
                new BreadcrumbItem("Link 2", "link2"),
                new BreadcrumbItem("Link 3", "link3", disabled: true)
            }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(3);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(2);
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderItemsWithIcons()
        {
            var comp = Context.RenderComponent<MudBreadcrumbs>(Parameter("Items", new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Link 1", "link1", icon: Icons.Material.Filled.Home),
                new BreadcrumbItem("Link 2", "link2", icon: Icons.Material.Filled.List),
                new BreadcrumbItem("Link 3", "link3", disabled: true, icon: Icons.Material.Filled.Create)
            }));

            comp.FindAll("li>a>svg").Should().HaveCount(3);
        }

        [Test]
        public void MudBreadcrumbs_ShouldCollapseWhenMaxItemsIsReached()
        {
            var comp = Context.RenderComponent<MudBreadcrumbs>(Parameter("MaxItems", (byte)4), Parameter("Items", new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Link 1", "link1"),
                new BreadcrumbItem("Link 2", "link2"),
                new BreadcrumbItem("Link 3", "link3"),
                new BreadcrumbItem("Link 4", "link4"),
                new BreadcrumbItem("Link 5", "link5", disabled: true)
            }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(2);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(2);
            comp.Find("li.mud-breadcrumbs-expander").Should().NotBeNull();
        }

        [Test]
        public async Task MudBreadcrumbs_Other()
        {
            var comp = Context.RenderComponent<MudBreadcrumbs>(Parameter("MaxItems", (byte)4), Parameter("Items", new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Link 1", "link1"),
                new BreadcrumbItem("Link 2", "link2"),
                new BreadcrumbItem("Link 3", "link3"),
                new BreadcrumbItem("Link 4", "link4"),
                new BreadcrumbItem("Link 5", "link5", disabled: true)
            }));

            comp.WaitForAssertion(() => comp.Instance.Collapsed.Should().BeTrue());
            await comp.InvokeAsync(() => comp.Instance.Expand());
            comp.WaitForAssertion(() => comp.Instance.Collapsed.Should().BeFalse());

            await comp.InvokeAsync(() => comp.Instance.Expand());
            comp.WaitForAssertion(() => comp.Instance.Collapsed.Should().BeFalse());

            comp.WaitForAssertion(() => MudBreadcrumbs.GetItemClassname(comp.Instance.Items[1]).Should().Be("mud-breadcrumb-item"));
        }
    }
}

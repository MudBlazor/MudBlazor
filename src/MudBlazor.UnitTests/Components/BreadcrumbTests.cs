using Bunit;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BreadcrumbTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddMudBlazorServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void MudBreadcrumbs_ShouldRenderItemsWithSeparators()
        {
            var comp = ctx.RenderComponent<MudBreadcrumbs>(Parameter("Items", new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Link 1", "link1"),
                new BreadcrumbItem("Link 2", "link2"),
                new BreadcrumbItem("Link 3", "link3", disabled: true)
            }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(3);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(2);
        }

        [Test]
        public void MudBreadcrumbs_ShouldCollapseWhenMaxItemsIsReached()
        {
            var comp = ctx.RenderComponent<MudBreadcrumbs>(Parameter("MaxItems", (byte)5), Parameter("Items", new List<BreadcrumbItem>
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
    }
}

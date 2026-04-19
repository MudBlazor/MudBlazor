using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BreadcrumbTests : BunitTest
    {
        [Test]
        public void MudBreadcrumbs_ShouldNotRenderWithoutItems()
        {
            var comp = Context.Render<MudBreadcrumbs>();

            comp.FindAll("nav").Should().BeEmpty();
            comp.FindAll("ol.mud-breadcrumbs").Should().BeEmpty();
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderItemsWithSeparators()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters.Add(x => x.Items, new List<BreadcrumbItem>
            {
                new("Link 1", "link1"),
                new("Link 2", "link2"),
                new("Link 3", "link3", disabled: true)
            }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(3);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(2);
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderItemsWithIcons()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters.Add(x => x.Items, new List<BreadcrumbItem>
            {
                new("Link 1", "link1", icon: Icons.Material.Filled.Home),
                new("Link 2", "link2", icon: Icons.Material.Filled.List),
                new("Link 3", "link3", disabled: true, icon: Icons.Material.Filled.Create)
            }));

            comp.FindAll("li>a>svg").Should().HaveCount(3);
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderNavWithBreadcrumbAriaLabel()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters.Add(x => x.Items, new List<BreadcrumbItem>
            {
                new("Link 1", "link1"),
                new("Link 2", "link2")
            }));

            comp.Find("nav").GetAttribute("aria-label").Should().Be("Breadcrumb");
        }

        [Test]
        public void MudBreadcrumbs_ShouldNotCollapseWhenItemCountEqualsMaxItems()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.MaxItems, (byte)4)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3"),
                    new("Link 4", "link4")
                }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(4);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(3);
            comp.FindAll("li.mud-breadcrumbs-expander").Should().BeEmpty();
        }

        [Test]
        public void MudBreadcrumbs_ShouldCollapseWhenMaxItemsIsReached()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.MaxItems, (byte)4)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3"),
                    new("Link 4", "link4"),
                    new("Link 5", "link5", disabled: true)
                }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(2);
            comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(2);
            comp.Find("li.mud-breadcrumbs-expander").Should().NotBeNull();
        }

        [Test]
        public void MudBreadcrumbs_ShouldUseSeparatorTemplateWhenProvided()
        {
            RenderFragment separatorTemplate = builder => builder.AddMarkupContent(0, "<span class=\"test-separator\">|</span>");

            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.Separator, ">")
                .Add(x => x.SeparatorTemplate, separatorTemplate)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3")
                }));

            comp.FindAll("li.mud-breadcrumb-separator .test-separator").Should().HaveCount(2);
            comp.FindAll("li.mud-breadcrumb-separator span")
                .Should()
                .OnlyContain(x => x.ClassList.Contains("test-separator"));
        }

        [Test]
        public void MudBreadcrumbs_ShouldUseItemTemplateWhenProvided()
        {
            RenderFragment<BreadcrumbItem> itemTemplate = item => builder =>
                builder.AddMarkupContent(0, $"<button class=\"test-item-template\">{item.Text}</button>");

            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.ItemTemplate, itemTemplate)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", null, disabled: true)
                }));

            comp.FindAll("button.test-item-template").Should().HaveCount(3);
            comp.FindAll("li.mud-breadcrumb-item > a").Should().BeEmpty();
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderHashHrefWhenItemHrefIsNull()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters.Add(x => x.Items, new List<BreadcrumbItem>
            {
                new("Link 1", null)
            }));

            comp.Find("li.mud-breadcrumb-item > a").GetAttribute("href").Should().Be("#");
        }

        [Test]
        public async Task MudBreadcrumbs_ShouldExpandAfterExpanderClick()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.MaxItems, (byte)4)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3"),
                    new("Link 4", "link4"),
                    new("Link 5", "link5", disabled: true)
                }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(2);
            await comp.Find("li.mud-breadcrumbs-expander").ClickAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(5);
                comp.FindAll("li.mud-breadcrumb-separator").Should().HaveCount(4);
                comp.FindAll("li.mud-breadcrumbs-expander").Should().BeEmpty();
            });
        }

        [Test]
        public async Task MudBreadcrumbs_Other()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.MaxItems, (byte)4)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3"),
                    new("Link 4", "link4"),
                    new("Link 5", "link5", disabled: true)
                }));

            await comp.WaitForAssertionAsync(() => comp.Instance.Collapsed.Should().BeTrue());
            await comp.InvokeAsync(() => comp.Instance.Expand());
            await comp.WaitForAssertionAsync(() => comp.Instance.Collapsed.Should().BeFalse());

            await comp.InvokeAsync(() => comp.Instance.Expand());
            await comp.WaitForAssertionAsync(() => comp.Instance.Collapsed.Should().BeFalse());

            await comp.WaitForAssertionAsync(() => MudBreadcrumbs.GetItemClassname(comp.Instance.Items[1]).Should().Be("mud-breadcrumb-item"));
        }
    }
}

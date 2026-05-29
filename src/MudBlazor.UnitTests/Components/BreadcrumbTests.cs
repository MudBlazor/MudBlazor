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
        public void MudBreadcrumbs_ShouldApplyCustomClassToOrderedList()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.Class, "custom-breadcrumbs")
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2")
                }));

            var list = comp.Find("ol.mud-breadcrumbs");
            list.ClassList.Should().Contain("mud-typography-body1");
            list.ClassList.Should().Contain("custom-breadcrumbs");
        }

        [Test]
        public void MudBreadcrumbs_ShouldApplyStyleToOrderedList()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.Style, "color: red;")
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2")
                }));

            var list = comp.Find("ol.mud-breadcrumbs");
            list.GetAttribute("style").Should().Be("color: red;");
        }

        [Test]
        public void MudBreadcrumbs_ShouldApplyUserAttributesToOrderedList()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.UserAttributes, new Dictionary<string, object> { ["data-testid"] = "breadcrumbs" })
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2")
                }));

            var list = comp.Find("ol.mud-breadcrumbs");
            list.GetAttribute("data-testid").Should().Be("breadcrumbs");
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
        public void MudBreadcrumbs_ShouldRenderCustomExpanderIconWhenCollapsed()
        {
            var customExpanderIcon = Icons.Material.Filled.MoreHoriz;

            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.MaxItems, (byte)2)
                .Add(x => x.ExpanderIcon, customExpanderIcon)
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3")
                }));

            comp.Markup.Should().Contain(customExpanderIcon);
        }

        [Test]
        public void MudBreadcrumbs_ShouldRenderCustomSeparatorTextWhenNoTemplateProvided()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters
                .Add(x => x.Separator, ">")
                .Add(x => x.Items, new List<BreadcrumbItem>
                {
                    new("Link 1", "link1"),
                    new("Link 2", "link2"),
                    new("Link 3", "link3")
                }));

            comp.FindAll("li.mud-breadcrumb-separator span")
                .Should()
                .OnlyContain(x => x.TextContent == ">");
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
        public void MudBreadcrumbs_ShouldRenderDisabledItemsWithDisabledClass()
        {
            var comp = Context.Render<MudBreadcrumbs>(parameters => parameters.Add(x => x.Items, new List<BreadcrumbItem>
            {
                new("Link 1", "link1"),
                new("Link 2", "link2", disabled: true)
            }));

            comp.FindAll("li.mud-breadcrumb-item").Should().HaveCount(2);
            comp.FindAll("li.mud-breadcrumb-item.mud-disabled").Should().ContainSingle();
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
        public void BreadcrumbLink_ShouldRenderWithoutParentOrItem()
        {
            var comp = Context.Render<BreadcrumbLink>();

            comp.Find("li.mud-breadcrumb-item > a").GetAttribute("href").Should().Be("#");
            comp.Find("li.mud-breadcrumb-item").TextContent.Should().BeEmpty();
            comp.FindAll("svg").Should().BeEmpty();
        }

        [Test]
        public void BreadcrumbSeparator_ShouldRenderWithoutParent()
        {
            var comp = Context.Render<BreadcrumbSeparator>();

            comp.Find("li.mud-breadcrumb-separator > span").TextContent.Should().BeEmpty();
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

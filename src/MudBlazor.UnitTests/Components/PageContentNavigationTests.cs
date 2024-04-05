

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Interfaces;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PageContentNavigationTests : BunitTest
    {
        public override void Setup()
        {
            base.Setup();
            Context.Services.Add(new ServiceDescriptor(typeof(IScrollSpyFactory), new MockScrollSpyFactory()));

        }

        [Test]
        public void DefaultValues()
        {
            var comp = Context.RenderComponent<MudPageContentNavigation>();

            comp.Instance.ActiveSection.Should().BeNull();
            comp.Instance.Sections.Should().BeEmpty();
            comp.Instance.Headline.Should().Be("Contents");
            comp.Instance.SectionClassSelector.Should().BeNullOrEmpty();

            comp.Nodes.Should().ContainSingle();
            comp.Nodes[0].ChildNodes.Should().BeEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task AddSection(bool withUpdate)
        {
            var comp = Context.RenderComponent<MudPageContentNavigation>();

            var section1 = new MudPageContentSection("my section", "my-id");
            var section2 = new MudPageContentSection("my section 2", "my-id-2");

            if (withUpdate)
            {
                await comp.InvokeAsync(() => comp.Instance.AddSection(section1, true));
                await comp.InvokeAsync(() => comp.Instance.AddSection(section2, true));
            }
            else
            {
                comp.Instance.AddSection(section1, false);
                comp.Instance.AddSection(section2, false);
                await comp.InvokeAsync(() => ((IMudStateHasChanged)comp.Instance).StateHasChanged());
            }

            comp.RenderCount.Should().Be(withUpdate ? 3 : 2);

            comp.Instance.ActiveSection.Should().BeNull();
            comp.Instance.Sections.Should().BeEquivalentTo(new[] { section1, section2 });

            comp.Nodes.Should().ContainSingle();

            var navLinks = comp.FindComponents<MudNavLink>();
            navLinks.Should().HaveCount(2);
            navLinks[0].Instance.Class.Should().Be("page-content-navigation-navlink navigation-level-0");

            var firstLinkText = navLinks[0].Find(".mud-nav-link-text");
            firstLinkText.TextContent.Should().Be("my section");

            var secondLinkText = navLinks[1].Find(".mud-nav-link-text");
            secondLinkText.TextContent.Should().Be("my section 2");
        }

        [Test]
        public async Task AddSection_WhichIsActiveByScrollSpy()
        {
            var mockedScrollSpy = new MockScrollSpy
            {
                CenteredSection = "my-id"
            };

            var factory = new MockScrollSpyFactory(mockedScrollSpy);
            Context.Services.Add(new ServiceDescriptor(typeof(IScrollSpyFactory), factory));

            var comp = Context.RenderComponent<MudPageContentNavigation>();

            var section1 = new MudPageContentSection("my section", "my-id");
            var section2 = new MudPageContentSection("different section", "my-id-2");

            comp.Instance.AddSection(section1, false);
            comp.Instance.AddSection(section2, false);

            await comp.InvokeAsync(() => ((IMudStateHasChanged)comp.Instance).StateHasChanged());

            comp.RenderCount.Should().Be(2);

            comp.Instance.ActiveSection.Should().Be(section1);
            comp.Instance.Sections.Should().BeEquivalentTo(new[] { section1, section2 });
            comp.Nodes.Should().ContainSingle();

            var navLinks = comp.FindComponents<MudNavLink>();
            navLinks.Should().HaveCount(2);
            navLinks[0].Instance.Class.Should().Be("page-content-navigation-navlink active navigation-level-0");
            var linkText = navLinks[0].Find(".mud-nav-link-text");
            linkText.TextContent.Should().Be("my section");

            navLinks[1].Instance.Class.Should().NotContain("active");
        }

        [Test]
        public async Task AddSection_ActivateDefault()
        {
            var mockedScrollSpy = new MockScrollSpy();

            var factory = new MockScrollSpyFactory(mockedScrollSpy);
            Context.Services.Add(new ServiceDescriptor(typeof(IScrollSpyFactory), factory));

            var comp = Context.RenderComponent<MudPageContentNavigation>(p => p.Add(x => x.ActivateFirstSectionAsDefault, true));

            var section1 = new MudPageContentSection("my section", "my-id");
            var section2 = new MudPageContentSection("my section 2", "my-id-2");

            await comp.InvokeAsync(() => comp.Instance.AddSection(section1, true));
            await comp.InvokeAsync(() => comp.Instance.AddSection(section2, true));

            comp.RenderCount.Should().Be(3);

            comp.Instance.ActiveSection.Should().Be(section1);
            comp.Instance.Sections.Should().BeEquivalentTo(new[] { section1, section2 });
            comp.Nodes.Should().ContainSingle();

            var navLinks = comp.FindComponents<MudNavLink>();
            navLinks.Should().HaveCount(2);
            navLinks[0].Instance.Class.Should().Be("page-content-navigation-navlink active navigation-level-0");
            var linkText = navLinks[0].Find(".mud-nav-link-text");

            linkText.TextContent.Should().Be("my section");

            navLinks[1].Instance.Class.Should().NotContain("active");

            mockedScrollSpy.ScrollHistory.Should().BeEquivalentTo("my-id");
        }

        [Test]
        public void NavigateBySections()
        {
            var spyMock = new MockScrollSpy();

            var factory = new MockScrollSpyFactory(spyMock);
            Context.Services.Add(new ServiceDescriptor(typeof(IScrollSpyFactory), factory));

            var comp = Context.RenderComponent<MudPageContentNavigation>();

            var section1 = new MudPageContentSection("my first section", "my-id1");
            var section2 = new MudPageContentSection("my second section", "my-id2");
            var section3 = new MudPageContentSection("my third section", "my-id3");
            var sections = new[] { section1, section2, section3 };

            comp.Instance.AddSection(section1, false);
            comp.Instance.AddSection(section2, false);
            comp.Instance.AddSection(section3, false);

            comp.InvokeAsync(() => ((IMudStateHasChanged)comp.Instance).StateHasChanged());

            for (var i = 0; i < 3; i++)
            {
                var navLinks = comp.FindComponents<MudNavLink>();
                navLinks[i].Find(".mud-nav-link").Click();

                comp.Instance.ActiveSection.Should().Be(sections[i]);
                navLinks = comp.FindComponents<MudNavLink>();

                for (var j = 0; j < 3; j++)
                {
                    navLinks[j].Instance.Class.Should().Be(i == j ? "page-content-navigation-navlink active navigation-level-0" : "page-content-navigation-navlink navigation-level-0");
                }
            }

            spyMock.ScrollHistory.Should().BeEquivalentTo("my-id1", "my-id2", "my-id3");
        }

        [Test]
        public async Task ObserverScrollAndHandlingEvent()
        {
            var spyMock = new MockScrollSpy();

            var factory = new MockScrollSpyFactory(spyMock);
            Context.Services.Add(new ServiceDescriptor(typeof(IScrollSpyFactory), factory));

            var comp = Context.RenderComponent<MudPageContentNavigation>(x => x.Add(y => y.SectionClassSelector, "my-section-class"));

            spyMock.SpyingInitiated.Should().BeTrue();
            spyMock.SpyingClassSelector.Should().Be("my-section-class");

            var section1 = new MudPageContentSection("my first section", "my-id1");
            var section2 = new MudPageContentSection("my second section", "my-id2");
            var section3 = new MudPageContentSection("my third section", "my-id3");

            var sections = new[] { section1, section2, section3 };

            comp.Instance.AddSection(section1, false);
            comp.Instance.AddSection(section2, false);
            comp.Instance.AddSection(section3, false);

            await comp.InvokeAsync(() => ((IMudStateHasChanged)comp.Instance).StateHasChanged());

            //active second section
            await comp.InvokeAsync(() => spyMock.FireScrollSectionSectionCenteredEvent(section2.Id));
            comp.Instance.ActiveSection.Should().Be(section2);

            //active third section
            await comp.InvokeAsync(() => spyMock.FireScrollSectionSectionCenteredEvent(section3.Id));
            comp.Instance.ActiveSection.Should().Be(section3);

            //active first section
            await comp.InvokeAsync(() => spyMock.FireScrollSectionSectionCenteredEvent(section1.Id));
            comp.Instance.ActiveSection.Should().Be(section1);

            //active non existing section
            await comp.InvokeAsync(() => spyMock.FireScrollSectionSectionCenteredEvent("non-existing-section"));
            comp.Instance.ActiveSection.Should().Be(section1);

            //active empty section
            await comp.InvokeAsync(() => spyMock.FireScrollSectionSectionCenteredEvent(null));
            comp.Instance.ActiveSection.Should().Be(section1);
        }

        [Test]
        public async Task HideContentIfOnlyOneSectionIsAdded()
        {
            var comp = Context.RenderComponent<MudPageContentNavigation>();

            var section = new MudPageContentSection("my section", "my-id");

            await comp.InvokeAsync(() => comp.Instance.AddSection(section, true));

            comp.RenderCount.Should().Be(2);

            comp.Instance.Sections.Should().BeEquivalentTo(new[] { section });
            comp.Nodes.Should().ContainSingle();

            var navLinks = comp.FindComponents<MudNavLink>();
            navLinks.Should().BeEmpty();
        }
    }
}

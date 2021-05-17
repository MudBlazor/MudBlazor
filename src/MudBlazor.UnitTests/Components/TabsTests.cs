#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class TabsTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public async Task AddingAndRemovingTabPanels()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<TabsAddingRemovingTabsTest>();
            Console.WriteLine(comp.Markup);
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();
            // add a panel
            comp.FindAll("button")[0].Click();
            Console.WriteLine("\n" + comp.Markup);
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().NotBeEmpty();
            comp.FindAll("div.mud-tab").Count.Should().Be(1);
            comp.FindAll("p.mud-typography").Count.Should().Be(1);
            // add another
            comp.FindAll("button")[0].Click();
            Console.WriteLine("\n" + comp.Markup);
            comp.FindAll("div.mud-tab").Count.Should().Be(2);
            comp.FindAll("p.mud-typography").Count.Should().Be(1, because: "Only the current tab panel is displayed");
            // we are now on tab 0
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 0");
            // switch to tab1
            comp.FindAll("div.mud-tab")[1].Click();
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 1");
            // remove tab1
            comp.FindAll("button")[1].Click();
            comp.FindAll("div.mud-tab").Count.Should().Be(1);
            comp.FindAll("p.mud-typography").Count.Should().Be(1);
            // we should be on tab0 again
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 0");
            // remove another
            comp.FindAll("button")[1].Click();
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();
        }

        /// <summary>
        /// When KeepPanelsAlive="true" the panels are not destroyed and recreated on tab-switch. We prove that by using a button click counter on every tab and
        /// a callback that is fired only when OnRenderAsync of the tab panel happens the first time (which outputs a message at the bottom).
        /// </summary>
        [Test]
        public async Task KeepTabsAliveTest()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<TabsKeepAliveTest>();
            Console.WriteLine(comp.Markup);
            // all panels should be evident in the markup:
            comp.FindAll("button").Count.Should().Be(3);
            // every panel should be rendered first exactly once throughout the test:
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br></p>");
            // only the first panel should be active:
            comp.FindAll("div.mud-tabs-panels > div")[0].GetAttribute("style").Should().Be("display:contents;");
            comp.FindAll("div.mud-tabs-panels > div")[1].GetAttribute("style").Should().Be("display:none;");
            comp.FindAll("div.mud-tabs-panels > div")[2].GetAttribute("style").Should().Be("display:none;");
            // click first button and show button click counters
            comp.FindAll("button")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Panel 2=0");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Panel 3=0");
            // switch to the second tab:
            comp.FindAll("div.mud-tab")[1].Click();
            // none of the panels should have had a render pass with firstRender==true, so this must be as before:
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br></p>");
            // second panel should be displayed
            comp.FindAll("div.mud-tabs-panels > div")[0].GetAttribute("style").Should().Be("display:none;");
            comp.FindAll("div.mud-tabs-panels > div")[1].GetAttribute("style").Should().Be("display:contents;");
            comp.FindAll("div.mud-tabs-panels > div")[2].GetAttribute("style").Should().Be("display:none;");
            // click second button twice and show button click counters. the click of the first button should still be evident 
            comp.FindAll("button")[1].Click();
            comp.FindAll("button")[1].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Panel 2=2");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Panel 3=0");
            // switch to the third tab:
            comp.FindAll("div.mud-tab")[2].Click();
            // second panel should be displayed
            comp.FindAll("div.mud-tabs-panels > div")[0].GetAttribute("style").Should().Be("display:none;");
            comp.FindAll("div.mud-tabs-panels > div")[1].GetAttribute("style").Should().Be("display:none;");
            comp.FindAll("div.mud-tabs-panels > div")[2].GetAttribute("style").Should().Be("display:contents;");
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Panel 2=2");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Panel 3=0");
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br></p>");
            // switch back to the first tab:
            comp.FindAll("div.mud-tab")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Panel 2=2");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Panel 3=0");
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br></p>");
            // only the first panel should be active:
            comp.FindAll("div.mud-tabs-panels > div")[0].GetAttribute("style").Should().Be("display:contents;");
            comp.FindAll("div.mud-tabs-panels > div")[1].GetAttribute("style").Should().Be("display:none;");
            comp.FindAll("div.mud-tabs-panels > div")[2].GetAttribute("style").Should().Be("display:none;");
        }

        /// <summary>
        /// When KeepPanelsAlive="true" the panels are not destroyed and recreated on tab-switch. We prove that by using a button click counter on every tab and
        /// a callback that is fired only when OnRenderAsync of the tab panel happens the first time (which outputs a message at the bottom).
        /// </summary>
        [Test]
        public async Task KeepTabs_Not_AliveTest()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<TabsKeepAliveTest>(ComponentParameter.CreateParameter("KeepPanelsAlive", false));
            Console.WriteLine(comp.Markup);
            // only one panel should be evident in the markup:
            comp.FindAll("button").Count.Should().Be(1);
            // only the first panel should be rendered first
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br></p>");
            // no child divs in div.mud-tabs-panels
            comp.FindAll("div.mud-tabs-panels > div").Count.Should().Be(0);
            // click first button and show button click counters
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=0");
            comp.FindAll("button")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            // switch to the second tab:
            comp.FindAll("div.mud-tab")[1].Click();
            // first and second panel were rendered once with firstRender==true:
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br></p>");
            // only one panel should be evident in the markup:
            comp.FindAll("button").Count.Should().Be(1);
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 2=0");
            // click the button twice
            comp.FindAll("button")[0].Click();
            comp.FindAll("button")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 2=2");
            // switch to the third tab:
            comp.FindAll("div.mud-tab")[2].Click();
            // second panel should be displayed
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 3=0");
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br></p>");
            // switch back to the first tab:
            comp.FindAll("div.mud-tab")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=0");
            comp.FindAll("button")[0].Click();
            comp.FindAll("button")[0].TrimmedText().Should().Be("Panel 1=1");
            comp.FindAll("p")[^1].MarkupMatches("<p>Panel 1<br>Panel 2<br>Panel 3<br>Panel 1<br></p>");
        }

        [Test]
        public void ScrollToItem_NoScrollingNeedded()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            for (int i = 0; i < 6; i++)
            {
                comp.Instance.SetPanelActive(i);

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");

                toolbarWrappter.Should().NotBeNull();

                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be("transform:translateX(-0px);");

                GetSliderValue(comp).Should().Be(i * 250.0);
            }
        }

        [Test]
        [TestCase(400.0, 100)]
        [TestCase(300.0, 100)]
        [TestCase(200.0, 200)]
        [TestCase(100.0, 200)]
        public void ScrollToItem_CentralizeViewAroundActiveItem(double totalSize, double expectedTranslation)
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = totalSize + 10,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(2);

            var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");

            toolbarWrappter.Should().NotBeNull();

            toolbarWrappter.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrappter.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            GetSliderValue(comp).Should().Be(2 * 100.0);
        }

        [Test]
        [TestCase(400.0, 100)]
        [TestCase(300.0, 100)]
        [TestCase(200.0, 200)]
        [TestCase(100.0, 200)]
        public void ScrollToItem_CentralizeViewAroundActiveItem_ScrollVertically(double totalSize, double expectedTranslation)
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = totalSize + 10,
                IsVertical = true,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            comp.SetParametersAndRender(p => p.Add(x => x.Position, Position.Left));
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(2);

            var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");

            toolbarWrappter.Should().NotBeNull();

            toolbarWrappter.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrappter.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateY(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            GetSliderValue(comp, "top").Should().Be(2 * 100.0);
        }

        [Test]
        public void ScrollToItem_CentralizeView_ActivateAllItems()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200 + 10,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            Dictionary<int, double> expectedTranslations = new Dictionary<int, double>
            {
                { 0, 0 },
                { 1, 100 },
                { 2, 200 },
                { 3, 300 },
                { 4, 400 },
                { 5, 400 },
            };

            for (int i = 0; i < 6; i++)
            {
                comp.Instance.SetPanelActive(i);

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");

                toolbarWrappter.Should().NotBeNull();

                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslations[i].ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().Be(i * 100.0);
            }
        }

        [Test]
        public async Task Scroll_NotEnabled_EnoughSpace()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.Should().HaveCount(2);

            foreach (var item in scrollButtons)
            {
                item.Instance.Disabled.Should().BeTrue();
            }
        }

        [Test]
        public void ScrollNext_EnabledStates()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            for (int i = 0; i < 6; i++)
            {
                comp.Instance.SetPanelActive(i);

                var shouldBeDisabled = i >= 4;

                scrollButtons.Last().Instance.Disabled.Should().Be(shouldBeDisabled);
            }
        }

        [Test]
        public void ScrollPrev_EnabledStates()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            for (int i = 5; i <= 0; i--)
            {
                comp.Instance.SetPanelActive(i);

                var shouldBeDisabled = i == 0;
                scrollButtons.First().Instance.Disabled.Should().Be(shouldBeDisabled);
            }
        }

        [Test]
        public async Task ScrollNext()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            double expectedTranslation = 0.0;

            for (int i = 0; i < 2; i++)
            {
                scrollButtons.Last().Find("button").Click();
                expectedTranslation += 200;

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().Be(0);
            }

            // clicking the button more often should change something
            for (int i = 0; i < 3; i++)
            {
                scrollButtons.Last().Find("button").Click();

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-400px);");
                GetSliderValue(comp).Should().Be(0);
            }
        }

        [Test]
        public void ScrollPrev()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            comp.Instance.SetPanelActive(5);

            double expectedTranslation = 400.0;

            for (int i = 0; i < 2; i++)
            {
                scrollButtons.First().Find("button").Click();
                expectedTranslation -= 200;

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().Be(5 * 100.0);
            }

            // clicking the button more often should change something
            for (int i = 0; i < 3; i++)
            {
                scrollButtons.First().Find("button").Click();

                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-0px);");
                GetSliderValue(comp).Should().Be(5 * 100.0);
            }
        }

        [Test]
        public void Handle_ResizeOfPanel()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().Be(1 * 100.0);

            observer.UpdateTotalPanelSize(200.0);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();
            GetSliderValue(comp).Should().Be(1 * 100.0);
        }

        [Test]
        public void Handle_ResizeOfElement()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().Be(1 * 100.0);

            observer.UpdatePanelSize(0, 200.0);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();
            GetSliderValue(comp).Should().Be(200.0);
        }

        [Test]
        public async Task Handle_Add()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(4);

            GetSliderValue(comp).Should().Be(4 * 100.0);

            await comp.Instance.AddPanel();

            GetSliderValue(comp).Should().Be(4 * 100.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            scrollButtons.Last().Instance.Disabled.Should().BeFalse();
            comp.Instance.SetPanelActive(5);
            scrollButtons.Last().Instance.Disabled.Should().BeTrue();
            comp.Instance.SetPanelActive(6);

            var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
            toolbarWrappter.Should().NotBeNull();
            toolbarWrappter.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrappter.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-400px);");
        }

        [Test]
        public async Task Handle_Remove_BeforeSelection()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(2);

            GetSliderValue(comp).Should().Be(2 * 100.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            await comp.Instance.RemovePanel(0);

            scrollButtons.First().Instance.Disabled.Should().BeTrue();

            var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
            toolbarWrappter.Should().NotBeNull();
            toolbarWrappter.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrappter.GetAttribute("style");
            styleAttr.Should().Be($"transform:translateX(-0px);");

            double sliderValue = GetSliderValue(comp);
            GetSliderValue(comp).Should().Be(1 * 100.0);
        }

        [Test]
        public async Task Handle_Remove_AfterSelection()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<ScrollableTabsTest>();
            Console.WriteLine(comp.Markup);

            comp.Instance.SetPanelActive(2);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeFalse();
            {
                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");
                styleAttr.Should().Be($"transform:translateX(-100px);");
                GetSliderValue(comp).Should().Be(2 * 100.0);
            }

            await comp.Instance.RemovePanel(5);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            {
                var toolbarWrappter = comp.Find(".mud-tabs-toolbar-wrapper");
                toolbarWrappter.Should().NotBeNull();
                toolbarWrappter.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrappter.GetAttribute("style");
                styleAttr.Should().Be($"transform:translateX(-100px);");
                GetSliderValue(comp).Should().Be(2 * 100.0);
            }
        }

        [Test]
        public async Task PanelAdd_ScrollButtonsBecomeVisible()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 250.0,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<SimplifiedScrollableTabsTest>();

            Console.WriteLine(comp.Markup);

            var buttonContainer = comp.FindAll(".mud-tabs-scroll-button");
            buttonContainer.Should().HaveCount(0);

            //add the first panel, buttons shouldn't be visible
            await comp.Instance.AddPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(0);

            //add second panel, buttons shouldn't be visible
            await comp.Instance.AddPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(0);

            //add third panel, buttons should be visible
            await comp.Instance.AddPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(2);
        }

        [Test]
        public async Task PanelRemove_ScrollButtonsBecomeInvisible()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 250.0,
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), observer));

            var comp = ctx.RenderComponent<SimplifiedScrollableTabsTest>(p => p.Add(x => x.StartAmount, 5));

            Console.WriteLine(comp.Markup);

            var buttonContainer = comp.FindAll(".mud-tabs-scroll-button");
            buttonContainer.Should().HaveCount(2);

            //remove 5th panel, buttons should be visible
            await comp.Instance.RemoveLastPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(2);

            //remove 4th panel, buttons should be visible
            await comp.Instance.RemoveLastPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(2);

            //remove 3rd panel, buttons shouldn't be visible
            await comp.Instance.RemoveLastPanel();

            buttonContainer.Refresh();
            buttonContainer.Should().HaveCount(0);
        }

        [Test]
        public async Task ActivatePanels()
        {
            var activator = new Action<IRenderedComponent<ActivateDisabledTabsTest>, ActivateDisabledTabsTest.TabBindingHelper>[] {
               (x,y) => x.Instance.ActivateTab(y.Index),
               (x,y) => x.Instance.ActivateTab(y.Panel),
               (x,y) => x.Instance.ActivateTab(y.Tag),

               (x,y) => x.Instance.ActivateTab(y.Index, false),
               (x,y) => x.Instance.ActivateTab(y.Panel, false),
               (x,y) => x.Instance.ActivateTab(y.Tag, false),
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            foreach (var invoker in activator)
            {
                for (int k = 0; k < 2; k++)
                {
                    var comp = ctx.RenderComponent<ActivateDisabledTabsTest>();

                    if (k == 0)
                    {
                        comp.Instance.ActivateAll();
                    }
                    else
                    {
                        comp.Instance.EnableTab(0);
                    }

                    Console.WriteLine(comp.Markup);

                    var panels = comp.FindAll(".test-panel-selector");
                    var activePanels = comp.FindAll(".mud-tab-active");

                    //checking expected default values
                    panels.Should().HaveCount(5);
                    activePanels.Should().HaveCount(1);
                    panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

                    for (int i = 1; i < comp.Instance.Tabs.Count; i++)
                    {
                        invoker(comp, comp.Instance.Tabs[i]);

                        panels.Refresh();
                        activePanels.Refresh();

                        if (k == 0)
                        {
                            panels[i - 1].ClassList.Contains("mud-tab-active").Should().BeFalse();
                            panels[i].ClassList.Contains("mud-tab-active").Should().BeTrue();
                        }
                        else
                        {
                            panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();
                            panels[i].ClassList.Contains("mud-disabled").Should().BeTrue();
                        }
                    }
                }
            }
        }

        [Test]
        public async Task ActivatePanels_EvenWhenDisabled()
        {
            var activator = new Action<IRenderedComponent<ActivateDisabledTabsTest>, ActivateDisabledTabsTest.TabBindingHelper>[] {
               (x,y) => x.Instance.ActivateTab(y.Index, true),
               (x,y) => x.Instance.ActivateTab(y.Panel, true),
               (x,y) => x.Instance.ActivateTab(y.Tag, true),
            };

            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            foreach (var invoker in activator)
            {
                var comp = ctx.RenderComponent<ActivateDisabledTabsTest>();

                Console.WriteLine(comp.Markup);

                var panels = comp.FindAll(".test-panel-selector");

                //checking expected default values
                panels.Should().HaveCount(5);
                panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

                for (int i = 1; i < comp.Instance.Tabs.Count; i++)
                {
                    invoker(comp, comp.Instance.Tabs[i]);

                    panels.Refresh();

                    panels[i - 1].ClassList.Contains("mud-tab-active").Should().BeFalse();
                    panels[i].ClassList.Contains("mud-tab-active").Should().BeTrue();
                    panels[i].ClassList.Contains("mud-disabled").Should().BeTrue();

                    var contentElement = comp.Find(".test-active-panel");

                    contentElement.TextContent.Should().Be(comp.Instance.Tabs[i].Content);
                }
            }
        }

        #region Helper

        private static double GetSliderValue(IRenderedComponent<ScrollableTabsTest> comp, string attribute = "left")
        {
            var slider = comp.Find(".mud-tab-slider");
            slider.HasAttribute("style").Should().Be(true);

            var styleAttribute = slider.GetAttribute("style");
            var indexToSplit = styleAttribute.IndexOf($"{attribute}:");
            var substring = styleAttribute.Substring(indexToSplit + attribute.Length + 1);
            substring = substring.Remove(substring.Length - 3);
            var value = double.Parse(substring, CultureInfo.InvariantCulture);
            return value;
        }




        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Examples;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TabsTests : BunitTest
    {
        public override void Setup()
        {
            base.Setup();
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), new MockResizeObserverFactory()));
        }

        [Test]
        public void AddingAndRemovingTabPanels()
        {
            var comp = Context.RenderComponent<TabsAddingRemovingTabsTest>();
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();
            comp.Instance.Tabs.Panels.Should().NotBeNull().And.BeEmpty();

            // add a panel
            comp.FindAll("button")[0].Click();
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().NotBeEmpty();
            comp.FindAll("div.mud-tab").Count.Should().Be(1);
            comp.FindAll("p.mud-typography").Count.Should().Be(1);

            comp.Instance.Tabs.Panels.Should().NotBeNull().And.HaveCount(1);
            comp.FindComponents<MudTabPanel>().First().Instance.Should().Be(comp.Instance.Tabs.Panels[0]);

            // add another
            comp.FindAll("button")[0].Click();
            comp.FindAll("div.mud-tab").Count.Should().Be(2);

            comp.Instance.Tabs.Panels.Should().NotBeNull().And.HaveCount(2);
            comp.FindComponents<MudTabPanel>().ElementAt(0).Instance.Should().Be(comp.Instance.Tabs.Panels[0]);
            comp.FindComponents<MudTabPanel>().ElementAt(1).Instance.Should().Be(comp.Instance.Tabs.Panels[1]);

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

            comp.Instance.Tabs.Panels.Should().NotBeNull().And.HaveCount(1);
            comp.FindComponents<MudTabPanel>().ElementAt(0).Instance.Should().Be(comp.Instance.Tabs.Panels[0]);

            // we should be on tab0 again
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 0");
            // remove another
            comp.FindAll("button")[1].Click();
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();

            comp.Instance.Tabs.Panels.Should().NotBeNull().And.BeEmpty();
        }

        /// <summary>
        /// When KeepPanelsAlive="true" the panels are not destroyed and recreated on tab-switch. We prove that by using a button click counter on every tab and
        /// a callback that is fired only when OnRenderAsync of the tab panel happens the first time (which outputs a message at the bottom).
        /// </summary>
        [Test]
        public void KeepTabsAliveTest()
        {
            var comp = Context.RenderComponent<TabsKeepAliveTest>();
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
        public void KeepTabs_Not_AliveTest()
        {
            var comp = Context.RenderComponent<TabsKeepAliveTest>(ComponentParameter.CreateParameter("KeepPanelsAlive", false));
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
        public void TabHeaderClassPropagated()
        {
            var comp = Context.RenderComponent<MudTabs>();

            comp.SetParametersAndRender(builder => builder.Add(tabs => tabs.TabHeaderClass, "testA testB"));

            comp.Find(".mud-tabs-tabbar").ClassList.Should().Contain(new[] { "testA", "testB" });
        }

        [Test]
        public void ScrollToItem_NoScrollingNeeded()
        {
            var comp = Context.RenderComponent<ScrollableTabsTest>();

            for (var i = 0; i < 6; i++)
            {
                comp.Instance.SetPanelActive(i);

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

                toolbarWrapper.Should().NotBeNull();

                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

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

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            comp.Instance.SetPanelActive(2);

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

            toolbarWrapper.Should().NotBeNull();

            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

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

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.SetParametersAndRender(p => p.Add(x => x.Position, Position.Left));

            comp.Instance.SetPanelActive(2);

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

            toolbarWrapper.Should().NotBeNull();

            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

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

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            var expectedTranslations = new Dictionary<int, double>
            {
                { 0, 0 },
                { 1, 100 },
                { 2, 200 },
                { 3, 300 },
                { 4, 400 },
                { 5, 390 },
            };

            for (var i = 0; i < 6; i++)
            {
                comp.Instance.SetPanelActive(i);

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

                toolbarWrapper.Should().NotBeNull();

                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslations[i].ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().Be(i * 100.0);
            }
        }

        [Test]
        public void Scroll_NotEnabled_EnoughSpace()
        {
            var comp = Context.RenderComponent<ScrollableTabsTest>();

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

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            for (var i = 0; i < 6; i++)
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

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            for (var i = 5; i <= 0; i--)
            {
                comp.Instance.SetPanelActive(i);

                var shouldBeDisabled = i == 0;
                scrollButtons.First().Instance.Disabled.Should().Be(shouldBeDisabled);
            }
        }

        [Test]
        public void ScrollNext()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            var expectedTranslation = 0.0;

            for (var i = 0; i < 2; i++)
            {
                scrollButtons.Last().Find("button").Click();
                expectedTranslation += observer.PanelSize;

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            comp.Instance.SetPanelActive(5);

            var expectedTranslation = 500.0;

            for (var i = 0; i < 2; i++)
            {
                scrollButtons.First().Find("button").Click();
                expectedTranslation -= observer.PanelSize;

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            comp.Instance.SetPanelActive(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().Be(1 * 100.0);

            observer.UpdateTotalPanelSize(200.0);

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().Be(1 * 100.0);
        }

        [Test]
        public void BackButtonAfterResizing_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.Instance.SetPanelActive(5);

            observer.UpdateTotalPanelSize(601.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            var expectedTranslation = 0.0;
            scrollButtons[0].Find("button").Click();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            scrollButtons[0].Instance.Disabled.Should().BeTrue();
        }

        [Test]
        public void PrevButtonOnLowWidth()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 50,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.Instance.SetPanelActive(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            scrollButtons[0].Find("button").Click();
            var expectedTranslation = 0.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            scrollButtons[0].Instance.Disabled.Should().BeTrue();
        }

        [Test]
        public void NextButtonOnLowWidth()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 50,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.Instance.SetPanelActive(4);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[1].Instance.Disabled.Should().BeFalse();

            scrollButtons[1].Find("button").Click();
            var expectedTranslation = 500.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            scrollButtons[1].Instance.Disabled.Should().BeTrue();
        }


        [Test]
        public void BackButtonAfterResizing_Without_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.AlwaysShowScrollButtons, false));
            comp.Instance.SetPanelActive(5);


            observer.UpdateTotalPanelSize(601.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            var expectedTranslation = 0.0;
            scrollButtons[0].Find("button").Click();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            comp.FindComponents<MudIconButton>().Should().HaveCount(0);
        }

        [Test]
        public void ButtonsNotVisibleAfterResizing_Without_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.AlwaysShowScrollButtons, false));
            comp.Instance.SetPanelActive(5);


            observer.UpdateTotalPanelSize(601.0);
            comp.Instance.SetPanelActive(5);

            var expectedTranslation = 0.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            comp.FindComponents<MudIconButton>().Should().HaveCount(0);
        }

        [Test]
        public void Handle_ResizeOfElement()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            comp.Instance.SetPanelActive(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().Be(1 * 100.0);

            observer.UpdatePanelSize(0, 200.0);

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

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

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            comp.Instance.SetPanelActive(2);

            GetSliderValue(comp).Should().Be(2 * 100.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            await comp.Instance.RemovePanel(0);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");
            styleAttr.Should().Be($"transform:translateX(-100px);");

            var sliderValue = GetSliderValue(comp);
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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            comp.Instance.SetPanelActive(2);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeFalse();
            {
                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");
                styleAttr.Should().Be($"transform:translateX(-100px);");
                GetSliderValue(comp).Should().Be(2 * 100.0);
            }

            await comp.Instance.RemovePanel(5);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            {
                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");
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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<SimplifiedScrollableTabsTest>();

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

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<SimplifiedScrollableTabsTest>(p => p.Add(x => x.StartAmount, 5));

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
        public void ActivatePanels()
        {
            var activator = new Action<IRenderedComponent<ActivateDisabledTabsTest>, ActivateDisabledTabsTest.TabBindingHelper>[] {
               (x,y) => x.Instance.ActivateTab(y.Index),
               (x,y) => x.Instance.ActivateTab(y.Panel),
               (x,y) => x.Instance.ActivateTab(y.Tag),

               (x,y) => x.Instance.ActivateTab(y.Index, false),
               (x,y) => x.Instance.ActivateTab(y.Panel, false),
               (x,y) => x.Instance.ActivateTab(y.Tag, false),
            };

            foreach (var invoker in activator)
            {
                for (var k = 0; k < 2; k++)
                {
                    var comp = Context.RenderComponent<ActivateDisabledTabsTest>();

                    if (k == 0)
                    {
                        comp.Instance.ActivateAll();
                    }
                    else
                    {
                        comp.Instance.EnableTab(0);
                    }

                    var panels = comp.FindAll(".test-panel-selector");
                    var activePanels = comp.FindAll(".mud-tab-active");

                    //checking expected default values
                    panels.Should().HaveCount(5);
                    activePanels.Should().HaveCount(1);
                    panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

                    for (var i = 1; i < comp.Instance.Tabs.Count; i++)
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
        public void ActivatePanels_EvenWhenDisabled()
        {
            var activator = new Action<IRenderedComponent<ActivateDisabledTabsTest>, ActivateDisabledTabsTest.TabBindingHelper>[] {
               (x,y) => x.Instance.ActivateTab(y.Index, true),
               (x,y) => x.Instance.ActivateTab(y.Panel, true),
               (x,y) => x.Instance.ActivateTab(y.Tag, true),
            };

            foreach (var invoker in activator)
            {
                var comp = Context.RenderComponent<ActivateDisabledTabsTest>();

                var panels = comp.FindAll(".test-panel-selector");

                //checking expected default values
                panels.Should().HaveCount(5);
                panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

                for (var i = 1; i < comp.Instance.Tabs.Count; i++)
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

        [Test]
        public void SelectedIndex_Binding()
        {
            //starting with index 1:
            var comp = Context.RenderComponent<SelectedIndexTabsTest>();
            comp.Instance.Tabs.ActivePanelIndex.Should().Be(1);
            var panels = comp.FindAll(".mud-tab");
            var activePanels = comp.FindAll(".mud-tab-active");
            activePanels.Should().HaveCount(1);
            panels[1].ClassList.Contains("mud-tab-active").Should().BeTrue();

            //starting with index 2:
            SelectedIndexTabsTest.SelectedTab = 2;
            comp = Context.RenderComponent<SelectedIndexTabsTest>();
            comp.Instance.Tabs.ActivePanelIndex.Should().Be(2);
            panels = comp.FindAll(".mud-tab");
            activePanels = comp.FindAll(".mud-tab-active");
            activePanels.Should().HaveCount(1);
            panels[2].ClassList.Contains("mud-tab-active").Should().BeTrue();

            //starting with index 0:
            SelectedIndexTabsTest.SelectedTab = 0;
            comp = Context.RenderComponent<SelectedIndexTabsTest>();
            comp.Instance.Tabs.ActivePanelIndex.Should().Be(0);
            panels = comp.FindAll(".mud-tab");
            activePanels = comp.FindAll(".mud-tab-active");
            activePanels.Should().HaveCount(1);
            panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

        }

        [Test]
        public void DefaultValuesForHeaders()
        {
            var tabs = new MudTabs();

            tabs.HeaderPosition.Should().Be(TabHeaderPosition.After);
            tabs.Header.Should().BeNull();

            tabs.TabPanelHeaderPosition.Should().Be(TabHeaderPosition.After);
            tabs.TabPanelHeader.Should().BeNull();

        }

        /// <summary>
        /// The header should be rendered based on the value of header position.
        /// </summary>
        [Test]
        [TestCase(TabHeaderPosition.After)]
        [TestCase(TabHeaderPosition.Before)]
        public void RenderHeaderBasedOnPosition(TabHeaderPosition position)
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.TabHeaderPosition, position));
            comp.SetParametersAndRender(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

            var headerContent = comp.Find(".test-header-content");
            headerContent.TextContent.Should().Be($"Count: {3}");

            var headerPanel = headerContent.ParentElement;
            var additionalClass = position == TabHeaderPosition.After ? "mud-tabs-header-after" : "mud-tabs-header-before";
            headerPanel.ClassList.Should().BeEquivalentTo("mud-tabs-header", additionalClass);

            var tabInnerHeader = comp.Find(".mud-tabs-tabbar-inner");

            tabInnerHeader.Children.Should().Contain(headerPanel);
            if (position == TabHeaderPosition.After)
            {
                tabInnerHeader.Children.Last().Should().Be(headerPanel);
            }
            else
            {
                tabInnerHeader.Children.First().Should().Be(headerPanel);
            }
        }

        /// <summary>
        /// If the header template is set, but the position is none, no header should be rendered
        /// </summary>
        [Test]
        public void RenderHeaderBasedOnPosition_None()
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            comp.SetParametersAndRender(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

            var headerContent = comp.FindAll(".test-header-content");
            headerContent.Should().BeEmpty();
        }

        /// <summary>
        /// The panel header header should be rendered based on the value of header position.
        /// </summary>
        [Test]
        [TestCase(TabHeaderPosition.After)]
        [TestCase(TabHeaderPosition.Before)]
        public void RenderHeaderPanelBasedOnPosition(TabHeaderPosition position)
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            comp.SetParametersAndRender(x => x.Add(y => y.TabPanelHeaderPosition, position));

            var headerContent = comp.FindAll(".test-panel-header-content");
            headerContent.Should().HaveCount(3);

            headerContent.Select(x => x.TextContent).ToList().Should().BeEquivalentTo("Index: 0", "Index: 1", "Index: 2");

            foreach (var item in headerContent)
            {
                var headerPanel = item.ParentElement;
                var additionalClass = position == TabHeaderPosition.After ? "mud-tabs-panel-header-after" : "mud-tabs-panel-header-before";

                headerPanel.ClassList.Should().BeEquivalentTo("mud-tabs-panel-header", additionalClass);

                var parent = headerPanel.ParentElement;

                if (position == TabHeaderPosition.After)
                {
                    parent.Children.Last().Should().Be(headerPanel);
                }
                else
                {
                    parent.Children.First().Should().Be(headerPanel);
                }
            }
        }

        /// <summary>
        /// If the header template is set, but the position is none, no header should be rendered
        /// </summary>
        [Test]
        public void RenderHeaderPanelBasedOnPosition_None()
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            comp.SetParametersAndRender(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

            var headerContent = comp.FindAll(".test-panel-header-content");
            headerContent.Should().BeEmpty();
        }

        [Test]
        public void TabPanelIconColorOverridesTabIconColor()
        {
            var comp = Context.RenderComponent<TabPanelIconColorTest>();
            comp.SetParametersAndRender(x => x.Add(y => y.MudTabPanelIconColor, Color.Success));

            var iconRef = comp.Find(".mud-icon-root.mud-svg-icon");
            iconRef.ClassList.Should().Contain("mud-success-text");
        }

        [Test]
        public void TabPanelIconColorOverridesTabIconColorExceptWhenDisabled()
        {
            var comp = Context.RenderComponent<TabPanelIconColorTest>();
            comp.SetParam("DisableTab", true);
            comp.SetParametersAndRender(x => x.Add(y => y.MudTabPanelIconColor, Color.Success));

            var iconRef = comp.Find(".mud-icon-root.mud-svg-icon");
            iconRef.ClassList.Should().NotContain("mud-success-text");
        }

        [Test]
        public void HtmlTextTabs()
        {
            // get the tab panels, we must have 2 tabs, one with html text and one without
            var comp = Context.RenderComponent<HtmlTextTabsTest>();
            var panels = comp.FindAll(".mud-tab");
            panels.Should().HaveCount(2);

            // index 0 : html text "Hello <span>World</span>!"
            panels[0].InnerHtml.Should().Be("Hello &lt;span&gt;World&lt;/span&gt;!");
            panels[0].TextContent.Should().Be("Hello <span>World</span>!");

            // index 1 : simple text without html "Hello World!"
            panels[1].InnerHtml.Contains("Hello World!").Should().BeTrue();
            panels[1].TextContent.Contains("Hello World!").Should().BeTrue();
        }

        /// <summary>
        ///  Depending on the SliderAnimation parameter, it should toggle the transition style attribute
        /// </summary>
        [Test]
        public void ToggleTabsSliderAnimation()
        {
            //The first tab should be active because for the rest the slider position is calculated by JS
            //and before the calculation the slider is hidden to avoid movement on first load
            var comp = Context.RenderComponent<ToggleTabsSlideAnimationTest>(p => p.Add(x => x.SelectedTab, 0));

            //Set SliderAnimation to true
            //Check if style attr does not contain transform: none
            comp.Instance.SliderAnimation = true;
            comp.Render();
            comp.Find(".mud-tab-slider").GetAttribute("style").Contains("transition:none").Should().BeFalse();

            //Set SliderAnimation to false
            //Check if style attr contains transform: none
            comp.Instance.SliderAnimation = false;
            comp.Render();
            comp.Find(".mud-tab-slider").GetAttribute("style").Contains("transition:none").Should().BeTrue();

        }

        /// <summary>
        ///  Specifying a custom minimum width should add a min-width style to each tab
        /// </summary>
        [Test]
        public void MinimumTabWidth()
        {
            var comp = Context.RenderComponent<MinimumWidthTabs>();

            //Check if style respects minimum width from test
            comp.Find(".mud-tab").GetAttribute("style").Contains("min-width").Should().BeTrue();
            comp.Find(".mud-tab").GetAttribute("style").Contains("20px").Should().BeTrue();

        }

        /// <summary>
        /// See: https://github.com/MudBlazor/MudBlazor/issues/2976
        /// </summary>
        [Test]
        public void MenuInHeaderPanelCloseOnClickOutside()
        {
            var comp = Context.RenderComponent<TabsWithMenuInHeader>();

            //open the menu
            comp.Find("button").Click();

            // make sure the menu is rendered
            _ = comp.Find(".my-menu-item-1");

            //click the overlay to force a close
            comp.Find(".mud-overlay").Click();

            //no menu item should be visible anymore
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".my-menu-item-1"));
        }

        [Test]
        public void PrePanelContent()
        {
            var comp = Context.RenderComponent<TabsWithPrePanelContent>(p => p.Add(x => x.SelectedIndex, 0));

            var content = comp.Find(".pre-panel-content-custom");

            content.TextContent.Should().Be("Selected: Tab One");

            content.PreviousElementSibling.ClassList.Should().Contain("mud-tabs-tabbar");
            content.NextElementSibling.ClassList.Should().Contain("mud-tabs-panels");

            comp.SetParametersAndRender(p => p.Add(x => x.SelectedIndex, 1));

            content = comp.Find(".pre-panel-content-custom");

            content.TextContent.Should().Be("Selected: Tab Two");

            content.PreviousElementSibling.ClassList.Should().Contain("mud-tabs-tabbar");
            content.NextElementSibling.ClassList.Should().Contain("mud-tabs-panels");
        }

        [Test]
        public void CancelPanelActivation()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = Context.RenderComponent<CancelActivationTabsTest>();
            comp.SetParametersAndRender(p => p.Add(x => x.Position, Position.Left));

            comp.Instance.SetPanelActive(2);
            comp.Instance.ActivePanel.Should().NotBe(2);
        }

        #region Helper

        private static double GetSliderValue(IRenderedComponent<ScrollableTabsTest> comp, string attribute = "left")
        {
            var slider = comp.Find(".mud-tab-slider");
            slider.HasAttribute("style").Should().Be(true);

            var styleAttribute = slider.GetAttribute("style");
            var indexToSplit = styleAttribute.IndexOf($"{attribute}:");
            var substring = styleAttribute.Substring(indexToSplit + attribute.Length + 1).Split(';')[0];
            substring = substring.Remove(substring.Length - 2);
            var value = double.Parse(substring, CultureInfo.InvariantCulture);

            return value;
        }

        #endregion


        [Test]
        public void DynamicTabs_CollectionRenderSyncTest()
        {
            var comp = Context.RenderComponent<DynamicTabsSimpleExample>();

            var userTabs = comp.Instance.UserTabs;
            var mudTabs = comp.Instance.DynamicTabs;

            // Initial
            userTabs.Count.Should().Be(3);
            mudTabs.Panels.Count.Should().Be(3);

            // Remove
            comp.Instance.RemoveTab(userTabs.Last().Id);
            userTabs.Count.Should().Be(2);
            comp.Render(); // Render to refresh MudTabs
            mudTabs.Panels.Count.Should().Be(2);

            // Add
            comp.Instance.AddTab(Guid.NewGuid());
            userTabs.Count.Should().Be(3);
            comp.Render(); // Render to refresh MudTabs
            mudTabs.Panels.Count.Should().Be(3);

            // Remove all, no ArgumentOutOfRangeException should be thrown.
            comp.Instance.RemoveTab(userTabs.Last().Id);
            comp.Instance.RemoveTab(userTabs.Last().Id);
            comp.Instance.RemoveTab(userTabs.Last().Id);
            userTabs.Count.Should().Be(0);
            comp.Render(); // Render to refresh MudTabs.
            mudTabs.Panels.Count.Should().Be(0);

            // No active panel.
            mudTabs.ActivePanel.Should().BeNull();

            // No active panel means no active panel index.
            comp.Instance.UserIndex.Should().Be(-1);
            mudTabs.ActivePanelIndex.Should().Be(-1);
        }


        [Test]
        public void TabPanel_ShowCloseIconTest()
        {
            var comp = Context.RenderComponent<DynamicTabsSimpleExample>();
            var tabs = comp.FindAll("div.mud-tab");
            tabs[0].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeTrue();
            tabs[1].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeFalse(); // The close icon is not shown.
            tabs[2].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeTrue();
        }

        [Test]
        public void Tabs_HaveRipple_WhenRippleIsTrue()
        {
            var comp = Context.RenderComponent<TabsRippleTest>(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Ripple, false));
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);
        }
    }
}

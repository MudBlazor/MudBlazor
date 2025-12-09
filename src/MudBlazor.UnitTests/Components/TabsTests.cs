using System.Globalization;
using System.Reflection;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents.Tabs;
using MudBlazor.UnitTests.TestComponents.Tabs.KeepTabsAlive;
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
            comp.FindAll("div.mud-tabs-panels > div").Count.Should().Be(1);
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
        public async Task TabHeaderClassPropagated()
        {
            var comp = Context.RenderComponent<MudTabs>();

            await comp.SetParametersAndRenderAsync(builder => builder.Add(tabs => tabs.TabHeaderClass, "testA testB"));

            comp.Find(".mud-tabs-tabbar").ClassList.Should().Contain(new[] { "testA", "testB" });
        }

        [Test]
        public async Task ScrollToItem_NoScrollingNeeded()
        {
            var comp = Context.RenderComponent<ScrollableTabsTest>();

            for (var i = 0; i < 6; i++)
            {
                await comp.Instance.SetPanelActiveAsync(i);

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

                toolbarWrapper.Should().NotBeNull();

                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be("transform:translateX(-0px);");

                GetSliderValue(comp).Should().BeApproximately(i * (1.0 / 6.0) * 100, 0.01);
            }
        }

        [Test]
        public void ScrollToItem_BeforeRender()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 110.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.AddTransient<IResizeObserverFactory>(_ => factory);

            var comp = Context.RenderComponent<ScrollableTabsRenderTest>();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            var tabs = comp.FindAll(".mud-tab");

            toolbarWrapper.Should().NotBeNull();
            tabs.Count.Should().Be(11);
            // Tab index starts from zero
            tabs[8].ClassList.Should().Contain("mud-tab-active");

            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");
            // center tab the leftover is 10 so must adjust by 5 (-800 to -795)
            styleAttr.Should().Be("transform:translateX(-795px);");
        }

        [Test]
        [TestCase(400.0, 50)] // centered tab
        [TestCase(300.0, 100)]
        [TestCase(200.0, 150)] // centered tab
        [TestCase(100.0, 200)] // centered tab
        public async Task ScrollToItem_CentralizeViewAroundActiveItem(double totalSize, double expectedTranslation)
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = totalSize,
            };

            var factory = new MockResizeObserverFactory(observer);

            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            await comp.Instance.SetPanelActiveAsync(2);

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

            toolbarWrapper.Should().NotBeNull();

            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            GetSliderValue(comp).Should().BeApproximately((2.0 / 6.0) * 100.0, 0.01);
        }

        [Test]
        [TestCase(400.0, 45)]
        [TestCase(300.0, 95)] // formula beside edges is presize - half viewport - half panel
        [TestCase(200.0, 145)] // selected panel 2 so 2 presize panels (0 and 1) or 200
        [TestCase(100.0, 195)] // 200 - (110 / 2 = 55) + (100 / 2 = 50) = 195
        public async Task ScrollToItem_CentralizeViewAroundActiveItem_ScrollVertically(double totalSize, double expectedTranslation)
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
            await comp.Instance.ChangePositionAsync(true);

            await comp.Instance.SetPanelActiveAsync(2);

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

            toolbarWrapper.Should().NotBeNull();

            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateY(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            GetSliderValue(comp, "top").Should().BeApproximately((2.0 / 6.0) * 100.0, 0.01);
        }

        [Test]
        public async Task ScrollToItem_CentralizeView_ActivateAllItems()
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
                { 0, 0 },  // preSize (tabs before selected) - viewportCenter + panelCenter
                { 1, 45 }, // 2 tab size so center tab 100 - (210 / 2) + (100 / 2)
                { 2, 145 }, // 200 - (210 / 2) + (100 / 2)
                { 3, 245 }, // 300 - (210 / 2 = 105) + (100 / 2 = 50)  = 245
                { 4, 345 },
                { 5, 390 }, // end caps snap last tab to edge so maxScroll is all tabs (600) - viewport (210)
            };

            for (var i = 0; i < 6; i++)
            {
                await comp.Instance.SetPanelActiveAsync(i);

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");

                toolbarWrapper.Should().NotBeNull();

                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslations[i].ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().BeApproximately((i / 6.0) * 100.0, 0.01);
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
        public async Task ScrollNext_EnabledStates()
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
                await comp.Instance.SetPanelActiveAsync(i);

                var shouldBeDisabled = i == 5; // in a two tab showing only the last tab disables next

                scrollButtons.Last().Instance.Disabled.Should().Be(shouldBeDisabled);
            }
        }

        [Test]
        public async Task ScrollPrev_EnabledStates()
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

            for (var i = 5; i >= 0; i--)
            {
                await comp.Instance.SetPanelActiveAsync(i);

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
                expectedTranslation += observer.PanelSize * 2;

                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().Be(0);
            }
        }

        [Test]
        public async Task ScrollPrev()
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

            await comp.Instance.SetPanelActiveAsync(5);

            var expectedTranslation = 400.0;

            for (var i = 0; i < 2; i++)
            {
                scrollButtons.First().Find("button").Click(); // prev click
                expectedTranslation -= observer.PanelSize * 2; // scroll one page back (2 tabs)
                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");

                styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
                GetSliderValue(comp).Should().BeApproximately((5.0 / 6.0) * 100.0, 0.01);
            }
        }

        [Test]
        public async Task Handle_ResizeOfPanel()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            await comp.Instance.SetPanelActiveAsync(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().BeApproximately((1.0 / 6.0) * 100.0, 0.01);

            observer.UpdateTotalPanelSize(200.0);

            scrollButtons.First().Instance.Disabled.Should().BeFalse(); // fits 2 tabs, on the 2nd tab centered so both show
            GetSliderValue(comp).Should().BeApproximately((1.0 / 6.0) * 100.0, 0.01);
        }

        [Test]
        public async Task BackButtonAfterResizing_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            await comp.Instance.SetPanelActiveAsync(5);

            observer.UpdateTotalPanelSize(501.0);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            var expectedTranslation = 0.0;
            scrollButtons[0].Find("button").Click();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            scrollButtons[0].Instance.Disabled.Should().BeTrue(); // left scroll
            scrollButtons[1].Instance.Disabled.Should().BeFalse(); // right scroll
        }

        [Test]
        public async Task PrevButtonOnLowWidth()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 50,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            await comp.Instance.SetPanelActiveAsync(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            scrollButtons[0].Find("button").Click();
            var expectedTranslation = 25.0; // 25 px centers the first tab

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            scrollButtons[0].Instance.Disabled.Should().BeFalse();
            // scroll buttons are never disabled when width is too low due to tab centering
        }

        [Test]
        public async Task NextButtonOnLowWidth()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 50,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            await comp.Instance.SetPanelActiveAsync(4);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[1].Instance.Disabled.Should().BeFalse();

            scrollButtons[1].Find("button").Click();
            var expectedTranslation = 500.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            // scroll buttons are never disabled when width is too low due to tab centering
            scrollButtons[1].Instance.Disabled.Should().BeFalse();
        }


        [Test]
        public async Task BackButtonAfterResizing_Without_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.AlwaysShowScrollButtons, false));
            await comp.Instance.SetPanelActiveAsync(5);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons[0].Instance.Disabled.Should().BeFalse();

            // 6 panels will not show any scroll buttons in this scenario
            observer.UpdateTotalPanelSize(601.0);

            var expectedTranslation = 0.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            comp.FindComponents<MudIconButton>().Should().HaveCount(0);
        }

        [Test]
        public async Task ButtonsNotVisibleAfterResizing_Without_AlwaysShowScrollButtons()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 200.0,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.AlwaysShowScrollButtons, false));
            await comp.Instance.SetPanelActiveAsync(5);

            observer.UpdateTotalPanelSize(601.0);
            await comp.Instance.SetPanelActiveAsync(5);

            var expectedTranslation = 0.0;

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");

            styleAttr.Should().Be($"transform:translateX(-{expectedTranslation.ToString(CultureInfo.InvariantCulture)}px);");
            comp.FindComponents<MudIconButton>().Should().HaveCount(0);
        }

        [Test]
        public async Task Handle_ResizeOfElement()
        {
            var observer = new MockResizeObserver
            {
                PanelSize = 100.0,
                PanelTotalSize = 300,
            };

            var factory = new MockResizeObserverFactory(observer);
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), factory));

            var comp = Context.RenderComponent<ScrollableTabsTest>();

            await comp.Instance.SetPanelActiveAsync(1);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            GetSliderValue(comp).Should().BeApproximately((1.0 / 6.0) * 100.0, 0.01);

            observer.UpdatePanelSize(0, 200.0); // updates tab size not panel size

            scrollButtons.First().Instance.Disabled.Should().BeTrue();
            // 1/6 of the tabs is the exact center of the slider 
            GetSliderValue(comp).Should().BeApproximately((1.0 / 6.0) * 100.0, 0.01);
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

            await comp.Instance.SetPanelActiveAsync(4);

            comp.WaitForAssertion(() =>
                GetSliderValue(comp).Should().BeApproximately((4.0 / 6.0) * 100.0, 0.01));

            await comp.Instance.AddPanelAsync();

            comp.WaitForAssertion(() =>
                GetSliderValue(comp).Should().BeApproximately((4.0 / 7.0) * 100.0, 0.01));

            var scrollButtons = comp.FindComponents<MudIconButton>();
            scrollButtons.Should().HaveCount(2);

            scrollButtons.Last().Instance.Disabled.Should().BeFalse();
            await comp.Instance.SetPanelActiveAsync(5);
            scrollButtons.Last().Instance.Disabled.Should().BeTrue();
            await comp.Instance.SetPanelActiveAsync(6);

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

            await comp.Instance.SetPanelActiveAsync(2);

            GetSliderValue(comp).Should().BeApproximately((2.0 / 6.0) * 100.0, 0.01);

            var scrollButtons = comp.FindComponents<MudIconButton>();
            // panels 2, 3, 4 should be shown since panel 3 is selected
            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            await comp.Instance.RemovePanelAsync(0);
            // panels 1, 2, 3 should be shown since panel 2 is selected (old panel 3)
            // no scroll bar since 2 is centered puts 1 at left
            scrollButtons.First().Instance.Disabled.Should().BeTrue();

            var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
            toolbarWrapper.Should().NotBeNull();
            toolbarWrapper.HasAttribute("style").Should().Be(true);
            var styleAttr = toolbarWrapper.GetAttribute("style");
            styleAttr.Should().Be($"transform:translateX(-0px);");

            var sliderValue = GetSliderValue(comp);
            sliderValue.Should().BeApproximately((1.0 / 5.0) * 100.0, 0.00001);
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

            await comp.Instance.SetPanelActiveAsync(2);

            var scrollButtons = comp.FindComponents<MudIconButton>();

            scrollButtons.First().Instance.Disabled.Should().BeFalse();
            {
                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");
                styleAttr.Should().Be($"transform:translateX(-100px);");
                GetSliderValue(comp).Should().BeApproximately((2.0 / 6.0) * 100.0, 0.01);
            }

            await comp.Instance.RemovePanelAsync(5);

            scrollButtons.First().Instance.Disabled.Should().BeFalse();

            {
                var toolbarWrapper = comp.Find(".mud-tabs-tabbar-wrapper");
                toolbarWrapper.Should().NotBeNull();
                toolbarWrapper.HasAttribute("style").Should().Be(true);
                var styleAttr = toolbarWrapper.GetAttribute("style");
                styleAttr.Should().Be($"transform:translateX(-100px);");
                GetSliderValue(comp).Should().BeApproximately((2.0 / 5.0) * 100.0, 0.00001);
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
        public async Task ActivatePanels()
        {
            var activator = new Func<IRenderedComponent<ActivateDisabledTabsTest>, ActivateDisabledTabsTest.TabBindingHelper, Task>[] {
                (x,y) => x.Instance.ActivateTabAsync(y.Index),
                (x,y) => x.Instance.ActivateTabAsync(y.Panel),
                (x,y) => x.Instance.ActivateTabAsync(y.Tag),

                (x,y) => x.Instance.ActivateTabAsync(y.Index, false),
                (x,y) => x.Instance.ActivateTabAsync(y.Panel, false),
                (x,y) => x.Instance.ActivateTabAsync(y.Tag, false),
            };

            foreach (var invoker in activator)
            {
                for (var k = 0; k < 2; k++)
                {
                    var comp = Context.RenderComponent<ActivateDisabledTabsTest>();

                    if (k == 0)
                    {
                        await comp.Instance.ActivateAllAsync();
                    }
                    else
                    {
                        await comp.Instance.EnableTabAsync(0);
                    }

                    var panels = comp.FindAll(".test-panel-selector");
                    var activePanels = comp.FindAll(".mud-tab-active");

                    //checking expected default values
                    panels.Should().HaveCount(5);
                    activePanels.Should().HaveCount(1);
                    panels[0].ClassList.Contains("mud-tab-active").Should().BeTrue();

                    for (var i = 1; i < comp.Instance.Tabs.Count; i++)
                    {
                        await invoker(comp, comp.Instance.Tabs[i]);

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
                (x,y) => x.Instance.ActivateTabAsync(y.Index, true),
                (x,y) => x.Instance.ActivateTabAsync(y.Panel, true),
                (x,y) => x.Instance.ActivateTabAsync(y.Tag, true),
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
        public async Task RenderHeaderBasedOnPosition(TabHeaderPosition position)
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabHeaderPosition, position));
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

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
        public async Task RenderHeaderBasedOnPosition_None()
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

            var headerContent = comp.FindAll(".test-header-content");
            headerContent.Should().BeEmpty();
        }

        /// <summary>
        /// The panel header header should be rendered based on the value of header position.
        /// </summary>
        [Test]
        [TestCase(TabHeaderPosition.After)]
        [TestCase(TabHeaderPosition.Before)]
        public async Task RenderHeaderPanelBasedOnPosition(TabHeaderPosition position)
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabPanelHeaderPosition, position));

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
        public async Task RenderHeaderPanelBasedOnPosition_None()
        {
            var comp = Context.RenderComponent<TabsWithHeaderTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabHeaderPosition, TabHeaderPosition.None));
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.TabPanelHeaderPosition, TabHeaderPosition.None));

            var headerContent = comp.FindAll(".test-panel-header-content");
            headerContent.Should().BeEmpty();
        }

        [Test]
        public async Task TabPanelIconColorOverridesTabIconColor()
        {
            var comp = Context.RenderComponent<TabPanelIconColorTest>();
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.MudTabPanelIconColor, Color.Success));

            var iconRef = comp.Find(".mud-icon-root.mud-svg-icon");
            iconRef.ClassList.Should().Contain("mud-success-text");
        }

        [Test]
        public async Task TabPanelIconColorOverridesTabIconColorExceptWhenDisabled()
        {
            var comp = Context.RenderComponent<TabPanelIconColorTest>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DisableTab, true));
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.MudTabPanelIconColor, Color.Success));

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
        public async Task PrePanelContent()
        {
            var comp = Context.RenderComponent<TabsWithPrePanelContent>(p => p.Add(x => x.SelectedIndex, 0));

            var content = comp.Find(".pre-panel-content-custom");

            content.TextContent.Should().Be("Selected: Tab One");

            content.PreviousElementSibling.ClassList.Should().Contain("mud-tabs-tabbar");
            content.NextElementSibling.ClassList.Should().Contain("mud-tabs-panels");

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.SelectedIndex, 1));

            content = comp.Find(".pre-panel-content-custom");

            content.TextContent.Should().Be("Selected: Tab Two");

            content.PreviousElementSibling.ClassList.Should().Contain("mud-tabs-tabbar");
            content.NextElementSibling.ClassList.Should().Contain("mud-tabs-panels");
        }

        [Test]
        public async Task CancelPanelActivation()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = Context.RenderComponent<CancelActivationTabsTest>();
            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Position, Position.Left));

            await comp.Instance.SetPanelActiveAsync(2);
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
            substring = substring.Remove(substring.Length - 1);
            var value = double.Parse(substring, CultureInfo.InvariantCulture);

            return value;
        }

        #endregion

        [Test]
        public void DynamicTabs_CollectionRenderSyncTest()
        {
            var comp = Context.RenderComponent<DynamicTabsSimpleTest>();

            var userTabs = comp.Instance.UserTabs;
            var mudTabs = comp.FindComponent<MudDynamicTabs>();

            // Initial
            userTabs.Count.Should().Be(3);
            mudTabs.Instance.Panels.Count.Should().Be(3);

            // Remove
            comp.Instance.RemoveTab(userTabs.Last().Id);
            userTabs.Count.Should().Be(2);
            comp.Render(); // Render to refresh MudTabs
            mudTabs.Instance.Panels.Count.Should().Be(2);

            // Add
            comp.Instance.AddTab(Guid.NewGuid());
            userTabs.Count.Should().Be(3);
            comp.Render(); // Render to refresh MudTabs
            mudTabs.Instance.Panels.Count.Should().Be(3);

            // Remove all, no ArgumentOutOfRangeException should be thrown.
            comp.Instance.RemoveTab(userTabs.Last().Id);
            comp.Instance.RemoveTab(userTabs.Last().Id);
            comp.Instance.RemoveTab(userTabs.Last().Id);
            userTabs.Count.Should().Be(0);
            comp.Render(); // Render to refresh MudTabs.
            mudTabs.Instance.Panels.Count.Should().Be(0);

            // No active panel.
            mudTabs.Instance.ActivePanel.Should().BeNull();

            // No active panel means no active panel index.
            comp.Instance.UserIndex.Should().Be(-1);
            mudTabs.Instance.ActivePanelIndex.Should().Be(-1);
        }

        [Test]
        public void TabPanel_ShowCloseIconTest()
        {
            var comp = Context.RenderComponent<DynamicTabsSimpleTest>();
            var tabs = comp.FindAll("div.mud-tab");
            tabs[0].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeTrue();
            tabs[1].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeFalse(); // The close icon is not shown.
            tabs[2].InnerHtml.Contains("mud-icon-root mud-svg-icon").Should().BeTrue();
        }

        [Test]
        public async Task Tabs_HaveRipple_WhenRippleIsTrue()
        {
            var comp = Context.RenderComponent<TabsRippleTest>(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Ripple, false));
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TabPanel_Hidden_Class(bool visible)
        {
            var comp = Context.RenderComponent<TabsVisibleTest>(parameters => parameters.Add(x => x.Visible, visible));

            var panel = comp.FindAll(".mud-tab-panel")[1];
            if (visible)
            {
                panel.ClassList.Should().NotContain("mud-tab-panel-hidden");
            }
            else
            {
                panel.ClassList.Should().Contain("mud-tab-panel-hidden");
            }
        }

#nullable enable
        [Test]
        public async Task TabsDragAndDrop_With_FiresOnItemDroppedAsync()
        {
            bool onItemDroppedCalled = false;
            MudItemDropInfo<MudTabPanel>? finalDropInfo = null;

            var comp = Context.RenderComponent<TabsDragAndDropTest>(
                parameters => parameters.Add(p => p.ItemDroppedFired, (MudItemDropInfo<MudTabPanel> info) =>
                {
                    onItemDroppedCalled = true;
                    finalDropInfo = info;
                })
            );

            // should be 3 draggable tabs
            var droptabs = comp.FindAll("div[draggable='false']");
            droptabs.Count.Should().Be(2); // disabled droptab plus beginning ghost tab
            droptabs = comp.FindAll("div[draggable='true']");
            droptabs.Count.Should().Be(3); // enabled droptabs
            // should be 1 draggable "drop zone" to allow reordering
            var dropzone = comp.FindAll("div.mud-drop-zone");
            dropzone.Count.Should().Be(1);

            // Find the first draggable tab and the drop zone
            var tabs = comp.FindComponent<MudTabs>().Instance;
            var draggableTab = tabs._panels[0];
            var dropZone = comp.Find("div.mud-drop-zone");

            // simulate dragging a tab to index 2
            var dropInfo = new MudItemDropInfo<MudTabPanel>(draggableTab, "mud-drop-zone", 2);
            await tabs.ItemUpdated(dropInfo);

            // Assert that OnItemDropped was called
            comp.WaitForAssertion(() => onItemDroppedCalled.Should().BeTrue());
            finalDropInfo.Should().Be(dropInfo);
        }
#nullable disable

        [Test]
        public void LabelSorting_NaturalOrderIfSortingUnspecified()
        {
            // all parameters unspecified
            var comp = Context.RenderComponent<LabelSortTest>();

            // all labels should be present and in natural order
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("3");
        }

        [Test]
        public void LabelSorting_SpecifiedDirectionWithoutKeysOrComparer()
        {
            /* ***
             * all labels should be present and in natural order
             */
            var comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.None)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("3");

            /* ***
             * all labels should be present and in lexicographically ascending order
             */
            comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.Ascending)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("3");

            /* ***
             * all labels should be present and in lexicographically descending order
             */
            comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.Descending)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("3");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("1");
        }

        [Test]
        public void LabelSorting_SpecifiedDirectionWithKeysAndDefaultComparer()
        {
            // Caution: intentionally descending order to ensure this behaviour overrides Text ordering
            string[] sortKeys = ["c", "b", "a"];

            /* ***
             * all labels should be present and in natural order
             */
            var comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.None),
                ComponentParameter.CreateParameter("SortKeys", sortKeys)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(4);
            // sort order is per markup: 2, 1, 3, 4. Keys are ignored as list is unsorted.
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("3");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[3].InnerHtml.Should().Be("4");

            /* ***
             * all labels should be present and in lexicographically ascending order
             */
            comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.Ascending),
                ComponentParameter.CreateParameter("SortKeys", sortKeys)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(4);
            // sort order is: 4, a=3, b=1, c=2
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("4");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("3");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[3].InnerHtml.Should().Be("2");

            /* ***
             * all labels should be present and in lexicographically descending order
             */
            comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortDirection", SortDirection.Descending),
                ComponentParameter.CreateParameter("SortKeys", sortKeys)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(4);
            // sort order is: c=2, b=1, a=3, 4
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("2");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("1");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("3");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[3].InnerHtml.Should().Be("4");
        }

        [Test]
        public void LabelSorting_CustomSortComparerIgnoresSortDirection()
        {
            /* ***
             * All labels should be present and in Tag order, ignoring SortDirection and Keys.
             * For this test the Tabs.SortDirection is set to Descending, and the SortKeys
             * are set to Apple=3, Banana=2, Cherry=1, so there is no combination of SortKey, Label
             * or SortDirection that could ellicit the same sort order as we get from TestComparer.
             */
            var comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortComparer", new LabelSortTest.TestComparer()),
                ComponentParameter.CreateParameter("SortDirection", SortDirection.Descending)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("Cherry");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("Apple");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("Banana");
        }

        [Test]
        public void LabelSorting_CustomSortComparerWorksWithoutSortDirection()
        {
            /* ***
             * All labels should be present and in Tag order, SortDirection and Keys are unspecified.
             * For this test the Tabs.SortDirection is left unset, and the SortKeys
             * are set to Apple=3, Banana=2, Cherry=1, so there is no combination of SortKey, Label
             * or SortDirection that could ellicit the same sort order as we get from TestComparer.
             */
            var comp = Context.RenderComponent<LabelSortTest>(
                ComponentParameter.CreateParameter("SortComparer", new LabelSortTest.TestComparer()),
                ComponentParameter.CreateParameter("SortDirection", null)
            );
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab").Count.Should().Be(3);
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[0].InnerHtml.Should().Be("Cherry");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[1].InnerHtml.Should().Be("Apple");
            comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab")[2].InnerHtml.Should().Be("Banana");
        }

        [Test]
        public async Task Tab_DragAndDrop_ActiveIndexShouldNotChangeDisplay()
        {
            // defaulting the ActiveIndex to something other than 0 caused a display issue where it tried to make
            // that tab the FIRST tab putting any leading tabs underneath an arrow to "go left" (or right if rtl)
            // https://github.com/MudBlazor/MudBlazor/issues/11519
            var comp = Context.RenderComponent<ActivatePanelDragAndDropTest>();
            var divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            // no drop container
            comp.FindAll("div.mud-drop-container").Count().Should().Be(0);
            // all tabs should show
            divs.Count.Should().Be(4);
            divs[0].InnerHtml.Should().Be("One");
            divs[1].InnerHtml.Should().Be("Two");
            divs[2].InnerHtml.Should().Be("Three");
            divs[3].InnerHtml.Should().Be("Four");
            // no scroll bar should show
            comp.FindAll(".mud-tabs-scroll-button").Should().BeEmpty();
            // enable drag and drop
            var cbox = comp.Find("div.drag-drop-class input");
            cbox.Change(true);
            await comp.SetParametersAndRenderAsync();
            // drop container
            comp.WaitForAssertion(() => comp.FindAll("div.mud-drop-container").Count.Should().Be(1));
            divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            // all tabs should show
            divs.Count.Should().Be(4);
            divs[0].InnerHtml.Should().Be("One");
            divs[1].InnerHtml.Should().Be("Two");
            divs[2].InnerHtml.Should().Be("Three");
            divs[3].InnerHtml.Should().Be("Four");
        }

        [Test]
        public async Task Tab_DragAndDrop_ActivatePanel()
        {
            // ensures that the active tab class and custom class is updated when the index is updated regardless
            // of drag and drop. When enabled Drag and Drop was not properly updating state when a new item was clicked.
            // This was a bug caused by the Drag and Drop feature not updating it's display and fixed by creating a ref
            // and calling .Refresh() on ActivatePanel (clicking, drag and drop, etc). Basically the changes were too deep
            // for blazor to know it should update state
            // https://github.com/MudBlazor/MudBlazor/issues/11549
            var comp = Context.RenderComponent<ActivatePanelDragAndDropTest>();
            var divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            // no drop container
            comp.FindAll("div.mud-drop-container").Count().Should().Be(0);
            // all tabs should show
            divs.Count.Should().Be(4);
            divs[0].InnerHtml.Should().Be("One");
            divs[1].InnerHtml.Should().Be("Two");
            divs[2].InnerHtml.Should().Be("Three");
            divs[3].InnerHtml.Should().Be("Four");
            // no scroll bar should show
            comp.FindAll(".mud-tabs-scroll-button").Should().BeEmpty();
            // clicking a tab should activate it and update the class
            divs[2].Click(); // activate Three
            divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            comp.WaitForAssertion(() => divs[2].ClassList.Contains("mud-tab-active").Should().BeTrue());
            // enable drag and drop
            var cbox = comp.Find("div.drag-drop-class input");
            cbox.Change(true);
            await comp.SetParametersAndRenderAsync(p => p.Add(p => p.ActiveTabClass, "test-active"));
            // drop container
            comp.WaitForAssertion(() => comp.FindAll("div.mud-drop-container").Count.Should().Be(1));
            divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            // all tabs should show
            divs.Count.Should().Be(4);
            divs[0].InnerHtml.Should().Be("One");
            divs[1].InnerHtml.Should().Be("Two");
            divs[2].InnerHtml.Should().Be("Three");
            divs[3].InnerHtml.Should().Be("Four");
            divs[3].Click();
            divs = comp.FindAll("div.mud-tabs-tabbar-wrapper div.mud-tab");
            comp.WaitForAssertion(() => divs[3].ClassList.Contains("mud-tab-active").Should().BeTrue());
            comp.WaitForAssertion(() => divs[3].ClassList.Contains("test-active").Should().BeTrue());
        }

        /// <summary>
        /// Tab selection changes on keyboard Left and Right arrow keys, is activated by Enter/Space keys and ensures disabled tab is not selectable. 
        /// </summary>
        [Test]
        public async Task KeyboardActivation_DisablesDisabledTab_LeftRight()
        {
            var comp = Context.RenderComponent<TabsKeyboardAccessibilityTest>();
            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content One");

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[0].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" });
            });
            var tabsAfterArrowRight = comp.FindAll("div.mud-tab");
            await comp.InvokeAsync(async () =>
            {
                await tabsAfterArrowRight[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
            });
            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content Two");

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowLeft" });
            });
            var tabsAfterArrowLeft = comp.FindAll("div.mud-tab");
            await comp.InvokeAsync(async () =>
            {
                await tabsAfterArrowLeft[0].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
            });
            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content One");
        }

        /// <summary>
        /// Tab selection changes on keyboard Up and Down arrow keys, is activated by Enter/Space keys
        /// </summary>
        [Test]
        public async Task VerticalTabs_SupportsArrowUpDownNavigation()
        {
            var comp = Context.RenderComponent<VerticalTabsKeyboardAccessibilityTest>();
            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[0].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowDown" });
            });

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
            });

            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content Two");
            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowDown" });
            });

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[2].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
            });
            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content Three");
        }


        /// <summary>
        /// Tab selection wraps on keyboard Left and Right arrow keys, is activated by Enter/Space keys and ensures disabled tab is not selectable. 
        /// </summary>
        [Test]
        public async Task KeyboardNavigation_LeftArrow_WrapsToLastEnabledTab()
        {
            var comp = Context.RenderComponent<TabsKeyboardAccessibilityTest>();
            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[0].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowLeft" });
            });

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
            });

            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content Two");
            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[1].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowLeft" });
            });

            await comp.InvokeAsync(async () =>
            {
                var tabs = comp.FindAll("div.mud-tab");
                await tabs[0].TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
            });
            comp.Find("div.mud-tabs-panels").InnerHtml.Should().Contain("Content One");
        }

        /// <summary>
        /// Code coverage test showed a missing test line, this tests the return tabListId returns the correct ID. 
        /// </summary>
        [Test]
        public void TabListId_ReturnsCorrectId()
        {
            var comp = Context.RenderComponent<MudTabs>();
            var instance = comp.Instance;

            // Use reflection to set the internal field _tabListId
            var field = typeof(MudTabs).GetField("_tabListId", BindingFlags.NonPublic | BindingFlags.Instance);
            field.Should().NotBeNull("because the field '_tabListId' should exist on MudTabs");
            field!.SetValue(instance, "test-tab-list-id");

            var method = typeof(MudTabs).GetMethod("GetTabListId", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Should().NotBeNull("because the method 'GetTabListId' should exist on MudTabs");
            var result = method!.Invoke(instance, null);

            result.Should().Be("test-tab-list-id");
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        /// <summary>
        /// A test to ensure TabWrapperContent and Tooltip is rendered in both regular and EnableDragandDrop modes
        /// </summary>
        public void TabWrapperTest(bool enableDrag)
        {
            // initial pass at false fail at true, github issue 12006
            var comp = Context.RenderComponent<TabWrapperContentTest>(t => t.Add(x => x.EnableDragAndDrop, enableDrag));
            var wrapperDiv = comp.Find(".wrapper-class-content");
            wrapperDiv.Should().NotBeNull();
            var tooltipDiv = comp.Find(".mud-tabs-tabbar-content .mud-tooltip-root .mud-popover-cascading-value");
            tooltipDiv.Should().NotBeNull();
        }

        /// <summary>
        /// Test if TabButtonClass and TabPanelsClass are applying the CSS classes properly to TabPanel
        /// </summary>
        [Test]
        public void TabPanel_AppliesItsOwnCssClasses()
        {
            var tabButtonClasses = "class1";
            var panelClasses = "class2";

            var comp = Context.RenderComponent<TabPanelCssClassesMatchTest>(parameters => parameters
                .Add(p => p.TabButtonClass, tabButtonClasses)
                .Add(p => p.PanelClass, panelClasses));

            var tabButtonRef = comp.Find(".mud-tab.mud-tab-panel");
            tabButtonRef.ClassList.Should().Contain(tabButtonClasses);
            tabButtonRef.ClassList.Should().NotContain(panelClasses);

            var panelRef = comp.Find(":not(.mud-tab).mud-tab-panel");
            panelRef.ClassList.Should().Contain(panelClasses);
            panelRef.ClassList.Should().NotContain(tabButtonClasses);
        }

        /// <summary>
        /// Test if TabButtonClass and TabPanelsClass are applying the CSS classes properly to Tabs
        /// </summary>
        [Test]
        public void Tabs_AppliesItsOwnCssClasses()
        {
            var tabButtonClasses = "class1";
            var tabPanelClasses = "class2";

            var comp = Context.RenderComponent<TabsCssClassesMatchTest>(parameters => parameters
                .Add(p => p.TabButtonClass, tabButtonClasses)
                .Add(p => p.TabPanelClass, tabPanelClasses));

            var tabButtonRef = comp.Find(".mud-tab.mud-tab-panel");
            tabButtonRef.ClassList.Should().Contain(tabButtonClasses);
            tabButtonRef.ClassList.Should().NotContain(tabPanelClasses);

            var panelRef = comp.Find(".mud-tabs-panels");
            panelRef.ClassList.Should().Contain(tabPanelClasses);
            panelRef.ClassList.Should().NotContain(tabButtonClasses);
        }

        /// <summary>
        /// Test if Tabs and TabPanels combined TabButtonClass and TabPanelsClass are applying the CSS classes properly
        /// </summary>
        [Test]
        public void Tabs_And_TabPanel_CombinedClassesTest()
        {
            var comp = Context.RenderComponent<TabsAndTabPanelCssClassesMatchTest>();

            var tabButtons = comp.FindAll(".mud-tab");
            tabButtons.Should().AllSatisfy(x => x.ClassList.Should().Contain("mud-tabs-button-class"));

            tabButtons[0].ClassList.Should().Contain("mud-tab-panel-button-class-1");
            tabButtons[0].ClassList.Should().NotContain("mud-tabs-panel-class");
            tabButtons[0].ClassList.Should().NotContain("mud-tab-panel-panel-class-1");

            tabButtons[1].ClassList.Should().Contain("mud-tab-panel-button-class-2");
            tabButtons[1].ClassList.Should().NotContain("mud-tabs-panel-class");
            tabButtons[1].ClassList.Should().NotContain("mud-tab-panel-panel-class-2");

            var tabsPanels = comp.Find(".mud-tabs-panels");
            tabsPanels.ClassList.Should().Contain("mud-tabs-panel-class");
            tabsPanels.ClassList.Should().NotContain("mud-tabs-button-class");

            TabsAndTabPanelCssClassesMatchTest.SelectedTab = 0;
            comp = Context.RenderComponent<TabsAndTabPanelCssClassesMatchTest>();
            var activeTabPanel = comp.Find(":not(.mud-tab).mud-tab-panel");
            activeTabPanel.ClassList.Should().Contain("mud-tab-panel-panel-class-1");
            activeTabPanel.ClassList.Should().NotContain("mud-tab-panel-button-class-1");
            activeTabPanel.ClassList.Should().NotContain("mud-tabs-panel-class");

            TabsAndTabPanelCssClassesMatchTest.SelectedTab = 1;
            comp = Context.RenderComponent<TabsAndTabPanelCssClassesMatchTest>();
            activeTabPanel = comp.Find(":not(.mud-tab).mud-tab-panel");
            activeTabPanel.ClassList.Should().Contain("mud-tab-panel-panel-class-2");
            activeTabPanel.ClassList.Should().NotContain("mud-tab-panel-button-class-2");
            activeTabPanel.ClassList.Should().NotContain("mud-tabs-panel-class");
        }

        /// <summary>
        /// Tests the behavior of mouse events on tab headers, including closing tabs via mouse actions.
        /// </summary>
        [Test]
        public async Task TabHeaderMouseDownEvents()
        {
            var comp = Context.RenderComponent<ClosableTabsWithHeaderTest>();

            // Close the tab with the mouse wheel click.
            var tabs = comp.FindAll("div.mud-tab");
            tabs.Count.Should().Be(6);

            await tabs[1].TriggerEventAsync("onmousedown", new MouseEventArgs { Button = 1 });

            comp.FindAll("div.mud-tab").Count
                .Should().Be(5);

            // Close all but the selected tab.
            tabs = comp.FindAll("div.mud-tab");
            tabs.Count.Should().Be(5);

            await tabs[1].TriggerEventAsync("oncontextmenu", default);

            var menuItems = comp.FindComponents<MudMenuItem>();
            menuItems.Count.Should().Be(3);
            menuItems[2].Find(".mud-menu-item").Click();

            comp.FindAll("div.mud-tab").Count
                .Should().Be(1);

            // Close All tabs.
            tabs = comp.FindAll("div.mud-tab");
            await tabs[0].TriggerEventAsync("oncontextmenu", default);

            comp.FindComponents<MudMenuItem>()[1].Find(".mud-menu-item").Click();

            comp.FindAll("div.mud-tab").Count
                .Should().Be(0);
        }
    }
}

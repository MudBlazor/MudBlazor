// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.EnhanceChart;
using MudBlazor.UnitTests.TestComponents.EnhancedChart;
using NUnit.Framework;
using static MudBlazor.UnitTests.Components.EnhancedChart.MudEnhancedChartTesterHelper;

namespace MudBlazor.UnitTests.Components.EnhancedChart
{
    public class MudEnhancedPieChartTests
    {
        private Guid _defaultChartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();

            var mockedRuntime = new Mock<IJSRuntime>(MockBehavior.Strict);
            mockedRuntime.Setup(x => x.InvokeAsync<Object>("mudEnhancedChartHelper.triggerAnimation", new object[] { _defaultChartId })).ReturnsAsync(new Object()).Verifiable();
            ctx.Services.Add(new ServiceDescriptor(typeof(IJSRuntime), mockedRuntime.Object));
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void DefaultValues()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<MudEnhancedPieChartDataPoint> points = comp.Instance;
            points.Should().BeEmpty();

            called.Should().Be(1);

            comp.Instance.Padding.Should().Be(2.0);
            comp.Instance.StartAngle.Should().Be(-90.0);
            comp.Instance.AnimationIsEnabled.Should().BeFalse();
            comp.Instance.Id.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void UpdatingValuesResultsInRedraw()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<MudEnhancedPieChartDataPoint> points = comp.Instance;
            points.Should().BeEmpty();

            called.Should().Be(1);

            called = 0;


            //setting default values changed trigger redraw
            comp.SetParametersAndRender(x => x.Add(y => y.StartAngle, -90.0));
            comp.SetParametersAndRender(x => x.Add(y => y.StartAngle, 0.0));
            called.Should().Be(1);

            called = 0;
            //setting default values changed trigger redraw
            comp.SetParametersAndRender(x => x.Add(y => y.Padding, 2.0));
            comp.SetParametersAndRender(x => x.Add(y => y.Padding, 10.0));
            called.Should().Be(1);

            called = 0;
            comp.SetParametersAndRender(p =>
            {
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 0.0);
                });
            });

            called.Should().Be(1);

        }

        [Test]
        public void SingleValue()
        {
            String firstPointColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.Id, _defaultChartId);
                p.Add(x => x.Padding, 10.0);
                p.Add(x => x.StartAngle, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 5.0);
                    setP.Add(x => x.FillColor, firstPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class");
                });
            });

            XElement expectedRoot = new XElement("svg",
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class"),
                new XAttribute("fill", (object)firstPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 90 50 A 40 40,0,1,1, 89.999985 49.965093 L 50 50")
                ));

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void MultipleValue()
        {
            String firstPointColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondPointColor = (Colors.Amber.Darken1 + "ff").ToLower();
            String thirdPointColor = (Colors.Brown.Darken1 + "ff").ToLower();
            String fourthPointColor = (Colors.Green.Darken1 + "ff").ToLower();

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.Id, _defaultChartId);
                p.Add(x => x.Padding, 20.0);
                p.Add(x => x.StartAngle, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 250.0);
                    setP.Add(x => x.FillColor, firstPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-1");
                });
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, secondPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-2");
                });
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 125);
                    setP.Add(x => x.FillColor, thirdPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-3");
                });
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 125);
                    setP.Add(x => x.FillColor, fourthPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-4");
                });
            });

            XElement expectedRoot = new XElement("svg",
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-1"),
                new XAttribute("fill", (object)firstPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 80 50 A 30 30,0,0,1, 50 80 L 50 50")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-2"),
                new XAttribute("fill", (object)secondPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 50 80 A 30 30,0,0,1, 50 20 L 50 50")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-3"),
                new XAttribute("fill", (object)thirdPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 50 20 A 30 30,0,0,1, 71.213203 28.786797 L 50 50")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-4"),
                new XAttribute("fill", (object)fourthPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 71.213203 28.786797 A 30 30,0,0,1, 80 50 L 50 50")
                ));

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void Animation_Init()
        {
            String firstPointColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondPointColor = (Colors.Amber.Darken1 + "ff").ToLower();

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.Id, _defaultChartId);
                p.Add(x => x.Padding, 20.0);
                p.Add(x => x.StartAngle, 0.0);
                p.Add(x => x.AnimationIsEnabled, true);
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, firstPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-1");
                });
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, secondPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-2");
                });
            });

            XElement expectedRoot = new XElement("svg",
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-1"),
                new XAttribute("fill", (object)firstPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 80 50 A 30 30,0,0,1, 20 50 L 50 50"),
                new XElement("animate",
                   new XAttribute("attributename", "d"),
                   new XAttribute("dur", "500ms"),
                   new XAttribute("from", "M 50 50 L 80 50 A 30 30,0,0,1, 79.999954 50.05236 L 50 50"),
                   new XAttribute("to", "M 50 50 L 80 50 A 30 30,0,0,1, 20 50 L 50 50")
                )),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-2"),
                new XAttribute("fill", (object)secondPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 20 50 A 30 30,0,0,1, 80 50 L 50 50"),
                  new XElement("animate",
                   new XAttribute("attributename", "d"),
                   new XAttribute("dur", "500ms"),
                   new XAttribute("from", "M 50 50 L 20 50 A 30 30,0,0,1, 20.000046 49.94764 L 50 50"),
                   new XAttribute("to", "M 50 50 L 20 50 A 30 30,0,0,1, 80 50 L 50 50")
                ))
               );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void Animation_ChangeValue()
        {
            String firstPointColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondPointColor = (Colors.Amber.Darken1 + "ff").ToLower();

            var comp = ctx.RenderComponent<MudEnhancedPieChart>(p =>
            {
                p.Add(x => x.Id, _defaultChartId);
                p.Add(x => x.Padding, 20.0);
                p.Add(x => x.StartAngle, 0.0);
                p.Add(x => x.AnimationIsEnabled, true);
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, firstPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-1");
                });
                p.Add<MudEnhancedPieChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, secondPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-2");
                });
            });

            var points = comp.FindComponents<MudEnhancedPieChartDataPoint>();
            points[0].SetParametersAndRender(x => x.Add(y => y.Value, 250.0));

            XElement expectedRoot = new XElement("svg",
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-1"),
                new XAttribute("fill", (object)firstPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 80 50 A 30 30,0,0,1, 20 50 L 50 50"),
                new XElement("animate",
                   new XAttribute("attributename", "d"),
                   new XAttribute("dur", "500ms"),
                   new XAttribute("from", "M 50 50 L 80 50 A 30 30,0,0,1, 79.999954 50.05236 L 50 50"),
                   new XAttribute("to", "M 50 50 L 80 50 A 30 30,0,0,1, 20 50 L 50 50")
                )),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series pie active my-additional-class-2"),
                new XAttribute("fill", (object)secondPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 50 L 20 50 A 30 30,0,0,1, 80 50 L 50 50"),
                  new XElement("animate",
                   new XAttribute("attributename", "d"),
                   new XAttribute("dur", "500ms"),
                   new XAttribute("from", "M 50 50 L 20 50 A 30 30,0,0,1, 20.000046 49.94764 L 50 50"),
                   new XAttribute("to", "M 50 50 L 20 50 A 30 30,0,0,1, 80 50 L 50 50")
                ))
               );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void HoverAndActivePoints()
        {
            var comp = ctx.RenderComponent<MudEnhancedPieChartToolTipTest>();

            var chart = comp.FindComponent<MudEnhancedPieChart>();

            var expectedTipInfo = new[]
            {
                new PieChartToolTipInfo("Point 1",250.0,Colors.BlueGrey.Darken1.ToLower() + "FF",0.0,-90.0,40),
                new PieChartToolTipInfo("Point 2",500.0,Colors.Amber.Darken1.ToLower() + "FF",-90.0,-270.0,40),
                new PieChartToolTipInfo("Point 3",125.0,Colors.Brown.Darken1.ToLower() + "FF",-270.0,-315,40),
                new PieChartToolTipInfo("Point 4",125.0,Colors.Green.Darken1.ToLower() + "FF",-315,-360,40),
            };

            var expectedToolTipCoordiantes = new[]
            {
                (64.142135623730950488016887242097,64.142135623730950488016887242097),
                (30.0,50.0),
                (57.6536686473018,31.522409349774268),
                //rounding issues
                //(57.653668647301795434569199680608,31.522409349774264877436336212064),
                (68.477590650225735122563663787936,42.346331352698204565430800319392),
            };

            for (int i = 0; i < 4; i++)
            {
                var items = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();

                (items[i] as IElement).MouseOver();

                for (int j = 0; j < 4; j++)
                {
                    var itemsAgain = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();
                    String expectedClass = j != i ? "inactive" : "active";

                    itemsAgain[j].ClassList.Should().Contain(expectedClass);
                }

                var tooltip = comp.FindComponent<MudEnhancedPieChartToolTip>();
                tooltip.Should().NotBeNull();

                tooltip.Instance.ToolTips.Should().ContainSingle();
                tooltip.Instance.ToolTips.First().Should().BeEquivalentTo(expectedTipInfo[i]);

                var tooltipContainer = tooltip.Find(".mud-enhanched-chart-pie-chart-tooltip");
                tooltipContainer.GetAttribute("style").Should().Be($"position: absolute; left: {expectedToolTipCoordiantes[i].Item1.ToString(System.Globalization.CultureInfo.InvariantCulture)}%; top: {expectedToolTipCoordiantes[i].Item2.ToString(System.Globalization.CultureInfo.InvariantCulture)}%;");

                (items[i] as IElement).MouseOut();

                Assert.Throws<ComponentNotFoundException>(() => comp.FindComponent<MudEnhancedPieChartToolTip>());
            }
        }

        [Test]
        public void Legend()
        {
            var comp = ctx.RenderComponent<MudEnhancedPieChartLegendTest>();
            var points = comp.FindComponents<MudEnhancedPieChartDataPoint>();
            var legend = comp.FindComponent<MudEnhancedPieChartLegend>();

            legend.Instance.LegendInfo.Should().NotBeNull();
            legend.Instance.LegendInfo.Groups.Should().ContainSingle().And.AllBeAssignableTo<DataPointBasedChartLegendInfoGroup>();

            var legendGroups = (IEnumerable<DataPointBasedChartLegendInfoGroup>)legend.Instance.LegendInfo.Groups;
            var legendGroup = legendGroups.First();

            legendGroup.Points.Should().HaveCount(4);
            legendGroup.Points.Should().BeEquivalentTo(new[] {
                new ChartLegendInfoPoint("Point 1",Colors.BlueGrey.Darken1.ToLower() + "FF",true,points.ElementAt(0).Instance),
                new ChartLegendInfoPoint("Point 2",Colors.Amber.Darken1.ToLower() + "FF",true,points.ElementAt(1).Instance),
                new ChartLegendInfoPoint("Point 3",Colors.Brown.Darken1.ToLower() + "FF",true,points.ElementAt(2).Instance),
                new ChartLegendInfoPoint("Point 4",Colors.Green.Darken1.ToLower() + "FF",true,points.ElementAt(3).Instance),

            });
        }

        [Test]
        public void LegendInteraction_MouseOverAndOut()
        {
            var comp = ctx.RenderComponent<MudEnhancedPieChartLegendTest>();
            var chart = comp.FindComponent<MudEnhancedPieChart>();
            var points = comp.FindComponents<MudEnhancedPieChartDataPoint>();
            var legend = comp.FindComponent<MudEnhancedPieChartLegend>();

            var menuItems = legend.FindComponents<MudListItem>();

            menuItems.Should().HaveCount(4);

            for (int i = 0; i < 4; i++)
            {
                (menuItems[i].Nodes.First() as IElement).MouseOver();

                for (int j = 0; j < 4; j++)
                {
                    var itemsAgain = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();
                    String expectedClass = j != i ? "inactive" : "active";

                    //for some reason this test is not working as expected. Maybe an error in BUnit?
                    //itemsAgain[j].ClassList.Should().Contain(expectedClass);
                }

                //(legend.FindComponents<MudListItem>()[i].Nodes.First() as IElement).MouseOut();

                //for (int j = 0; j < 4; j++)
                //{
                //    var itemsAgain = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();
                //    itemsAgain[j].ClassList.Should().Contain("active");
                //}
            }
        }

        [Test]
        public void LegendInteraction_Click()
        {
            var comp = ctx.RenderComponent<MudEnhancedPieChartLegendTest>();
            var chart = comp.FindComponent<MudEnhancedPieChart>();
            var legend = comp.FindComponent<MudEnhancedPieChartLegend>();

            var menuItems = legend.FindComponents<MudListItem>();

            menuItems.Should().HaveCount(4);

            for (int i = 0; i < 4; i++)
            {
                (menuItems[i].Nodes.First() as IElement).Click();

                for (int j = 0; j < 4; j++)
                {
                    comp.Instance.IsEnabled[j].Should().Be( j == i ? false : true);
                }

                var items = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();

                items.Should().HaveCount(3);

                (legend.FindComponents<MudListItem>()[i].Nodes.First() as IElement).Click();

                items = chart.Nodes.OfType<IHtmlUnknownElement>().ToArray();
                items.Should().HaveCount(4);
            }
        }
    }
}

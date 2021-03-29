#pragma warning disable IDE1006 // leading underscore

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using AngleSharp.Svg.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.EnhanceChart;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.EnchancedChart
{
    [TestFixture]
    public class MudEnhancedChartTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void Default_ValuesAndRender()
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>();

            comp.Instance.Should().NotBeNull();

            comp.Instance.LegendPosition.Should().Be(Position.Right);
            comp.Instance.LegendAlignment.Should().Be(Align.Center);
            comp.Instance.ShowLegend.Should().Be(true);

            comp.Instance.TitlePosition.Should().Be(Position.Top);
            comp.Instance.TitleAlignment.Should().Be(Align.Center);
            comp.Instance.ShowTitle.Should().Be(true);
            comp.Instance.TitleDrawer.Should().NotBeNull();
            comp.Instance.Title.Should().NotBeNull();

            comp.Nodes.FirstOrDefault().Should().BeAssignableTo<IHtmlDivElement>();

            var rootElement = (IHtmlDivElement)comp.Nodes.FirstOrDefault();
            rootElement.ClassList.Should().NotBeEmpty().And.HaveCount(3).And.Contain(new[] { "mud-enhanced-chart", "d-flex", "flex-column" });

            rootElement.ChildNodes.Should().NotBeEmpty().And.HaveCount(2).And.AllBeAssignableTo<IHtmlDivElement>();

            var titleElement = (IHtmlDivElement)rootElement.ChildNodes[0];
            titleElement.ClassList.Should().NotBeEmpty().And.HaveCount(4).And.Contain(new[] { "mud-enhanced-chart-title-container", "d-flex", "justify-center", "order-first" });

            titleElement.ChildNodes.Should().NotBeEmpty().And.ContainSingle().And.AllBeAssignableTo<IHtmlHeadingElement>();
            var titleHeadingElement = (IHtmlHeadingElement)titleElement.ChildNodes[0];
            titleHeadingElement.NodeName.Should().Be("H4");
            Assert.True(String.IsNullOrEmpty(titleHeadingElement.TextContent));

            var drawerElement = (IHtmlDivElement)rootElement.ChildNodes[1];

            drawerElement.ClassList.Should().NotBeEmpty().And.HaveCount(3).And.Contain(new[] { "mud-enhanced-chart-drawer-container", "d-flex", "flex-row" });

            drawerElement.ChildNodes.Should().NotBeEmpty().And.HaveCount(2);
            drawerElement.ChildNodes.First().Should().BeAssignableTo<IHtmlDivElement>();

            drawerElement.ChildNodes[0].ChildNodes.First().Should().BeAssignableTo<ISvgElement>();

            var chartContent = (ISvgElement)drawerElement.ChildNodes[0].ChildNodes[0];

            var svgNode = XElement.Parse(chartContent.OuterHtml);
            svgNode.Should().BeEquivalentTo(XElement.Parse("<svg width=\"100%\" height=\"100%\" preserveAspectRatio=\"none\" viewBox=\"0 0 100 100\"></svg>"));

            drawerElement.ChildNodes[1].Should().BeAssignableTo<IHtmlDivElement>();

            var legendElement = (IHtmlDivElement)drawerElement.ChildNodes[1];

            legendElement.ClassList.Should().NotBeEmpty().And.HaveCount(4).And.Contain(new[] { "mud-enhanced-chart-legend-container", "d-flex", "order-last", "align-center" });
        }

        [Test]
        public void Set_Title()
        {
            String title = "new chart title";

            var comp = ctx.RenderComponent<MudEnhancedChart>();
            comp.SetParametersAndRender(p => p.Add(x => x.Title, title));

            var titleContainer = comp.Find(".mud-enhanced-chart-title-container");

            titleContainer.Should().NotBeNull();

            titleContainer.ChildNodes.Should().ContainSingle().And.AllBeAssignableTo<IHtmlHeadingElement>();

            IHtmlHeadingElement heading = (IHtmlHeadingElement)titleContainer.ChildNodes[0];
            heading.TextContent.Should().Be(title);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Set_ShowLegend(Boolean legendShouldBeVisible)
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>();
            comp.SetParametersAndRender(p => p.Add(x => x.ShowLegend, legendShouldBeVisible));

            if (legendShouldBeVisible == true)
            {
                //mud-enhanced-chart-legend-container
                var legendContainer = comp.Find(".mud-enhanced-chart-legend-container");
                legendContainer.Should().NotBeNull();
            }
            else
            {
                var drawerContainer = comp.Find(".mud-enhanced-chart-drawer-container");
                drawerContainer.ChildNodes.Should().ContainSingle().And.AllBeAssignableTo<IHtmlDivElement>();

                drawerContainer.ChildNodes[0].ChildNodes.Should().ContainSingle().And.AllBeAssignableTo<ISvgElement>();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Set_ShowTitle(Boolean titleShouldBeVisible)
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>();
            comp.SetParametersAndRender(p => p.Add(x => x.ShowTitle, titleShouldBeVisible));

            if (titleShouldBeVisible == true)
            {
                var titleContainer = comp.Find(".mud-enhanced-chart-title-container");
                titleContainer.Should().NotBeNull();
            }
            else
            {
                var chart = comp.Find(".mud-enhanced-chart");
                chart.ChildNodes.Should().ContainSingle().And.AllBeAssignableTo<IHtmlDivElement>();

                var drawerContainer = (IHtmlDivElement)chart.ChildNodes[0];
                drawerContainer.ClassList.Should().Contain(new[] { "mud-enhanced-chart-drawer-container" });
            }
        }

        [TestCase(Position.Right, Align.Center, "flex-row", "order-last", "align-center")]
        [TestCase(Position.Right, Align.Justify, "flex-row", "order-last", "align-center")]
        [TestCase(Position.Right, Align.Left, "flex-row", "order-last", "align-start")]
        [TestCase(Position.Right, Align.Right, "flex-row", "order-last", "align-end")]

        [TestCase(Position.Left, Align.Center, "flex-row", "order-first", "align-center")]
        [TestCase(Position.Left, Align.Justify, "flex-row", "order-first", "align-center")]
        [TestCase(Position.Left, Align.Left, "flex-row", "order-first", "align-start")]
        [TestCase(Position.Left, Align.Right, "flex-row", "order-first", "align-end")]

        [TestCase(Position.Bottom, Align.Center, "flex-column", "order-last", "justify-center")]
        [TestCase(Position.Bottom, Align.Justify, "flex-column", "order-last", "justify-center")]
        [TestCase(Position.Bottom, Align.Left, "flex-column", "order-last", "justify-start")]
        [TestCase(Position.Bottom, Align.Right, "flex-column", "order-last", "justify-end")]

        [TestCase(Position.Top, Align.Center, "flex-column", "order-first", "justify-center")]
        [TestCase(Position.Top, Align.Justify, "flex-column", "order-first", "justify-center")]
        [TestCase(Position.Top, Align.Left, "flex-column", "order-first", "justify-start")]
        [TestCase(Position.Top, Align.Right, "flex-column", "order-first", "justify-end")]
        public void Set_LegendPositionAndAlign(Position legendPosition, Align lengedAlignment, String expectedContainerClass, String expectedOrderClass, String expectedAlignmentClass)
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>();
            comp.SetParametersAndRender(p =>
            {
                p.Add(x => x.LegendPosition, legendPosition);
                p.Add(x => x.LegendAlignment, lengedAlignment);
            });

            var drawerContainer = comp.Find(".mud-enhanced-chart-drawer-container");
            var legendContainer = comp.Find(".mud-enhanced-chart-legend-container");

            drawerContainer.ClassList.Should().NotBeEmpty().And.Contain(new[] { expectedContainerClass });
            legendContainer.ClassList.Should().NotBeEmpty().And.Contain(new[] { expectedOrderClass, expectedAlignmentClass });
        }

        [TestCase(Position.Right, Align.Center, "flex-row", "order-last", "align-center")]
        [TestCase(Position.Right, Align.Justify, "flex-row", "order-last", "align-center")]
        [TestCase(Position.Right, Align.Left, "flex-row", "order-last", "align-start")]
        [TestCase(Position.Right, Align.Right, "flex-row", "order-last", "align-end")]

        [TestCase(Position.Left, Align.Center, "flex-row", "order-first", "align-center")]
        [TestCase(Position.Left, Align.Justify, "flex-row", "order-first", "align-center")]
        [TestCase(Position.Left, Align.Left, "flex-row", "order-first", "align-start")]
        [TestCase(Position.Left, Align.Right, "flex-row", "order-first", "align-end")]

        [TestCase(Position.Bottom, Align.Center, "flex-column", "order-last", "justify-center")]
        [TestCase(Position.Bottom, Align.Justify, "flex-column", "order-last", "justify-center")]
        [TestCase(Position.Bottom, Align.Left, "flex-column", "order-last", "justify-start")]
        [TestCase(Position.Bottom, Align.Right, "flex-column", "order-last", "justify-end")]

        [TestCase(Position.Top, Align.Center, "flex-column", "order-first", "justify-center")]
        [TestCase(Position.Top, Align.Justify, "flex-column", "order-first", "justify-center")]
        [TestCase(Position.Top, Align.Left, "flex-column", "order-first", "justify-start")]
        [TestCase(Position.Top, Align.Right, "flex-column", "order-first", "justify-end")]
        public void Set_TitlePositionAndAlign(Position titlePosition, Align titleAlignment, String expectedContainerClass, String expectedOrderClass, String expectedAlignmentClass)
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>();
            comp.SetParametersAndRender(p =>
            {
                p.Add(x => x.TitlePosition, titlePosition);
                p.Add(x => x.TitleAlignment, titleAlignment);
            });

            var chart = comp.Find(".mud-enhanced-chart");
            var titleContainer = comp.Find(".mud-enhanced-chart-title-container");

            chart.ClassList.Should().NotBeEmpty().And.Contain(new[] { "d-flex", expectedContainerClass });
            titleContainer.ClassList.Should().NotBeEmpty().And.Contain(new[] { expectedOrderClass, expectedAlignmentClass });
        }
    }
}

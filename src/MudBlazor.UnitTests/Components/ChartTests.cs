
using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ChartTests : BunitTest
    {
        /// <summary>
        /// single checkbox, initialized false, check -  uncheck
        /// </summary>
        [Test]
        public void PieChartSelectionTest()
        {
            var comp = Context.RenderComponent<PieExample1>();
            // print the generated html
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: -1");
            // now click something and see that the selected index changes:
            comp.FindAll("path.mud-chart-serie")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");
            comp.FindAll("path.mud-chart-serie")[3].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 3");
        }

        [Test]
        public void DonutChartSelectionTest()
        {
            var comp = Context.RenderComponent<DonutExample1>();
            // print the generated html
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: -1");
            // now click something and see that the selected index changes:
            comp.FindAll("circle.mud-chart-serie")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");
            comp.FindAll("circle.mud-chart-serie")[3].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 3");
        }

        [Test]
        public void LineChartSelectionTest()
        {
            var comp = Context.RenderComponent<LineChartSelectionTest>();
            // print the generated html
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: -1");
            // now click something and see that the selected index changes:
            comp.FindAll("path.mud-chart-line")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");
            comp.FindAll("path.mud-chart-line")[1].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 1");
        }

        [Test]
        public void BarChartSelectionTest()
        {
            var comp = Context.RenderComponent<BarExample1>();
            // print the generated html
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: -1");
            // now click something and see that the selected index changes:
            comp.FindAll("path.mud-chart-bar")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");
            comp.FindAll("path.mud-chart-bar")[10].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 1");
        }

        [Test]
        public void BarChartYAxisFormat()
        {
            var options = new ChartOptions();
            var series = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
                new ChartSeries() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
            };
            var xAxis = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
            var width = "100%";
            var height = "350px";

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
              .Add(p => p.ChartType, ChartType.Line)
              .Add(p => p.ChartSeries, series)
              .Add(p => p.XAxisLabels, xAxis)
              .Add(p => p.ChartOptions, options)
              .Add(p => p.Width, width)
              .Add(p => p.Height, height)
            );

            // check the first Y Axis value without any format
            var yaxis = comp.FindAll("g.mud-charts-yaxis");
            yaxis.Should().NotBeNull();
            yaxis[0].Children[0].InnerHtml.Trim().Should().Be("0");

            // now, we will apply currency format
            options.YAxisFormat = "c2";
            comp.SetParametersAndRender(parameters => parameters
              .Add(p => p.ChartType, ChartType.Line)
              .Add(p => p.ChartSeries, series)
              .Add(p => p.XAxisLabels, xAxis)
              .Add(p => p.ChartOptions, options)
              .Add(p => p.Width, width)
              .Add(p => p.Height, height)
            );
            yaxis = comp.FindAll("g.mud-charts-yaxis");
            yaxis.Should().NotBeNull();
            yaxis[0].Children[0].InnerHtml.Trim().Should().Be($"{0:c2}");

            //number format
            options.YAxisFormat = "n6";
            comp.SetParametersAndRender(parameters => parameters
              .Add(p => p.ChartType, ChartType.Line)
              .Add(p => p.ChartSeries, series)
              .Add(p => p.XAxisLabels, xAxis)
              .Add(p => p.ChartOptions, options)
              .Add(p => p.Width, width)
              .Add(p => p.Height, height)
            );
            yaxis = comp.FindAll("g.mud-charts-yaxis");
            yaxis.Should().NotBeNull();
            yaxis[0].Children[0].InnerHtml.Trim().Should().Be($"{0:n6}");
        }

        /// <summary>
        /// Using only one x-axis value should not throw an exception
        /// this is from issue #7736
        /// </summary>
        [Test]
        public void BarChartWithSingleXAxisValue()
        {
            var comp = Context.RenderComponent<BarChartWithSingleXAxisTest>();

            comp.Markup.Should().NotContain("NaN");
        }

        /// <summary>
        /// High values should not lead to millions of horizontal grid lines
        /// this is from issue #1591 "Line chart is not able to plot big Double values"
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public void LineChartWithBigValues()
        {
            // the test should run through instantly (max 5s for a slow build server). 
            // without the fix it took minutes on a fast computer
            var comp = Context.RenderComponent<LineChartWithBigValuesTest>();
        }

        /// <summary>
        /// Zero values should not case an exception
        /// this is from issue #8282 "Line chart is not able to plot all zeroes"
        /// </summary>
        [Test]
        public void LineChartWithZeroValues()
        {
            var comp = Context.RenderComponent<LineChartWithZeroValuesTest>();

            comp.Markup.Should().NotContain("NaN");
        }

        ///// <summary> 
        ///// Checks if the element is added to the CustomGraphics RenderFragment
        ///// </summary>
        [Test]
        [TestCase(ChartType.Line, "Hello")]
        [TestCase(ChartType.Bar, "123")]
        [TestCase(ChartType.Donut, "Garderoben")]
        [TestCase(ChartType.Pie, "henon")]
        public void ChartCustomGraphics(ChartType chartType, string text)
        {
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
              .Add(p => p.ChartType, chartType)
              .Add(p => p.Width, "100%")
              .Add(p => p.Height, "300px")
              .Add(p => p.CustomGraphics, "<text class='text-ref'>" + text + "</text>")
            );

            //Checks if the innerHtml of the added text element matches the text parameter
            comp.Find("text.text-ref").InnerHtml.Should().Be(text);
        }
    }
}

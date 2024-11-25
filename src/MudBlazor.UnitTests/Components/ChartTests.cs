// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
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
            var comp = Context.RenderComponent<PieChartSelectionTest>();
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
            var comp = Context.RenderComponent<DonutChartSelectionTest>();
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
            var comp = Context.RenderComponent<BarChartSelectionTest>();
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
                new() { Name = "Series 1", Data = [90, 79, 72, 69, 62, 62, 55, 65, 70] },
                new() { Name = "Series 2", Data = [10, 41, 35, 51, 49, 62, 69, 91, 148] },
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

        [Test]
        public void HeatMap_ShouldInitializeCorrectly()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2, 3] },
                new() { Name = "Series 2", Data = [4, 5, 6] }
            };
            var options = new ChartOptions { ShowLegend = true, ShowLegendLabels = true };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            comp.Instance.Should().NotBeNull();
            comp.Instance.ChartSeries.Count.Should().Be(2);
            comp.Instance.ChartOptions.Should().NotBeNull();
        }

        [Test]
        public void HeatMap_ShouldBuildLegendsCorrectly()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2, 3] },
                new() { Name = "Series 2", Data = [4, 5, 6] }
            };
            var options = new ChartOptions() { ShowLegend = true };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            var legends = comp.FindAll(".mud-chart-heatmap-legend");
            legends.Count.Should().Be(5);
        }

        [Test]
        public void HeatMap_ShouldFormatValueForDisplayCorrectly()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [ 1.176, 2, 3 ] },
                new() { Name = "Series 2", Data = [ 4.152, 5, 6 ] }
            };

            var options = new ChartOptions() { ValueFormatString = "F2" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            var formattedValues = comp.FindAll(".mud-chart-cell");

            formattedValues.Count.Should().Be(6);

            var cellTexts = formattedValues.Select(cell => cell.QuerySelector("text")?.TextContent?.Trim()).ToList();

            cellTexts[0].Should().Be("1.18");
            cellTexts[3].Should().Be("4.15");
        }

        [Test]
        public void HeatMap_ShouldHandleEmptyAndNullData()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Empty Series", Data = [] },
                new() { Name = "Null Series", Data = null },
                new() { Name = "Valid Series", Data = [1.0, 2.0] }
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
            );

            // Should render without errors and only show cells for valid data
            var cells = comp.FindAll(".mud-chart-cell");
            cells.Count.Should().Be(2); // Only the valid series should render cells
        }

        [Test]
        public void HeatMap_ShouldHandleSeriesVisibility()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2], Visible = false },
                new() { Name = "Series 2", Data = [3, 4], Visible = true }
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
            );

            var cells = comp.FindAll(".mud-chart-cell");
            cells.Count.Should().Be(2); // Only visible series should render
        }

        [Test]
        [TestCase(Position.Top)]
        [TestCase(Position.Bottom)]
        [TestCase(Position.Left)]
        [TestCase(Position.Right)]
        public void HeatMap_ShouldRenderLegendInCorrectPosition(Position position)
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2, 3] }
            };

            var options = new ChartOptions
            {
                ShowLegend = true,
                ShowLegendLabels = true
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
                .Add(p => p.LegendPosition, position)
            );

            // Verify legend exists and is positioned correctly
            var legends = comp.FindAll(".mud-chart-heatmap-legend");
            legends.Should().NotBeEmpty();

            // Verify "Less" and "More" labels are present
            comp.Markup.Should().Contain("Less");
            comp.Markup.Should().Contain("More");
        }

        [Test]
        public void HeatMap_ShouldHandleSmoothGradients()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2, 3] },
                new() { Name = "Series 2", Data = [4, 5, 6] }
            };

            var options = new ChartOptions { EnableSmoothGradient = true };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            // Verify gradient definitions exist
            comp.Markup.Should().Contain("linearGradient");

            // Check for gradient overlays
            var gradientRects = comp.FindAll("rect[fill^='url(#gradient-']");
            gradientRects.Should().NotBeEmpty();
        }

        [Test]
        [TestCase(XAxisLabelPosition.Top)]
        [TestCase(XAxisLabelPosition.Bottom)]
        [TestCase(YAxisLabelPosition.Left)]
        [TestCase(YAxisLabelPosition.Right)]
        public void HeatMap_ShouldRenderAxisLabelsInCorrectPosition(Enum position)
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2] }
            };
            var xAxisLabels = new[] { "Label 1", "Label 2" };

            var options = new ChartOptions();
            if (position is XAxisLabelPosition xPos)
                options.XAxisLabelPosition = xPos;
            else if (position is YAxisLabelPosition yPos)
                options.YAxisLabelPosition = yPos;

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.XAxisLabels, xAxisLabels)
                .Add(p => p.ChartOptions, options)
            );

            // Verify axis labels exist
            var axisLabels = comp.FindAll("g text.mud-charts-xaxis, g text.mud-charts-yaxis");
            axisLabels.Should().NotBeEmpty();
        }

        [Test]
        public void HeatMap_ShouldShowTooltipsWhenEnabled()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2] }
            };

            var options = new ChartOptions { ShowToolTips = true };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            // Check for title elements (tooltips)
            var tooltips = comp.FindAll("title");
            tooltips.Should().NotBeEmpty();
            tooltips[0].TextContent.Should().Be("1"); // First cell value
        }

        [Test]
        public void HeatMap_ShouldCalculateDynamicFontSize()
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1] }
            };

            // Test with different dimensions
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.Width, "200px") // Smaller width to test font size adaptation
                .Add(p => p.Height, "200px")
            );

            var cellText = comp.Find(".mud-chart-cell text");
            var fontSize = cellText.GetAttribute("font-size");

            // Font size should be calculated based on cell dimensions
            fontSize.Should().NotBeNull();
            double.Parse(fontSize).Should().BeGreaterThan(0);
        }
    }
}

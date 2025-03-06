// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.UnitTests.TestComponents.Charts;
using MudBlazor.Utilities;
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
            comp.FindAll("path.mud-chart-serie")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");
            comp.FindAll("path.mud-chart-serie")[3].Click();
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

        [Test]
        public void HeatMap_ShouldGenerateCorrectColorPaletteForDifferentInputs()
        {
            // Single color palette
            var singleColorOptions = new ChartOptions { ChartPalette = ["#587934"] };
            var singleColorComp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartOptions, singleColorOptions)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = [1, 2, 3] }
                ])
            );

            var singleColorPalette = singleColorComp.Instance.ChartOptions.ChartPalette;
            singleColorPalette.Should().HaveCount(1);
            singleColorPalette.Should().AllBeOfType<string>();

            // Multi-color palette
            var multiColorOptions = new ChartOptions { ChartPalette = ["#587934", "#FF0000", "#00FF00"] };
            var multiColorComp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartOptions, multiColorOptions)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = [1, 2, 3] }
                ])
            );

            var multiColorPalette = multiColorComp.Instance.ChartOptions.ChartPalette;
            multiColorPalette.Should().HaveCount(3);
            multiColorPalette.Should().AllBeOfType<string>();
        }

        [Test]
        [TestCase(null, "")]
        [TestCase(0, "0")]
        [TestCase(1.23456, "1.234")]
        [TestCase(1000.123, "1000.")]
        public void HeatMap_ShouldFormatValuesCorrectly(double? input, string expected)
        {
            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = input.HasValue ? [input.Value] : [] }
            };

            var options = new ChartOptions { ValueFormatString = "G" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            var cellTexts = comp.FindAll(".mud-chart-cell text");

            if (input.HasValue)
            {
                cellTexts.Should().NotBeEmpty();
                cellTexts[0].TextContent.Trim().Should().Be(expected);
            }
            else
            {
                cellTexts.Should().BeEmpty();
            }
        }

        [Test]
        [SetCulture("en-US")]
        public void HeatMap_ShouldHandleCustomHeatMapCellOverrides()
        {
            static void CellFragment(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<MudHeatMapCell>(0);
                builder.AddAttribute(1, "Row", 0);
                builder.AddAttribute(2, "Column", 0);
                builder.AddAttribute(3, "Value", 10.05);
                builder.AddAttribute(6, "MudColor", new MudColor("#FF5733"));
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(childBuilder =>
                {
                    childBuilder.AddContent(0, "Custom Content");
                }));
                builder.CloseComponent();
            }

            var series = new List<ChartSeries>
            {
                new() { Name = "Series 1", Data = [1, 2, 3] },
                new() { Name = "Series 2", Data = [4, 5, 6] }
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .AddChildContent(CellFragment) // Add custom cells as child content
            );

            // Verify that the custom cell content is rendered
            var customContent = comp.Find(".mud-chart-cell div");
            customContent.TextContent.Trim().Should().Be("Custom Content");

            // Verify that the custom cell has the correct color
            var customCell = comp.Find(".mud-chart-cell rect");
            customCell.GetAttribute("fill").Should().Contain(new MudColor("#FF5733").ToString(MudColorOutputFormats.RGBA));

            // Verify custom value override
            var customValue = comp.Find(".mud-chart-cell title");
            customValue.TextContent.Trim().Should().Be("10.05");
        }

        [Test]
        public void MudHeatMapCell_ShouldThrowExceptionIfNotInMudChart()
        {
            // Attempt to render MudHeatMapCell outside of MudChart
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudHeatMapCell>(parameters => parameters
                    .Add(p => p.Row, 0)
                    .Add(p => p.Column, 0)
                )
            );

            // Verify that the exception message is appropriate
            exception.Message.Should().Contain("MudHeatMapCell must be used inside a MudChart component.");
        }

        [TestCase(Position.Top)]
        [TestCase(Position.Bottom)]
        [TestCase(Position.Left)]
        [TestCase(Position.Right)]
        [TestCase(Position.Start)]
        [TestCase(Position.End)]
        [TestCase(Position.Center)]
        [Test]
        public void HeatMap_ShouldCorrectBadPositions(Position pos)
        {
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.LegendPosition, pos)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = [1, 2, 3] }
                ])
            );

            var heatMap = comp.FindComponent<HeatMap>();
            heatMap.Instance._legendPosition.Should().BeOneOf(Position.Top, Position.Bottom, Position.Left, Position.Right);
        }

    }
}

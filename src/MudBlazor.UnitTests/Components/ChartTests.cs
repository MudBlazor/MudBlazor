// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AngleSharp.Dom;
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

            //check tooltip
            comp.FindAll("path.mud-chart-bar")[0].MouseOver();
            comp.Find("tspan").InnerHtml.Trim().Should().Be("40");

            // now click something and see that the selected index changes:
            comp.FindAll("path.mud-chart-bar")[0].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 0");

            comp.FindAll("path.mud-chart-bar")[10].MouseOver();
            comp.Find("tspan").InnerHtml.Trim().Should().Be("24");

            comp.FindAll("path.mud-chart-bar")[10].Click();
            comp.Find("h6").InnerHtml.Trim().Should().Be("Selected portion of the chart: 1");
        }

        [Test]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.StackedBar)]
        public async Task ChartYAxisFormat(ChartType chartType)
        {
            DefaultAxisChartOptions options = chartType switch
            {
                ChartType.Bar => new BarChartOptions(),
                ChartType.Line => new LineChartOptions(),
                ChartType.StackedBar => new StackedBarChartOptions(),
                _ => throw new NotImplementedException()
            };

            var series = new List<ChartSeries<double>>()
            {
                new() { Name = "Series 1", Data = new([90, 79, 72, 69, 62, 62, 55, 65, 70]) },
                new() { Name = "Series 2", Data = new([10, 41, 35, 51, 49, 62, 69, 91, 148]) },
            };
            var xAxis = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
            var width = "100%";
            var height = "350px";

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, chartType)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartLabels, xAxis)
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
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartType, chartType)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartLabels, xAxis)
                .Add(p => p.ChartOptions, options)
                .Add(p => p.Width, width)
                .Add(p => p.Height, height)
            );
            yaxis = comp.FindAll("g.mud-charts-yaxis");
            yaxis.Should().NotBeNull();
            yaxis[0].Children[0].InnerHtml.Trim().Should().Be($"{0:c2}");

            //number format
            options.YAxisFormat = "n6";
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartType, chartType)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartLabels, xAxis)
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
            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2, 3]) },
                new() { Name = "Series 2", Data = new([4, 5, 6]) }
            };
            var options = new HeatMapChartOptions { ShowLegend = true, ShowLegendLabels = true };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2, 3]) },
                new() { Name = "Series 2", Data = new([4, 5, 6]) }
            };
            var options = new ChartOptions() { ShowLegend = true };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1.176, 2, 3]) },
                new() { Name = "Series 2", Data = new([4.152, 5, 6]) }
            };

            var options = new HeatMapChartOptions() { ValueFormatString = "F2" };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Empty Series", Data = [] },
                new() { Name = "Null Series", Data = null },
                new() { Name = "Valid Series", Data = new([1.0, 2.0]) }
            };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2]), Visible = false },
                new() { Name = "Series 2", Data = new([3, 4]), Visible = true }
            };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2, 3]) }
            };

            var options = new HeatMapChartOptions
            {
                ShowLegend = true,
                ShowLegendLabels = true
            };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2, 3]) },
                new() { Name = "Series 2", Data = new([4, 5, 6]) }
            };

            var options = new HeatMapChartOptions { EnableSmoothGradient = true };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2]) }
            };
            var xAxisLabels = new[] { "Label 1", "Label 2" };

            var options = new HeatMapChartOptions();
            if (position is XAxisLabelPosition xPos)
                options.XAxisLabelPosition = xPos;
            else if (position is YAxisLabelPosition yPos)
                options.YAxisLabelPosition = yPos;

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, options)
            );

            // Verify axis labels exist
            var axisLabels = comp.FindAll("g text.mud-charts-xaxis, g text.mud-charts-yaxis");
            axisLabels.Should().NotBeEmpty();
        }

        [Test]
        public void HeatMap_ShouldShowTooltipsWhenEnabled()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2]) }
            };

            var options = new ChartOptions { ShowToolTips = true };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)
            );

            comp.FindAll("rect[fill]")[0].MouseOver();
            comp.Find("tspan").InnerHtml.Trim().Should().Be("1");
        }

        [Test]
        public void HeatMap_ShouldCalculateDynamicFontSize()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1]) }
            };

            // Test with different dimensions
            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
            var singleColorComp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartOptions, singleColorOptions)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = new([1, 2, 3]) }
                ])
            );

            var singleColorPalette = singleColorComp.Instance.ChartOptions.ChartPalette;
            singleColorPalette.Should().HaveCount(1);
            singleColorPalette.Should().AllBeOfType<string>();

            // Multi-color palette
            var multiColorOptions = new ChartOptions { ChartPalette = ["#587934", "#FF0000", "#00FF00"] };
            var multiColorComp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartOptions, multiColorOptions)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = new([1, 2, 3]) }
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
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = input.HasValue ? new([input.Value]) : [] }
            };

            var options = new HeatMapChartOptions { ValueFormatString = "G" };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
                builder.OpenComponent<MudHeatMapCell<double>>(0);
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

            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = new([1, 2, 3]) },
                new() { Name = "Series 2", Data = new([4, 5, 6]) }
            };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ChartOptions() { ShowToolTips = true })
                .AddChildContent(CellFragment) // Add custom cells as child content
            );

            // Verify that the custom cell content is rendered
            var customContent = comp.Find(".mud-chart-cell div");
            customContent.TextContent.Trim().Should().Be("Custom Content");

            // Verify that the custom cell has the correct color
            var customCell = comp.Find(".mud-chart-cell rect");
            customCell.GetAttribute("fill").Should().Contain(new MudColor("#FF5733").ToString(MudColorOutputFormats.RGBA));

            // Verify custom value override
            customCell.MouseOver(); //value is shown via the tooltip
            comp.Find("tspan").InnerHtml.Trim().Should().Be("10.05");
        }

        [Test]
        public void MudHeatMapCell_ShouldThrowExceptionIfNotInMudChart()
        {
            // Attempt to render MudHeatMapCell outside of MudChart
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudHeatMapCell<double>>(parameters => parameters
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
            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.LegendPosition, pos)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = new([1, 2, 3]) }
                ])
            );

            var heatMap = comp.FindComponent<HeatMap<double>>();
            heatMap.Instance._legendPosition.Should().BeOneOf(Position.Top, Position.Bottom, Position.Left, Position.Right);
        }

        [TestCase(null, null)]
        [TestCase(0, 100)]
        [TestCase(0, .95)]
        public void HeatMap_Override_Min_Max(double? min, double? max)
        {
            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>
                {
                    new() { Name = "Series 1", Data = new([-0.5, .5, .98]) }
                })
                .Add(p => p.ChildContent, (RenderFragment)(builder =>
                {
                    builder.OpenComponent<MudHeatMapCell<double>>(0);
                    builder.AddAttribute(1, "Row", 0);
                    builder.AddAttribute(2, "Column", 0);
                    builder.AddAttribute(3, "MinValue", min);
                    builder.AddAttribute(4, "MaxValue", max);
                    builder.CloseComponent();
                }))
            );
            var heatmap = comp.FindComponent<HeatMap<double>>();
            heatmap.Instance._minValue.Should().Be(min.HasValue ? min : -0.5);
            heatmap.Instance._maxValue.Should().Be(max.HasValue ? max : .98);
        }

        [TestCase(ChartType.Donut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.StackedBar)]
        [TestCase(ChartType.HeatMap)]
        [TestCase(ChartType.Timeseries)]
        [Test]
        public void NoLabel_Chart_IsValid(ChartType chart)
        {
            var series = new List<ChartSeries<double>>()
            {
                new() { Name = "Series 1", Data = new([90, 79, 72, 69, 62, 62, 55, 65, 70]) },
                new() { Name = "Series 2", Data = new([10, 41, 35, 51, 49, 62, 69, 91, 148]) },
            };

            IChartOptions options = new ChartOptions();

            if (chart == ChartType.Line)
                options = new LineChartOptions() { InterpolationOption = InterpolationOption.Periodic };
            else if (chart == ChartType.Timeseries)
                options = new TimeSeriesChartOptions() { InterpolationOption = InterpolationOption.Periodic };

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                              .Add(p => p.ChartType, chart)
                              .Add(p => p.ChartOptions, options)
                              .Add(p => p.ChartSeries, series));

        }

        [Test]
        public void HeatmapChart_CanHideSeries_Test()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Sensor Alpha", Data = new double[] { 10, 20, 30, 25 } }, // Row 1
                new () { Name = "Sensor Beta", Data = new double[] { 15, 25, 35, 20 } },  // Row 2
                new () { Name = "Sensor Gamma", Data = new double[] { 5, 10, 15, 12 }, Visible = false } // Row 3, initially hidden
            };
            string[] xAxisLabels = { "Time 1", "Time 2", "Time 3", "Time 4" }; // Columns

            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.HeatMap)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "400px")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels) // X-axis labels for columns
                .Add(p => p.CanHideSeries, true)
            );

            // Initial state assertions for checkboxes
            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartSeries.Count, "Number of checkboxes should match number of series");

            seriesCheckboxes[0].IsChecked().Should().BeTrue("Sensor Alpha checkbox should be initially checked");
            seriesCheckboxes[1].IsChecked().Should().BeTrue("Sensor Beta checkbox should be initially checked");
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Sensor Gamma checkbox should be initially unchecked");

            // Initial state assertions for heatmap cells (rects)
            comp.FindAll(".mud-chart-cell").Count.Should().Be(chartSeries.Where(x => x.Visible).Sum(x => x.Data.Count()), "should equal count of visible cells");

            // Hide Sensor Alpha series
            comp.InvokeAsync(() => seriesCheckboxes[0].Change(false));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Sensor Alpha checkbox should be unchecked after hiding");
            chartSeries[0].Visible.Should().BeFalse("Sensor Alpha Visible property should be false after hiding");
            comp.FindAll(".mud-chart-cell").Count.Should().Be(chartSeries.Where(x => x.Visible).Sum(x => x.Data.Count()), "should equal count of visible cells");

            // Show Sensor Alpha series again
            comp.InvokeAsync(() => seriesCheckboxes[0].Change(true));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Sensor Alpha checkbox should be checked after re-showing");
            chartSeries[0].Visible.Should().BeTrue("Sensor Alpha Visible property should be true after re-showing");
            comp.FindAll(".mud-chart-cell").Count.Should().Be(chartSeries.Where(x => x.Visible).Sum(x => x.Data.Count()), "should equal count of visible cells");

            // Show Sensor Gamma series (initially hidden)
            comp.InvokeAsync(() => seriesCheckboxes[2].Change(true));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Sensor Gamma checkbox should be checked after showing");
            chartSeries[2].Visible.Should().BeTrue("Sensor Gamma Visible property should be true after showing");
            comp.FindAll(".mud-chart-cell").Count.Should().Be(chartSeries.Where(x => x.Visible).Sum(x => x.Data.Count()), "should equal count of visible cells");

            // Hide Sensor Gamma series again
            comp.InvokeAsync(() => seriesCheckboxes[2].Change(false));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Sensor Gamma checkbox should be unchecked after hiding again");
            chartSeries[2].Visible.Should().BeFalse("Sensor Gamma Visible property should be false after hiding again");
            comp.FindAll(".mud-chart-cell").Count.Should().Be(chartSeries.Where(x => x.Visible).Sum(x => x.Data.Count()), "should equal count of visible cells");
        }

        public record YAxisTestCase(Func<double, string> YAxisToStringFunc, string ExpectedValue);

        private const double YAxisTestValue = 20;

        private static IEnumerable<YAxisTestCase> YAxisFuncs()
        {
            yield return new YAxisTestCase(x => "hardcoded", "hardcoded");
            yield return new YAxisTestCase(x => $"{x}/tCO2e", "20/tCO2e");
            yield return new YAxisTestCase(x => x.ToString("0.00", CultureInfo.InvariantCulture), "20.00");
            yield return new YAxisTestCase(null!, "20");
        }

        [Test, TestCaseSource(nameof(YAxisFuncs))]
        [SetCulture("en-US")]
        public void YAxisToStringFuncTest(YAxisTestCase testCase)
        {
            var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartLabels, [""])
                .Add(p => p.ChartOptions, new LineChartOptions() { YAxisToStringFunc = testCase.YAxisToStringFunc })
                .Add(p => p.ChartSeries, [new() { Data = new([YAxisTestValue]) }])
            );

            var yaxis = comp.FindAll("g.mud-charts-yaxis");
            yaxis.Should().NotBeNull();
            yaxis[0].Children[0].InnerHtml.Trim().Should().Be(testCase.ExpectedValue);
        }
    }
}

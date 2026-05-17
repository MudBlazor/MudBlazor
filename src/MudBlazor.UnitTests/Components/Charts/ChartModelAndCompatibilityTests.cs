// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class ChartModelAndCompatibilityTests : BunitTest
{
    [Test]
    public void ChartData_TimeSeriesConstructorsAndImplicitOperatorsPreservePoints()
    {
        var start = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        AssertTimeSeriesPoints(new ChartData<int>(start, 1), [(start, 1)]);
        AssertTimeSeriesPoints(new ChartData<int>((start.AddMinutes(5), 2)), [(start.AddMinutes(5), 2)]);
        AssertTimeSeriesPoints(new ChartData<int>([(start.AddMinutes(10), 3), (start.AddMinutes(15), 4)]), [(start.AddMinutes(10), 3), (start.AddMinutes(15), 4)]);

        ChartData<int> timeValue = new TimeValue<int>(start.AddMinutes(20), 5);
        AssertTimeSeriesPoints(timeValue, [(start.AddMinutes(20), 5)]);

        ChartData<int> timeValues = new[] { new TimeValue<int>(start.AddMinutes(25), 6), new TimeValue<int>(start.AddMinutes(30), 7) };
        AssertTimeSeriesPoints(timeValues, [(start.AddMinutes(25), 6), (start.AddMinutes(30), 7)]);
    }

    [Test]
    public void ChartData_SankeyConstructorsAndImplicitOperatorsPreserveLinks()
    {
        AssertSankeyPoints(new ChartData<int>(new SankeyLink("Start", "Middle"), 1), [("Start", "Middle", 1)]);
        AssertSankeyPoints(new ChartData<int>(("Middle", "End", 2)), [("Middle", "End", 2)]);
        AssertSankeyPoints(new ChartData<int>([(new SankeyLink("A", "B"), 3), (new SankeyLink("B", "C"), 4)]), [("A", "B", 3), ("B", "C", 4)]);
        AssertSankeyPoints(new ChartData<int>([("C", "D", 5), ("D", "E", 6)]), [("C", "D", 5), ("D", "E", 6)]);

        ChartData<int> sankeyEdge = new SankeyEdge<int>("E", "F", 7);
        AssertSankeyPoints(sankeyEdge, [("E", "F", 7)]);

        ChartData<int> sankeyEdges = new[] { new SankeyEdge<int>("F", "G", 8), new SankeyEdge<int>("G", "H", 9) };
        AssertSankeyPoints(sankeyEdges, [("F", "G", 8), ("G", "H", 9)]);
    }

    [Test]
    public void ChartSeries_EqualsUsesNameAndValues()
    {
        var baseline = new ChartSeries<int> { Name = "Sales", Data = new[] { 1, 2, 3 } };
        var same = new ChartSeries<int> { Name = "Sales", Data = new[] { 1, 2, 3 } };

        baseline.Equals((ChartSeries<int>)null).Should().BeFalse();
        baseline.Equals(new object()).Should().BeFalse();
        baseline.Equals(baseline).Should().BeTrue();

        baseline.Equals(same).Should().BeTrue();
        baseline.GetHashCode().Should().Be(same.GetHashCode());

        baseline.Equals(new ChartSeries<int> { Name = "Revenue", Data = new[] { 1, 2, 3 } }).Should().BeFalse();
        baseline.Equals(new ChartSeries<int> { Name = "Sales", Data = new[] { 1, 2 } }).Should().BeFalse();
        baseline.Equals(new ChartSeries<int> { Name = "Sales", Data = new[] { 3, 2, 1 } }).Should().BeFalse();
    }

    [Test]
    [TestCase(ChartType.Timeseries, typeof(TimeSeriesChartOptions))]
    [TestCase(ChartType.Rose, typeof(RoseChartOptions))]
    [TestCase(ChartType.Radar, typeof(RadarChartOptions))]
    [TestCase(ChartType.Sankey, typeof(SankeyChartOptions))]
    [TestCase(ChartType.ScatterPlot, typeof(ScatterPlotChartOptions))]
    public void MudChart_ConvertsLegacyChartOptionsForSpecializedCharts(ChartType chartType, Type expectedOptionsType)
    {
        var legacyOptions = new ChartOptions
        {
            ShowLegend = false,
            ShowToolTips = false,
            TooltipTitleFormat = "Title",
            TooltipSubtitleFormat = "Subtitle",
            ChartPalette = ["#123456", "#abcdef"]
        };

        var resolvedOptions = RenderChartAndGetOptions(chartType, legacyOptions);

        resolvedOptions.Should().BeOfType(expectedOptionsType);
        resolvedOptions.ShowLegend.Should().BeFalse();
        resolvedOptions.ShowToolTips.Should().BeFalse();
        resolvedOptions.TooltipTitleFormat.Should().Be("Title");
        resolvedOptions.TooltipSubtitleFormat.Should().Be("Subtitle");
        resolvedOptions.ChartPalette.Should().Equal("#123456", "#abcdef");
    }

    [Test]
    [TestCase(ChartType.Timeseries, typeof(TimeSeriesChartOptions))]
    [TestCase(ChartType.Rose, typeof(RoseChartOptions))]
    [TestCase(ChartType.Radar, typeof(RadarChartOptions))]
    [TestCase(ChartType.Sankey, typeof(SankeyChartOptions))]
    [TestCase(ChartType.ScatterPlot, typeof(ScatterPlotChartOptions))]
    public void MudChart_UsesDefaultOptionsForSpecializedChartsWhenNoneAreProvided(ChartType chartType, Type expectedOptionsType)
    {
        var resolvedOptions = RenderChartAndGetOptions(chartType);

        resolvedOptions.Should().BeOfType(expectedOptionsType);
    }

    private IChartOptions RenderChartAndGetOptions(ChartType chartType, ChartOptions options = null)
    {
        var chart = chartType switch
        {
            ChartType.Timeseries => RenderChart(chartType, GetTimeSeriesData(), options),
            ChartType.Rose => RenderChart(chartType, GetRadialSeriesData(), options, chartLabels: ["North", "East", "South"]),
            ChartType.Radar => RenderChart(chartType, GetRadialSeriesData(), options, chartLabels: ["North", "East", "South"]),
            ChartType.Sankey => RenderChart(chartType, GetSankeySeriesData(), options),
            ChartType.ScatterPlot => RenderChart(chartType, GetScatterSeriesData(), options),
            _ => throw new AssertionException($"Unsupported chart type {chartType}")
        };

        return chartType switch
        {
            ChartType.Timeseries => chart.FindComponent<TimeSeries<double>>().Instance.ChartOptions!,
            ChartType.Rose => chart.FindComponent<Rose<double>>().Instance.ChartOptions!,
            ChartType.Radar => chart.FindComponent<Radar<double>>().Instance.ChartOptions!,
            ChartType.Sankey => chart.FindComponent<Sankey<double>>().Instance.ChartOptions!,
            ChartType.ScatterPlot => chart.FindComponent<ScatterPlot<double>>().Instance.ChartOptions!,
            _ => throw new AssertionException($"Unsupported chart type {chartType}")
        };
    }

    private IRenderedComponent<MudChart<double>> RenderChart(
        ChartType chartType,
        List<ChartSeries<double>> chartSeries,
        ChartOptions options = null,
        string[] chartLabels = null)
    {
        return Context.Render<MudChart<double>>(parameters =>
        {
            parameters.Add(p => p.ChartType, chartType);
            parameters.Add(p => p.ChartSeries, chartSeries);

            if (chartLabels is not null)
            {
                parameters.Add(p => p.ChartLabels, chartLabels);
            }

            if (options is not null)
            {
                parameters.Add(p => p.ChartOptions, options);
            }
        });
    }

    private static List<ChartSeries<double>> GetTimeSeriesData()
    {
        var start = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        return
        [
            new ChartSeries<double>
            {
                Name = "Series",
                Data = new[] { new TimeValue<double>(start, 1), new TimeValue<double>(start.AddHours(1), 2) }
            }
        ];
    }

    private static List<ChartSeries<double>> GetRadialSeriesData() =>
    [
        new ChartSeries<double> { Name = "Series", Data = new[] { 10d, 20d, 30d } }
    ];

    private static List<ChartSeries<double>> GetSankeySeriesData() =>
    [
        new ChartSeries<double>
        {
            Name = "Series",
            Data = new[] { new SankeyEdge<double>("Start", "Middle", 1), new SankeyEdge<double>("Middle", "End", 2) }
        }
    ];

    private static List<ChartSeries<double>> GetScatterSeriesData() =>
    [
        new ChartSeries<double>
        {
            Name = "Series",
            Data = new[] { (1d, 2d), (3d, 4d) }
        }
    ];

    private static void AssertTimeSeriesPoints(ChartData<int> data, IReadOnlyList<(DateTime X, int Y)> expectedPoints)
    {
        data.Points.Select(point => ((DateTime)point.X!, point.Y)).Should().Equal(expectedPoints);
    }

    private static void AssertSankeyPoints(ChartData<int> data, IReadOnlyList<(string Source, string Target, int Y)> expectedPoints)
    {
        data.Points
            .Select(point =>
            {
                var link = (SankeyLink)point.X!;
                return (link.Source, link.Target, point.Y);
            })
            .Should()
            .Equal(expectedPoints);
    }
}

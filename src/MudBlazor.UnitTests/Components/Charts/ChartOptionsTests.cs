// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class ChartOptionsTests
{
    [Test]
    public void SankeyChartOptions_Defaults_AreExpected()
    {
        var options = new SankeyChartOptions();

        // Inherited from DefaultChartOptions
        options.ShowLegend.Should().BeTrue();
        options.ShowToolTips.Should().BeTrue();
        options.ChartPalette.Should().HaveCount(20);

        // SankeyChartOptions-specific
        options.NodeOverrides.Should().BeEmpty();
        options.NodeWidth.Should().Be(10);
        options.MinVerticalSpacing.Should().Be(12);
        options.EdgeOpacity.Should().Be(0.5);
        options.ShowLabels.Should().BeTrue();
        options.ShowNodeValues.Should().BeTrue();
        options.LabelFontSize.Should().Be("0.75rem");
        options.LabelPadding.Should().Be(5);
        options.ShowEdgeLabels.Should().BeFalse();
        options.HighlightOnHover.Should().BeTrue();
        options.HighlightColor.Should().Be("var(--mud-palette-text-primary)");
        options.AggregationOption.Should().Be(AggregationOption.None);
        options.HideNodesSmallerThan.Should().Be(0);
        options.HideNodesWithNoEdges.Should().BeFalse();
        options.EdgeLabelSymbol.Should().Be("⇒");
        options.OrderNodesByValue.Should().BeFalse();
    }

    [Test]
    public void SankeyChartOptions_ImplicitConversion_CopiesBaseChartOptions()
    {
        var source = new ChartOptions
        {
            ShowLegend = false,
            ShowToolTips = false,
            TooltipTitleFormat = "title",
            TooltipSubtitleFormat = "subtitle",
            ChartPalette = ["#FF0000"],
        };

        SankeyChartOptions result = source;

        result.ShowLegend.Should().BeFalse();
        result.ShowToolTips.Should().BeFalse();
        result.TooltipTitleFormat.Should().Be("title");
        result.TooltipSubtitleFormat.Should().Be("subtitle");
        result.ChartPalette.Should().BeEquivalentTo(["#FF0000"]);
    }

    [Test]
    public void TimeSeriesChartOptions_Defaults_AreExpected()
    {
        var options = new TimeSeriesChartOptions();

        // Inherited from DefaultAxisLineChartOptions
        options.LineStrokeWidth.Should().Be(3);
        options.ShowDataMarkers.Should().BeFalse();
        options.YAxisRequireZeroPoint.Should().BeFalse();
        options.ClampToZero.Should().BeFalse();
        options.LineDisplayType.Should().Be(LineDisplayType.Line);
        options.InterpolationOption.Should().Be(InterpolationOption.Straight);

        // Inherited from DefaultAxisChartOptions
        options.YAxisTicks.Should().Be(20);
        options.MaxNumYAxisTicks.Should().Be(20);
        options.YAxisLines.Should().BeTrue();
        options.XAxisLines.Should().BeFalse();

        // TimeSeriesChartOptions-specific
        options.TimeLabelFormat.Should().Be("HH:mm");
        options.TimeLabelSpacing.Should().Be(TimeSpan.FromMinutes(5));
        options.TimeLabelSpacingRounding.Should().BeFalse();
        options.TimeLabelSpacingRoundingPadSeries.Should().BeFalse();
        options.TooltipTimeLabelFormat.Should().Be("HH:mm");
        options.TooltipTitleFormat.Should().Be("{{X_VALUE}}");
        options.TooltipSubtitleFormat.Should().Be("{{Y_VALUE}}");
    }

    [Test]
    public void TimeSeriesChartOptions_ImplicitConversion_CopiesBaseChartOptions()
    {
        var source = new ChartOptions
        {
            ShowLegend = false,
            ShowToolTips = false,
            TooltipTitleFormat = "title",
            TooltipSubtitleFormat = "subtitle",
            ChartPalette = ["#00FF00"],
        };

        TimeSeriesChartOptions result = source;

        result.ShowLegend.Should().BeFalse();
        result.ShowToolTips.Should().BeFalse();
        result.TooltipTitleFormat.Should().Be("title");
        result.TooltipSubtitleFormat.Should().Be("subtitle");
        result.ChartPalette.Should().BeEquivalentTo(["#00FF00"]);
    }

    [Test]
    public void RadarChartOptions_Defaults_AreExpected()
    {
        var options = new RadarChartOptions();

        // Inherited from DefaultRadialChartOptions
        options.ShowAsPercentage.Should().BeFalse();

        // Inherited from DefaultChartOptions
        options.ShowLegend.Should().BeTrue();
        options.ShowToolTips.Should().BeTrue();
        options.ChartPalette.Should().HaveCount(20);

        // RadarChartOptions-specific
        options.AngleOffset.Should().Be(0);
        options.ShowGridLines.Should().BeTrue();
        options.GridLineColor.Should().Be("var(--mud-palette-divider)");
        options.GridLineWidth.Should().Be(1.0);
        options.GridLevels.Should().Be(5);
        options.ShowAxisLabels.Should().BeTrue();
        options.ShowAxisValues.Should().BeTrue();
        options.AxisLineColor.Should().Be("var(--mud-palette-lines-inputs)");
        options.AxisLineWidth.Should().Be(1.0);
        options.FillOpacity.Should().Be(0.4);
        options.StrokeWidth.Should().Be(2.0);
        options.ShowDataMarkers.Should().BeTrue();
        options.DataPointRadius.Should().Be(3.0);
        options.AxisSuggestedMax.Should().BeNull();
        options.AxisFormat.Should().BeNull();
        options.AxisToStringFunc.Should().BeNull();
        options.AggregationOption.Should().Be(AggregationOption.GroupByDataSet);
    }

    [Test]
    public void RoseChartOptions_Defaults_AreExpected()
    {
        var options = new RoseChartOptions();

        // Inherited from DefaultRadialChartOptions
        options.AggregationOption.Should().Be(AggregationOption.GroupByLabel);
        options.FillOpacity.Should().Be(1);
        options.ShowAsPercentage.Should().BeFalse();

        // Inherited from DefaultChartOptions
        options.ShowLegend.Should().BeTrue();
        options.ShowToolTips.Should().BeTrue();
        options.ChartPalette.Should().HaveCount(20);

        // RoseChartOptions-specific
        options.AngleOffset.Should().Be(0);
        options.ScaleFactor.Should().Be(0.9);
        options.ShowValues.Should().BeFalse();
    }
}

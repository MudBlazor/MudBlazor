// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Justification.BarGroup;
using MudBlazor.Justification.StackedBars;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class ChartJustificationStrategyTests
{
    [TestCase(Justify.FlexStart, typeof(MudBlazor.Justification.BarGroup.FlexStartStrategy))]
    [TestCase(Justify.FlexEnd, typeof(MudBlazor.Justification.BarGroup.FlexEndStrategy))]
    [TestCase(Justify.Center, typeof(MudBlazor.Justification.BarGroup.CenterStrategy))]
    [TestCase(Justify.SpaceBetween, typeof(MudBlazor.Justification.BarGroup.SpaceBetweenStrategy))]
    [TestCase(Justify.SpaceAround, typeof(MudBlazor.Justification.BarGroup.SpaceAroundStrategy))]
    [TestCase(Justify.SpaceEvenly, typeof(MudBlazor.Justification.BarGroup.SpaceEvenlyStrategy))]
    public void BarGroupStrategyFactory_ShouldReturnExpectedStrategyType(Justify justify, Type expectedType)
    {
        var strategy = BarGroupStrategyFactory.GetStrategy(justify);

        strategy.Should().BeOfType(expectedType);
    }

    [Test]
    public void BarGroupStrategyFactory_ShouldThrowForUnsupportedJustification()
    {
        var action = () => BarGroupStrategyFactory.GetStrategy((Justify)int.MaxValue);

        action.Should().Throw<NotSupportedException>().WithMessage("Unsupported Justification*");
    }

    [Test]
    public void BarGroupStrategies_ShouldCalculateExpectedPositions_ForCommonJustifications()
    {
        var context = CreateBarGroupContext();

        BarGroupStrategyFactory.GetStrategy(Justify.FlexStart).CalculatePositions(context).Should().Equal(new[] { 17d, 53d, 89d });
        BarGroupStrategyFactory.GetStrategy(Justify.Center).CalculatePositions(context).Should().Equal(new[] { 21d, 57d, 93d });
        BarGroupStrategyFactory.GetStrategy(Justify.SpaceBetween).CalculatePositions(context).Should().Equal(new[] { 17d, 65d, 113d });
        BarGroupStrategyFactory.GetStrategy(Justify.SpaceAround).CalculatePositions(context).Should().Equal(new[] { 20d, 60d, 100d });
        BarGroupStrategyFactory.GetStrategy(Justify.SpaceEvenly).CalculatePositions(context).Should().Equal(new[] { 29d, 65d, 101d });
    }

    [TestCase(2, new[] { 27d, 61.09375d, 95.1875d })]
    [TestCase(3, new[] { -3d, 45.09375d, 93.1875d })]
    [TestCase(4, new[] { -38d, 24.09375d, 86.1875d })]
    public void BarGroupFlexEndStrategy_ShouldCoverDataSetCountBranches(int dataSetCount, double[] expectedPositions)
    {
        var context = CreateBarGroupContext(dataSetCount);

        var positions = BarGroupStrategyFactory.GetStrategy(Justify.FlexEnd).CalculatePositions(context);

        positions.Should().Equal(expectedPositions);
    }

    [Test]
    public void BarGroupSpaceBetweenStrategy_ShouldCenterSingleColumn()
    {
        var context = CreateBarGroupContext(columnsPerDataSet: 1);

        var positions = BarGroupStrategyFactory.GetStrategy(Justify.SpaceBetween).CalculatePositions(context);

        positions.Should().Equal(new[] { 65d });
    }

    [Test]
    public void BarGroupSpaceAroundStrategy_ShouldOffsetSingleDataSetDifferentlyThanMultiple()
    {
        var singleDataSetPositions = BarGroupStrategyFactory.GetStrategy(Justify.SpaceAround).CalculatePositions(CreateBarGroupContext(dataSetCount: 1));
        var multipleDataSetPositions = BarGroupStrategyFactory.GetStrategy(Justify.SpaceAround).CalculatePositions(CreateBarGroupContext(dataSetCount: 2));

        singleDataSetPositions.Should().Equal(new[] { 25d, 65d, 105d });
        multipleDataSetPositions.Should().Equal(new[] { 20d, 60d, 100d });
    }

    [TestCase(Justify.FlexStart, typeof(MudBlazor.Justification.StackedBars.FlexStartStrategy))]
    [TestCase(Justify.FlexEnd, typeof(MudBlazor.Justification.StackedBars.FlexEndStrategy))]
    [TestCase(Justify.Center, typeof(MudBlazor.Justification.StackedBars.CenterStrategy))]
    [TestCase(Justify.SpaceBetween, typeof(MudBlazor.Justification.StackedBars.SpaceBetweenStrategy))]
    [TestCase(Justify.SpaceAround, typeof(MudBlazor.Justification.StackedBars.SpaceAroundStrategy))]
    [TestCase(Justify.SpaceEvenly, typeof(MudBlazor.Justification.StackedBars.SpaceEvenlyStrategy))]
    public void StackedBarStrategyFactory_ShouldReturnExpectedStrategyType(Justify justify, Type expectedType)
    {
        var strategy = StackedBarStrategyFactory.GetStrategy(justify);

        strategy.Should().BeOfType(expectedType);
    }

    [Test]
    public void StackedBarStrategyFactory_ShouldThrowForUnsupportedJustification()
    {
        var action = () => StackedBarStrategyFactory.GetStrategy((Justify)int.MaxValue);

        action.Should().Throw<NotSupportedException>().WithMessage("Unsupported Justification*");
    }

    [Test]
    public void StackedBarStrategies_ShouldCalculateExpectedPositions_ForCommonJustifications()
    {
        var context = CreateStackedBarContext();

        StackedBarStrategyFactory.GetStrategy(Justify.FlexStart).CalculatePositions(context).Should().Equal(new[] { 10d, 28d, 46d });
        StackedBarStrategyFactory.GetStrategy(Justify.FlexEnd).CalculatePositions(context).Should().Equal(new[] { 75d, 93d, 111d });
        StackedBarStrategyFactory.GetStrategy(Justify.Center).CalculatePositions(context).Should().Equal(new[] { 40d, 58d, 76d });
        StackedBarStrategyFactory.GetStrategy(Justify.SpaceBetween).CalculatePositions(context).Should().Equal(new[] { 10d, 58d, 106d });
        StackedBarStrategyFactory.GetStrategy(Justify.SpaceAround).CalculatePositions(context).Should().Equal(new[] { 22d, 58d, 94d });
        StackedBarStrategyFactory.GetStrategy(Justify.SpaceEvenly).CalculatePositions(context).Should().Equal(new[] { 28d, 58d, 88d });
    }

    [Test]
    public void StackedBarSpaceBetweenStrategy_ShouldCenterSingleColumn()
    {
        var context = CreateStackedBarContext(maxColumns: 1);

        var positions = StackedBarStrategyFactory.GetStrategy(Justify.SpaceBetween).CalculatePositions(context);

        positions.Should().Equal(new[] { 58d });
    }

    private static BarGroupContext CreateBarGroupContext(int dataSetCount = 2, int columnsPerDataSet = 3)
    {
        const double barWidth = 10;
        const double barGap = 4;

        return new BarGroupContext
        {
            ColumnsPerDataSet = columnsPerDataSet,
            DataSetCount = dataSetCount,
            HorizontalSpace = 120,
            BarWidth = barWidth,
            BarGap = barGap,
            BarGroupWidth = (dataSetCount * barWidth) + (Math.Max(0, dataSetCount - 1) * barGap),
            HorizontalStartSpace = 5,
            HorizontalEndSpace = 7,
            SeriesSpacingRatio = 2,
            CalculateSpaceWidth = (_, _) => 8
        };
    }

    private static StackedBarContext CreateStackedBarContext(int maxColumns = 3)
    {
        return new StackedBarContext
        {
            MaxColumns = maxColumns,
            BarWidth = 12,
            SpaceBetweenBars = 6,
            HorizontalSpace = 108,
            HorizontalStartSpace = 4,
            HorizontalEndSpace = 9
        };
    }
}

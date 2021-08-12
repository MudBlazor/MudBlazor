#pragma warning disable BL0005 // Component parameter should not be set outside of its component.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Svg.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.EnhanceChart;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.Utilities;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.EnhancedChart
{
    [TestFixture]
    public class MudEnhancedChart_ToolTipFocused_Tests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void ChartInteraction_ActivatingSeries()
        {
            var series = new List<Double> { 100.0, 80.0, 20.0 };

            List<(String Name, MudColor Color)> expectedToolTips = new()
            {
                ("my 1 series", Colors.Green.Accent1),
                ("my 2 series", Colors.Green.Accent1),
                ("my 3 series", Colors.Green.Accent1),
            };

            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartToolTip, IEnumerable<ChartToolTipInfo>>(a => a.ToolTip, value => itemParams => itemParams.Add(o => o.ToolTips, value.OfType<BarChartToolTipInfo>()));
                pChart.Add<MudEnhancedBarChart>(a => a.Chart, p =>
                {
                    p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (pAxis) =>
                    {
                        pAxis.Add(y => y.Labels, new List<String> { "1", "2", "3" });
                    });
                    p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                    {
                        setP.Add(y => y.Name, "my 1 dataset");
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, expectedToolTips[0].Name);
                            seriesP.Add(z => z.Color, expectedToolTips[0].Color);
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, expectedToolTips[1].Name);
                            seriesP.Add(z => z.Color, expectedToolTips[1].Color);
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, expectedToolTips[2].Name);
                            seriesP.Add(z => z.Color, expectedToolTips[2].Color);
                            seriesP.Add(z => z.Points, series);
                        });
                    });
                });
            });

            var rects = comp.FindAll("polygon");

            rects.Should().HaveCount(9);

            for (int i = 0; i < rects.Count; i++)
            {
                Int32 seriesIndex = i % 3;
                Int32 labelIndex = i / 3;

                rects[i].MouseOver(new MouseEventArgs());

                var toolTipComponent = comp.FindComponent<MudEnhancedBarChartToolTip>();
                toolTipComponent.Instance.ToolTips.Should().HaveCount(1);

                var firstTip = toolTipComponent.Instance.ToolTips.First();
                firstTip.Should().NotBeNull();

                firstTip.Color.Should().Be(expectedToolTips[seriesIndex].Color);
                firstTip.SeriesName.Should().Be(expectedToolTips[seriesIndex].Name);
                firstTip.DataSetSeriesName.Should().Be("my 1 dataset");
                
                firstTip.Value.Should().Be(series[labelIndex]);
                firstTip.XLabel.Should().Be((labelIndex + 1).ToString());

                comp.InvokeAsync( () =>  rects[i].MouseOut(new MouseEventArgs()));
                
                Assert.Throws<Bunit.Rendering.ComponentNotFoundException>(() => comp.FindComponent<MudEnhancedBarChartToolTip>());
            }
        }
    }
}

#pragma warning restore BL0005 // Component parameter should not be set outside of its component.


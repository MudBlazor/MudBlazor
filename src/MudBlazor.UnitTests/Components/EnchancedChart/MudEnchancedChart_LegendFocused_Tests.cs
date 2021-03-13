#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using AngleSharp.Svg.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.Utilities;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.EnchancedChart
{
    [TestFixture]
    public class MudEnchancedChart_LegendFocused_Tests
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
        public void GetLegendFromSereries()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            List<ChartLegendInfoSeries> secondDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 3 series", Colors.Orange.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 4 series", Colors.Purple.Accent1.ToCssColor()),
            };

            var group2 = new ChartLegendInfoGroup("my 2 dataset", secondDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1, group2 });

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, Colors.Red.Accent1.ToCssColor());
                    });
                });
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 2 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 3 series");
                        seriesP.Add(z => z.Color, Colors.Orange.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 4 series");
                        seriesP.Add(z => z.Color, Colors.Purple.Accent1.ToCssColor());
                    });
                });
            });

            var acutalLegendInfo = comp.Instance.LegendInfo;

            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);
        }

        [Test]
        public void LegendGetUpdated_DataSetAdded()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            List<ChartLegendInfoSeries> secondDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 3 series", Colors.Orange.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 4 series", Colors.Purple.Accent1.ToCssColor()),
            };

            var group2 = new ChartLegendInfoGroup("my 2 dataset", secondDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1, group2 });
            Boolean isUnderTest = false;
            Boolean legendPassed = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, Colors.Red.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            BarDataSet dataSet = new BarDataSet { Name = "my 2 dataset" };
            dataSet.Add(new BarChartSeries { Name = "my 3 series", Color = Colors.Orange.Accent1.ToCssColor() });
            dataSet.Add(new BarChartSeries { Name = "my 4 series", Color = Colors.Purple.Accent1.ToCssColor() });

            comp.Instance.Add(dataSet);

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSetRemoved()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, Colors.Red.Accent1.ToCssColor());
                    });
                });
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 2 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 3 series");
                        seriesP.Add(z => z.Color, Colors.Orange.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 4 series");
                        seriesP.Add(z => z.Color, Colors.Purple.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            var secondDataSet = comp.FindComponents<BarDataSet>().Last();
            secondDataSet.Instance.Dispose();

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesAdded()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                    //setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    //{
                    //    seriesP.Add(z => z.Name, "");
                    //    seriesP.Add(z => z.Color, );
                    //});
                });
            });

            isUnderTest = true;

            var dataset = comp.FindComponent<BarDataSet>().Instance;
            dataset.Add(new BarChartSeries { Name = "my 2 series", Color = Colors.Red.Accent1.ToCssColor(), Dataset = dataset });

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesRemoved()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, Colors.Red.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            var secondSeries = comp.FindComponents<BarChartSeries>().Last();
            secondSeries.Instance.Dispose();

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSetNameChanged()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset changed", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            comp.FindComponent<BarDataSet>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Name, "my 1 dataset changed");
            });

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesNameChanged()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series changed", Colors.Blue.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            comp.FindComponent<BarChartSeries>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Name, "my 1 series changed");
            });

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesColorChanged()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Cyan.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            Action<ChartLegendInfo> legendChanged = (info) =>
            {
                if (isUnderTest == false) { return; }

                try
                {
                    CheckIfLegendsAreMatching(expectedLegendInfo, info);
                    legendPassed = true;
                }
                catch (Exception)
                {
                    legendPassed = false;
                }
            };

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                    });
                });
            });

            isUnderTest = true;

            comp.FindComponent<BarChartSeries>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Color, Colors.Cyan.Accent1.ToCssColor());
            });

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void Legend_FullExample_WithChanges()
        {
            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Cyan.Accent1.ToCssColor()),
            };

            var group1 = new ChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            var comp = ctx.RenderComponent<MudEnchancedChart>(pChart =>
            {
                pChart.Add<MudEnchancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
                pChart.Add<MudEnchancedBarChart>(a => a.Chart, p =>
                {
                    p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                    {
                        setP.Add(y => y.Name, "my 1 dataset");
                        setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Color, Colors.Blue.Accent1.ToCssColor());
                        });
                    });
                });
            });


            comp.FindComponent<BarChartSeries>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Color, Colors.Cyan.Accent1.ToCssColor());
            });

            var acutalLegendInfo = comp.FindComponent<MudEnchancedBarChart>().Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            var acutalLegendInfoInLegendComponent = comp.FindComponent<MudEnchancedBarChartLegend>().Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfoInLegendComponent);

            var listItems = comp.FindComponents<MudListItem>();

            listItems.Should().NotBeNull().And.HaveCount(2);

            var firstItem = listItems.ElementAt(0);
            firstItem.Instance.Text.Should().Be("my 1 dataset");

            var secondItem = listItems.ElementAt(1);
            secondItem.Instance.Text.Should().Be("my 1 series");

            comp.FindComponent<BarChartSeries>().SetParametersAndRender(p => p.Add(x => x.Name, "new name"));

            var changedItem = comp.FindComponents<MudListItem>().Last().Instance;

            changedItem.Text.Should().Be("new name");
        }

        private static void CheckIfLegendsAreMatching(ChartLegendInfo expectedLegendInfo, ChartLegendInfo acutalLegendInfo)
        {
            acutalLegendInfo.Should().NotBeNull();
            acutalLegendInfo.Groups.Should().HaveCount(expectedLegendInfo.Groups.Count());

            for (int i = 0; i < expectedLegendInfo.Groups.Count(); i++)
            {
                var actualGroup = acutalLegendInfo.Groups.ElementAt(i);
                var expectedGroup = expectedLegendInfo.Groups.ElementAt(i);
                actualGroup.Should().NotBeNull().And.BeEquivalentTo(actualGroup, opt => opt.Excluding(x => x.series));

                actualGroup.series.Should().NotBeNull().And.HaveCount(expectedGroup.series.Count());

                for (int j = 0; j < expectedGroup.series.Count(); j++)
                {
                    var actualSeries = actualGroup.series.ElementAt(j);
                    var expectedSeries = expectedGroup.series.ElementAt(j);

                    actualSeries.Should().NotBeNull().And.Be(expectedSeries);
                }
            }
        }
    }
}

#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

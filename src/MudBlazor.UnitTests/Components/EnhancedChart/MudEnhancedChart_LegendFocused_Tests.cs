#pragma warning disable IDE1006 // leading underscore
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
    public class MudEnhancedChart_LegendFocused_Tests
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
            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Red.Accent1));
                    });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 2 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 3 series");
                        seriesP.Add(z => z.Color, Colors.Orange.Accent1);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 4 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Purple.Accent1));
                    });
                });
            });

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(1).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            List<ChartLegendInfoSeries> secondDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 3 series", Colors.Orange.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(2).Instance),
                new ChartLegendInfoSeries("my 4 series", Colors.Purple.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(3).Instance),
            };

            var group2 = new DataSeriesBasedChartLegendInfoGroup("my 2 dataset", secondDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1, group2 });


            var acutalLegendInfo = comp.Instance.LegendInfo;

            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);
        }

        [Test]
        public void LegendGetUpdated_DataSetAdded()
        {
            Boolean isUnderTest = false;
            Boolean legendPassed = false;

            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Red.Accent1));
                    });
                });
            });

            isUnderTest = true;

            MudEnhancedBarDataSet dataSet = new MudEnhancedBarDataSet { Name = "my 2 dataset" };
            var series1 = new MudEnhancedBarChartSeries { Name = "my 3 series", Color = new MudColor(Colors.Orange.Accent1) };
            var series2 = new MudEnhancedBarChartSeries { Name = "my 4 series", Color = new MudColor(Colors.Purple.Accent1) };
            dataSet.Add(series1);
            dataSet.Add(series2);

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", new MudColor(Colors.Blue.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
                new ChartLegendInfoSeries("my 2 series", new MudColor(Colors.Red.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(1).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            List<ChartLegendInfoSeries> secondDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 3 series", new MudColor(Colors.Orange.Accent1), true, series1),
                new ChartLegendInfoSeries("my 4 series", new MudColor(Colors.Purple.Accent1), true, series2),
            };

            var group2 = new DataSeriesBasedChartLegendInfoGroup("my 2 dataset", secondDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1, group2 });

            comp.Instance.Add(dataSet);

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSetRemoved()
        {
            Boolean legendPassed = false;
            Boolean isUnderTest = false;

            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Red.Accent1));
                    });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 2 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 3 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Orange.Accent1));
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 4 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Purple.Accent1));
                    });
                });
            });

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
                new ChartLegendInfoSeries("my 2 series", Colors.Red.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(1).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            isUnderTest = true;

            var secondDataSet = comp.FindComponents<MudEnhancedBarDataSet>().Last();
            secondDataSet.Instance.Dispose();

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesAdded()
        {
            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                    //setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    //{
                    //    seriesP.Add(z => z.Name, "");
                    //    seriesP.Add(z => z.Color, );
                    //});
                });
            });

            isUnderTest = true;

            var dataset = comp.FindComponent<MudEnhancedBarDataSet>().Instance;
            var series = new MudEnhancedBarChartSeries { Name = "my 2 series", Color = new MudColor(Colors.Red.Accent1), Dataset = dataset };

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", new MudColor(Colors.Blue.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
                new ChartLegendInfoSeries("my 2 series", new MudColor(Colors.Red.Accent1), true, series),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);
            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            dataset.Add(series);

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSeriesRemoved()
        {
            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 2 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Red.Accent1));
                    });
                });
            });

            isUnderTest = true;

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", Colors.Blue.Accent1, true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            var secondSeries = comp.FindComponents<MudEnhancedBarChartSeries>().Last();
            secondSeries.Instance.Dispose();

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void LegendGetUpdated_DataSetNameChanged()
        {
            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                });
            });

            isUnderTest = true;

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", new MudColor(Colors.Blue.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset changed", firstDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            comp.FindComponent<MudEnhancedBarDataSet>().SetParametersAndRender(p =>
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
            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                });
            });

            isUnderTest = true;

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series changed", new MudColor(Colors.Blue.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            comp.FindComponent<MudEnhancedBarChartSeries>().SetParametersAndRender(p =>
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
            ChartLegendInfo expectedLegendInfo = null;

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

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.LegendInfoChanged, legendChanged);
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(y => y.Name, "my 1 dataset");
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my 1 series");
                        seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                    });
                });
            });

            isUnderTest = true;

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", new MudColor(Colors.Cyan.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            comp.FindComponent<MudEnhancedBarChartSeries>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Color, new MudColor(Colors.Cyan.Accent1));
            });

            var acutalLegendInfo = comp.Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            Assert.IsTrue(legendPassed);
        }

        [Test]
        public void Legend_FullExample_WithChanges()
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
                pChart.Add<MudEnhancedBarChart>(a => a.Chart, p =>
                {
                    p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                    {
                        setP.Add(y => y.Name, "my 1 dataset");
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                        });
                    });
                });
            });

            List<ChartLegendInfoSeries> firstDataSetseriesInfo = new()
            {
                new ChartLegendInfoSeries("my 1 series", new MudColor(Colors.Cyan.Accent1), true, comp.FindComponents<MudEnhancedBarChartSeries>().ElementAt(0).Instance),
            };

            var group1 = new DataSeriesBasedChartLegendInfoGroup("my 1 dataset", firstDataSetseriesInfo, true);

            var expectedLegendInfo = new ChartLegendInfo(new[] { group1 });

            comp.FindComponent<MudEnhancedBarChartSeries>().SetParametersAndRender(p =>
            {
                p.Add(x => x.Color, new MudColor(Colors.Cyan.Accent1));
            });

            var acutalLegendInfo = comp.FindComponent<MudEnhancedBarChart>().Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfo);

            var acutalLegendInfoInLegendComponent = comp.FindComponent<MudEnhancedBarChartLegend>().Instance.LegendInfo;
            CheckIfLegendsAreMatching(expectedLegendInfo, acutalLegendInfoInLegendComponent);

            var listItems = comp.FindComponents<MudListItem>();

            listItems.Should().NotBeNull().And.HaveCount(2);

            var firstItem = listItems.ElementAt(0);
            firstItem.Instance.Text.Should().Be("my 1 dataset");

            var secondItem = listItems.ElementAt(1);
            secondItem.Instance.Text.Should().Be("my 1 series");

            comp.FindComponent<MudEnhancedBarChartSeries>().SetParametersAndRender(p => 
            p.Add(x => x.Name, "new name"));

            var changedItem = comp.FindComponents<MudListItem>().Last().Instance;

            changedItem.Text.Should().Be("new name");
        }

        [Test]
        public void Legend_FullExample_ClickFoToggleIsEnabledOfASeries()
        {
            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
                pChart.Add<MudEnhancedBarChart>(a => a.Chart, p =>
                {
                    p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                    {
                        setP.Add(y => y.Name, "my 1 dataset");
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Color, new MudColor(Colors.Blue.Accent1));
                        });
                    });
                });
            });

            var listItems = comp.FindComponents<MudListItem>();

            listItems.Should().NotBeNull().And.HaveCount(2);

            var secondItem = listItems.ElementAt(1);
            var matItem = secondItem.Nodes.First() as IHtmlDivElement;

            matItem.Click(new MouseEventArgs());

            var series = comp.FindComponent<MudEnhancedBarChartSeries>().Instance;

            series.IsEnabled.Should().Be(false);

            matItem.Click(new MouseEventArgs());

            series.IsEnabled.Should().Be(true);
        }

        [Test]
        public async Task Legend_FullExample_ActiveAndInActiveBasedOnMouseOver()
        {
            var series = new List<Double> { 100.0, 80.0, 20.0 };

            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
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
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 2 series");
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 3 series");
                            seriesP.Add(z => z.Points, series);
                        });
                    });
                });
            });


            var rects = comp.FindAll("polygon");

            rects.Should().HaveCount(9);

            foreach (var item in rects)
            {
                item.ClassList.Contains("active").Should().Be(true);
            }

            var seriesItems = GetListItemsAsWorkaround(comp);

            for (int j = 0; j < 3; j++)
            {
                // for some reasons only the first is working, all other events are not invoked
                // switch to gettting the series element from tag and invoke method there
                //seriesItems[j].(new MouseEventArgs());
                await comp.InvokeAsync(() => seriesItems[j].Series.SentRequestToBecomeActiveAlone());

                rects = comp.FindAll("polygon");
                rects.Should().HaveCount(9);

                for (int i = 0; i < rects.Count; i++)
                {
                    if ((i % 3) == j)
                    {
                        rects[i].ClassList.Contains("active").Should().Be(true);
                    }
                    else
                    {
                        rects[i].ClassList.Contains("inactive").Should().Be(true);
                    }
                }

                seriesItems = GetListItemsAsWorkaround(comp);
                //seriesItems[j].MouseOut(new MouseEventArgs());
                await comp.InvokeAsync(() => seriesItems[j].Series.RevokeExclusiveActiveState());

                rects = comp.FindAll("polygon");
                rects.Should().HaveCount(9);

                foreach (var item in rects)
                {
                    item.ClassList.Contains("active").Should().Be(true);
                }
            }
        }

        [Test]
        public void Legend_FullExample_NotEnaledAndNotAbleToSetExclusivlyActive()
        {
            var series = new List<Double> { 100.0, 80.0, 20.0 };

            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
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
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Points, series);
                            seriesP.Add(z => z.IsEnabled, false);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 2 series");
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 3 series");
                            seriesP.Add(z => z.Points, series);
                        });
                    });
                });
            });

            var rects = comp.FindAll("polygon");

            rects.Should().HaveCount(6);

            foreach (var item in rects)
            {
                item.ClassList.Contains("active").Should().Be(true);
            }

            var seriesItems = GetListItems(comp);

            seriesItems[0].MouseOver(new MouseEventArgs());

            rects = comp.FindAll("polygon");

            rects.Should().HaveCount(6);

            foreach (var item in rects)
            {
                item.ClassList.Contains("active").Should().Be(true);
            }
        }

        [Test]
        public void ChartInteraction_ActivatingSeries()
        {
            var series = new List<Double> { 100.0, 80.0, 20.0 };

            var comp = ctx.RenderComponent<MudEnhancedChart>(pChart =>
            {
                pChart.Add<MudEnhancedBarChartLegend, ChartLegendInfo>(a => a.Legend, value => itemParams => itemParams.Add(o => o.LegendInfo, value));
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
                            seriesP.Add(z => z.Name, "my 1 series");
                            seriesP.Add(z => z.Points, series);
                            seriesP.Add(z => z.IsEnabled, false);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 2 series");
                            seriesP.Add(z => z.Points, series);
                        });
                        setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                        {
                            seriesP.Add(z => z.Name, "my 3 series");
                            seriesP.Add(z => z.Points, series);
                        });
                    });
                });
            });

            var rects = comp.FindAll("polygon");

            rects.Should().HaveCount(6);

            foreach (var item in rects)
            {
                item.ClassList.Contains("active").Should().Be(true);
            }

            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].MouseOver(new MouseEventArgs());

                rects = comp.FindAll("polygon");
                rects.Should().HaveCount(6);

                Int32 moduloFactor = 6 / 3;

                for (int j = 0; j < rects.Count; j++)
                {
                    if ((j % moduloFactor) == i % moduloFactor)
                    {
                        rects[j].ClassList.Contains("active").Should().Be(true);
                    }
                    else
                    {
                        rects[j].ClassList.Contains("inactive").Should().Be(true);
                    }
                }

                rects[i].MouseOut(new MouseEventArgs());
                rects = comp.FindAll("polygon");
                rects.Should().HaveCount(6);

                foreach (var item in rects)
                {
                    item.ClassList.Contains("active").Should().Be(true);
                }
            }
        }

        private static List<IElement> GetListItems(IRenderedComponent<MudEnhancedChart> comp)
        {
            var listItems = comp.FindComponents<MudListItem>();

            listItems.Should().NotBeNull().And.HaveCount(4);

            List<IElement> seriesItems = new List<IElement>
            {
                listItems.ElementAt(1).Nodes.First() as IElement,
                listItems.ElementAt(2).Nodes.First() as IElement,
                listItems.ElementAt(3).Nodes.First() as IElement,
            };

            return seriesItems;
        }

        private static List<ChartLegendInfoSeries> GetListItemsAsWorkaround(IRenderedComponent<MudEnhancedChart> comp)
        {
            var listItems = comp.FindComponents<MudListItem>().Skip(1).Select(x => x.Instance.Tag as ChartLegendInfoSeries).ToList();
            return listItems;
        }

        private static void CheckIfLegendsAreMatching(ChartLegendInfo expectedLegendInfo, ChartLegendInfo acutalLegendInfo)
        {
            acutalLegendInfo.Should().NotBeNull();
            acutalLegendInfo.Groups.Should().HaveCount(expectedLegendInfo.Groups.Count());

            for (int i = 0; i < expectedLegendInfo.Groups.Count(); i++)
            {
                var actualGroup = acutalLegendInfo.Groups.ElementAt(i) ;
                var expectedGroup = expectedLegendInfo.Groups.ElementAt(i) as DataSeriesBasedChartLegendInfoGroup;

                actualGroup.Should().NotBeNull().And.BeOfType<DataSeriesBasedChartLegendInfoGroup>();
                var actualCastedGroup = (DataSeriesBasedChartLegendInfoGroup)actualGroup;

                actualCastedGroup.Should().BeEquivalentTo(actualGroup, opt => opt.Excluding(x => ((DataSeriesBasedChartLegendInfoGroup)x).Series));

                actualCastedGroup.Series.Should().NotBeNull().And.HaveCount(expectedGroup.Series.Count());

                for (int j = 0; j < expectedGroup.Series.Count(); j++)
                {
                    var actualSeries = actualCastedGroup.Series.ElementAt(j);
                    var expectedSeries = expectedGroup.Series.ElementAt(j);

                    actualSeries.Should().NotBeNull().And.Be(expectedSeries);
                }
            }
        }
    }
}

#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

#pragma warning disable IDE1006 // leading underscore

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using AngleSharp.Svg.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.EnhanceChart;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
using static MudBlazor.UnitTests.Components.EnhancedChart.MudEnhancedChartTesterHelper;

namespace MudBlazor.UnitTests.Components.EnhancedChart
{
    [TestFixture]
    public class MudEnhancedBarChartTests
    {
        private Guid _defaultChartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void DefaultValues()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<MudEnhancedBarDataSet> sets = comp.Instance;
            sets.Should().BeEmpty();

            ICollection<IYAxis> axes = comp.Instance;
            axes.Should().ContainSingle();

            //1. chart itself
            //2. x axis
            //3. y axis
            //4. major tick
            //5. minor tick
            called.Should().Be(5);

            comp.Instance.Margin.Should().Be(2);
            comp.Instance.Padding.Should().Be(3);

            comp.Instance.AnimationIsEnabled.Should().BeTrue();

            comp.Instance.Id.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void UpdateOfOwnValuesRecevied()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            called = 0;

            comp.SetParametersAndRender(p => p.Add(x => x.Margin, 5.0));
            comp.SetParametersAndRender(p => p.Add(x => x.Margin, 2.3));

            called.Should().Be(2);

            comp.SetParametersAndRender(p => p.Add(x => x.Padding, 5.0));
            comp.SetParametersAndRender(p => p.Add(x => x.Padding, 2.3));

            called.Should().Be(4);
        }

        [Test]
        public void UpdatesOfXAxesReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var xAxesComponent = comp.FindComponent<MudEnhancedBarChartXAxis>();
            called = 0;

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowGridLines, false));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowGridLines, true));

            called.Should().Be(2);

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, XAxisPlacement.Top));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, XAxisPlacement.Bottom));

            called.Should().Be(4);

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "my new class"));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "another new class"));

            called.Should().Be(6);

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineCssClass, "my new class"));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineCssClass, "another new class"));

            called.Should().Be(8);

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineThickness, 2.0));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineThickness, 0.5));

            called.Should().Be(10);

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineColor, "#808081"));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.GridLineColor, "#808084"));

            called.Should().Be(12);
        }

        [Test]
        public void UpdatesOfTickValuesReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var yAxesComponent = comp.FindComponent<MudEnhancedNumericLinearAutoScaleAxis>();
            called = 0;

            Int32 expectedCalledCounter = 0;
            called.Should().Be(expectedCalledCounter);

            var tickComponents = yAxesComponent.FindComponents<MudEnhancedTick>();

            foreach (var tickComponent in tickComponents)
            {
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Mode, TickMode.Absolute));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Mode, TickMode.Relative));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Value, 10.5));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Value, 20.3));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Thickness, 2.1));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Thickness, 0.7));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Color, "#121234"));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Color, "#A2F3D4"));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.LineCssClass, "some-class"));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.LineCssClass, "another-class"));

                expectedCalledCounter += 10;
                called.Should().Be(expectedCalledCounter);

                tickComponent.Instance.Dispose();
                expectedCalledCounter += 1;
                called.Should().Be(expectedCalledCounter);
            }
        }

        [Test]
        public void UpdatesOfYAxisValuesReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var yAxesComponent = comp.FindComponent<MudEnhancedNumericLinearAutoScaleAxis>();
            called = 0;

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMajorTicks, false));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMajorTicks, true));

            called.Should().Be(2);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMinorTicks, true));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMinorTicks, false));

            called.Should().Be(4);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelSize, 10.5));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelSize, 20.3));

            called.Should().Be(6);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Margin, 12.5));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Margin, 13.44));

            called.Should().Be(8);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "my-awesome-label"));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "my-awesomer-label"));

            called.Should().Be(10);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, YAxisPlacement.Rigth));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, YAxisPlacement.Left));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, YAxisPlacement.None));

            called.Should().Be(13);
        }

        [Test]
        public void UpdatesOfDataSetsValuesAreReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                   {
                       seriesP.Add(z => z.Name, "my first series");
                   });
                });
            });

            called = 0;

            var set = comp.FindComponent<MudEnhancedBarDataSet>();

            var series = set.FindComponent<MudEnhancedBarChartSeries>();

            series.SetParametersAndRender(p => p.Add(x => x.Name, "my first series with a new name"));
            series.SetParametersAndRender(p => p.Add(x => x.Name, "my second series"));

            called.Should().Be(2);

            series.SetParametersAndRender(p => p.Add(x => x.IsEnabled, false));
            series.SetParametersAndRender(p => p.Add(x => x.IsEnabled, true));

            called.Should().Be(4);

            set.SetParametersAndRender(p => p.Add(x => x.IsStacked, true));
            set.SetParametersAndRender(p => p.Add(x => x.IsStacked, false));

            called.Should().Be(6);

            set.SetParametersAndRender(p => p.Add(x => x.Name, "Something"));
            set.SetParametersAndRender(p => p.Add(x => x.Name, "Something new"));

            called.Should().Be(8);


            series.Instance.Dispose();
            called.Should().Be(9);

            set.Instance.Dispose();
            called.Should().Be(10);
        }

        [Test]
        public void MudEnhancedBarChartSeries_DefaultValues()
        {
            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                    });
                });
            });

            var set = comp.FindComponent<MudEnhancedBarDataSet>();
            var series = set.FindComponent<MudEnhancedBarChartSeries>();

            series.Instance.Color.Should().NotBeNull();
            series.Instance.Points.Should().NotBeNull();
            series.Instance.Name.Should().NotBeNull();
        }

        [Test]
        public void UpdatesOfClearReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            called = 0;

            ((ICollection<MudEnhancedBarDataSet>)comp.Instance).Clear();
            called.Should().Be(1);

            ((ICollection<IYAxis>)comp.Instance).Clear();
            called.Should().Be(2);
        }

        [TestCase("en-us")]
        [TestCase("de-de")]
        public void DrawSimpleDataSet_OnlyPostiveValues_NoVisibileAxes(String culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, false);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.None);
                    setP.Add(y => y.ShowMajorTicks, false);
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects))
                );
            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                root.Add(XElement.Parse(preParsedHtml));
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase("en-us")]
        [TestCase("de-de")]
        public void DrawSimpleDataSet_OnlyPostiveValues_XAxesLabel_Bottom(String culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            String classLabel = "dummy-label-class";

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.Height, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.ShowGridLines, false);

                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.None);
                    setP.Add(y => y.ShowMajorTicks, false);
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                (
                    x.P1.ScaleY(0.8).MoveAlongYAxis(20),
                    x.P2.ScaleY(0.8).MoveAlongYAxis(20),
                    x.P3.ScaleY(0.8).MoveAlongYAxis(20),
                    x.P4.ScaleY(0.8).MoveAlongYAxis(20),
                    x.FillColor
                )))
                ));

            Point firstLabelPoint = new Point(25, 92.5);
            Point secondLabelPoint = new Point(75, 92.5);

            expectedRoot.Add(new XElement("text",
                new XAttribute("x", firstLabelPoint.X),
                new XAttribute("y", firstLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Mo"
                ),
                new XElement("text",
                new XAttribute("x", secondLabelPoint.X),
                new XAttribute("y", secondLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Tu"
                )
                );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                root.Add(XElement.Parse(preParsedHtml));
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase("en-us")]
        [TestCase("de-de")]
        public void DrawSimpleDataSet_OnlyPostiveValues_XAxesLabel_Top(String culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            String classLabel = "dummy-label-class";

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.Placement, XAxisPlacement.Top);
                    setP.Add(y => y.Height, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.ShowGridLines, false);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.None);
                    setP.Add(y => y.ShowMajorTicks, false);

                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                (
                    x.P1.ScaleY(0.8),
                    x.P2.ScaleY(0.8),
                    x.P3.ScaleY(0.8),
                    x.P4.ScaleY(0.8),
                    x.FillColor
                )))
                ));

            Point firstLabelPoint = new Point(25, 7.5);
            Point secondLabelPoint = new Point(75, 7.5);

            expectedRoot.Add(new XElement("text",
                new XAttribute("x", firstLabelPoint.X),
                new XAttribute("y", firstLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Mo"
                ),
                new XElement("text",
                new XAttribute("x", secondLabelPoint.X),
                new XAttribute("y", secondLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Tu"
                )
                );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                root.Add(XElement.Parse(preParsedHtml));
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase("en-us")]
        [TestCase("de-de")]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisIsLeft(String culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            String classLabel = "special-class-label";

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, false);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Left);
                    setP.Add(y => y.LabelSize, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 5.0);
                    });
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                 TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                 (
                     x.P1.ScaleX(0.8).MoveAlongXAxis(20),
                     x.P2.ScaleX(0.8).MoveAlongXAxis(20),
                     x.P3.ScaleX(0.8).MoveAlongXAxis(20),
                     x.P4.ScaleX(0.8).MoveAlongXAxis(20),
                     x.FillColor
                 )))
                 ));

            Dictionary<Double, String> expectedLabels = new()
            {
                { 100, "0" },
                { 75, "50" },
                { 50, "100" },
                { 25, "150" },
                { 0, "200" },
            };

            foreach (var item in expectedLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "15"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label " + classLabel),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "end"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase("en-us")]
        [TestCase("de-de")]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisIsRight(String culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            String classLabel = "special-class-label";

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, false);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Rigth);
                    setP.Add(y => y.LabelSize, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 5.0);
                    });
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                 TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                 (
                     x.P1.ScaleX(0.8),
                     x.P2.ScaleX(0.8),
                     x.P3.ScaleX(0.8),
                     x.P4.ScaleX(0.8),
                     x.FillColor
                 )))
                 ));

            Dictionary<Double, String> expectedLabels = new()
            {
                { 100, "0" },
                { 75, "50" },
                { 50, "100" },
                { 25, "150" },
                { 0, "200" },
            };

            foreach (var item in expectedLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "85"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label " + classLabel),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "start"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(0.1)]
        [TestCase(0.01)]
        [TestCase(0.001)]
        [TestCase(0.0001)]
        public void DrawSimpleDataSet_OnlyPostiveValues_ValuesSmallerThanOne(Double scaling)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            String classLabel = "special-class-label";

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, false);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Rigth);
                    setP.Add(y => y.LabelSize, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 5.0);
                    });
                });
            }, scaling, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                 TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                 (
                     x.P1.ScaleX(0.8),
                     x.P2.ScaleX(0.8),
                     x.P3.ScaleX(0.8),
                     x.P4.ScaleX(0.8),
                     x.FillColor
                 )))
                 ));

            Dictionary<Double, String> expectedLabels = new()
            {
                { 100, "0" },
                { 75, Math.Round(50.0 * scaling, 4).ToString(CultureInfo.InvariantCulture) },
                { 50, Math.Round(100.0 * scaling, 4).ToString(CultureInfo.InvariantCulture) },
                { 25, Math.Round(150.0 * scaling, 4).ToString(CultureInfo.InvariantCulture) },
                { 0, Math.Round(200.0 * scaling, 4).ToString(CultureInfo.InvariantCulture) },
            };

            foreach (var item in expectedLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "85"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label " + classLabel),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "start"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase(183, 5, 200, 50, 5)]
        [TestCase(183, 6, 200, 50, 5)]
        [TestCase(183, 7, 200, 20, 11)]
        [TestCase(183, 13, 200, 20, 11)]
        [TestCase(183, 14, 190, 10, 20)]
        [TestCase(183, 15, 190, 10, 20)]

        [TestCase(18.3, 5, 20, 5, 5)]
        [TestCase(18.3, 6, 20, 5, 5)]
        [TestCase(18.3, 7, 20, 2, 11)]
        [TestCase(18.3, 13, 20, 2, 11)]
        [TestCase(18.3, 14, 19, 1, 20)]
        [TestCase(18.3, 15, 19, 1, 20)]

        [TestCase(0.183, 5, 0.2, 0.05, 5)]
        [TestCase(0.183, 6, 0.2, 0.05, 5)]
        [TestCase(0.183, 7, 0.2, 0.02, 11)]
        [TestCase(0.183, 13, 0.2, 0.02, 11)]
        [TestCase(0.183, 14, 0.19, 0.01, 20)]
        [TestCase(0.183, 15, 0.19, 0.01, 20)]
        public void DrawSimpleDataSet_OnlyPostiveValues_AutoScaleValues(Double maxDataSeries, Double majorTickCount, Double expectedMaxLabel, Double expectedStep, Int32 stepAmount)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();

            var firstData = new List<Double> { maxDataSeries, maxDataSeries / 10.0 };

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 1.0);
                p.Add(x => x.Padding, 10.0);
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, false);

                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Rigth);
                    setP.Add(y => y.LabelSize, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, majorTickCount);
                    });
                });
            });

            XElement expectedRoot = new XElement("svg");

            Dictionary<Double, String> expectedLabels = new();

            Double delta = 100.0 / (stepAmount - 1);
            for (int i = 0; i < stepAmount; i++)
            {
                expectedLabels.Add(100 - (delta * i), Math.Round(i * expectedStep, 4).ToString(CultureInfo.InvariantCulture));
            }

            foreach (var item in expectedLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "85"),
                    new XAttribute("y", Math.Round(item.Key, 6).ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label"),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "start"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                if (item.NodeName != "POLYGON")
                {
                    var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                    var element = XElement.Parse(preParsedHtml);
                    RoundElementValues(item, element);
                    root.Add(element);
                }
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        private void GenerateSampleBarChart(
          Action<ComponentParameterCollectionBuilder<MudEnhancedBarChart>> additionalConfiguration,
          out List<Rectangle> untransformedExpectedRects,
          out IRenderedComponent<MudEnhancedBarChart> comp)
            => GenerateSampleBarChart(additionalConfiguration, 1.0, out untransformedExpectedRects, out comp);

        private void GenerateSampleBarChart(
        Action<ComponentParameterCollectionBuilder<MudEnhancedBarChart>> additionalConfiguration,
        Double pointScalingFactor,
        out List<Rectangle> untransformedExpectedRects,
        out IRenderedComponent<MudEnhancedBarChart> comp)
        {
            String firtSeriesColor = (Colors.Brown.Default + "FF").ToLower();
            String secondSeriesColor = (Colors.BlueGrey.Default + "FF").ToLower();
            String thirdSeriesColor = (Colors.Red.Default + "FF").ToLower();
            String fourthSeriesColor = (Colors.Orange.Default + "FF").ToLower();
            String fifthSeriesColor = (Colors.Amber.Default + "FF").ToLower();

            var firstData = new List<Double> { pointScalingFactor * 125.0, pointScalingFactor * 150.0 };
            var secondData = new List<Double> { pointScalingFactor * 100.0, pointScalingFactor * 200.0 };
            var thirdData = new List<Double> { pointScalingFactor * 0.0, pointScalingFactor * 150.0 };
            var fourthData = new List<Double> { pointScalingFactor * 150.0 };
            var fithData = new List<Double> { pointScalingFactor * 250.0, pointScalingFactor * 200.0 };

            untransformedExpectedRects = new List<Rectangle>
            {
                new Rectangle(new Point(5,0),new Point(5,62.5),new Point(14,62.5),new Point(14,0),firtSeriesColor),
                new Rectangle(new Point(15,0),new Point(15,50),new Point(24,50),new Point(24,0),secondSeriesColor),
                new Rectangle(new Point(25,0),new Point(25,0),new Point(34,0),new Point(34,0),thirdSeriesColor),
                new Rectangle(new Point(35,0),new Point(35,75),new Point(44,75),new Point(44,0),fourthSeriesColor),

                new Rectangle(new Point(55,0),new Point(55,75),new Point(64,75),new Point(64,0),firtSeriesColor),
                new Rectangle(new Point(65,0),new Point(65,100),new Point(74,100),new Point(74,0),secondSeriesColor),
                new Rectangle(new Point(75,0),new Point(75,75),new Point(84,75),new Point(84,0),thirdSeriesColor),
                new Rectangle(new Point(85,0),new Point(85,0),new Point(94,0),new Point(94,0),fourthSeriesColor),
            };
            comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 1.0);
                p.Add(x => x.Padding, 10.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, _defaultChartId);

                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, firtSeriesColor);

                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, secondData);
                        seriesP.Add(z => z.Color, secondSeriesColor);

                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my third series");
                        seriesP.Add(z => z.Points, thirdData);
                        seriesP.Add(z => z.Color, thirdSeriesColor);

                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my fourth series");
                        seriesP.Add(z => z.Points, fourthData);
                        seriesP.Add(z => z.Color, fourthSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my fifth series");
                        seriesP.Add(z => z.Points, fithData);
                        seriesP.Add(z => z.Color, fifthSeriesColor);
                        seriesP.Add(z => z.IsEnabled, false);

                    });
                });
                additionalConfiguration(p);
            });
        }

        private void CheckChartBasedOnXAxisAndYAxisAlignment(
            XAxisPlacement xAxisPlacement, YAxisPlacement yAxisPlacement,
            Func<Point, Point> pointTransformation,
            Point firstExpectedXLabelPoint, Point secondExpectedXLabelPoint,
            Dictionary<Double, String> expectedYAxisLabels, Double yLabelXCoordinate, String yAxisLabelTextAnchor,
            Boolean withXAxisGrid, Boolean withYAxisGrid
            )
        {
            String classLabelX = "special-x-class-label";
            String classLabelY = "special-y-class-label";

            Double majorTickValue = 25.0;
            Double minorTickValue = 5.0;

            String xGridLabelClass = "x-grid";
            Double xGridLineThickness = 1.2;
            String xGridLineColor = (Colors.Orange.Accent4 + "FF").ToLower();

            String yGridLineMajorClass = "y-axis-major";
            String yGridLineMajorColor = (Colors.Red.Accent4 + "FF").ToLower();
            Double yGridLineMajorThickness = 1.4;

            String yGridLineMinorClass = "y-axis-minor";
            String yGridLineMinorColor = (Colors.DeepPurple.Accent4 + "FF").ToLower();
            Double yGridLineMinorThickness = 1.3;

            List<Rectangle> untransformedExpectedRects;
            IRenderedComponent<MudEnhancedBarChart> comp;
            GenerateSampleBarChart((p) =>
            {
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, xAxisPlacement);
                    setP.Add(y => y.Margin, 10.0);
                    setP.Add(y => y.Height, 5.0);
                    setP.Add(y => y.LabelCssClass, classLabelX);
                    setP.Add(y => y.ShowGridLines, withXAxisGrid);
                    setP.Add(y => y.GridLineCssClass, xGridLabelClass);
                    setP.Add(y => y.GridLineThickness, xGridLineThickness);
                    setP.Add(y => y.GridLineColor, xGridLineColor);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, yAxisPlacement);
                    setP.Add(y => y.LabelSize, 15.0);
                    setP.Add(y => y.Margin, 5.0);
                    setP.Add(y => y.LabelCssClass, classLabelY);
                    setP.Add(y => y.ShowMajorTicks, withYAxisGrid);
                    setP.Add(y => y.ShowMinorTicks, withYAxisGrid);

                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 5.0);
                        setT.Add(z => z.LineCssClass, yGridLineMajorClass);
                        setT.Add(z => z.Color, yGridLineMajorColor);
                        setT.Add(z => z.Thickness, yGridLineMajorThickness);
                    });
                    setP.Add<MudEnhancedTick>(y => y.MinorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 5.0);
                        setT.Add(z => z.LineCssClass, yGridLineMinorClass);
                        setT.Add(z => z.Color, yGridLineMinorColor);
                        setT.Add(z => z.Thickness, yGridLineMinorThickness);
                    });
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects.Select(x => new Rectangle
                (
                    pointTransformation(x.P1),
                    pointTransformation(x.P2),
                    pointTransformation(x.P3),
                    pointTransformation(x.P4),
                    x.FillColor
                )))
                ));

            expectedRoot.Add(new XElement("text",
              new XAttribute("x", firstExpectedXLabelPoint.X.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("y", firstExpectedXLabelPoint.Y.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("font-size", 5.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabelX),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Mo"
              ),
              new XElement("text",
              new XAttribute("x", secondExpectedXLabelPoint.X.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("y", secondExpectedXLabelPoint.Y.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("font-size", 5.ToString(CultureInfo.InvariantCulture)),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label " + classLabelX),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Tu"
              )
           );

            foreach (var item in expectedYAxisLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedYAxisLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedYAxisLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", yLabelXCoordinate),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label " + classLabelY),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", yAxisLabelTextAnchor),
                    item.Value
                    ));
            }

            Func<Point, Point> AxisCorrector = (p) => p.MirrowY().MoveAlongYAxis(100);

            if (withXAxisGrid)
            {
                Double[] xValues = new[] { 0.0, 50.0, 100.0 };

                for (int i = 0; i < xValues.Length; i++)
                {
                    Point p1 = AxisCorrector(pointTransformation(new Point(xValues[i], 0)));
                    Point p2 = AxisCorrector(pointTransformation(new Point(xValues[i], 100)));

                    expectedRoot.Add(new XElement("line",
                    new XAttribute("stroke", xGridLineColor),
                    new XAttribute("class", $"mud-enhanced-chart-x-axis-grid-line {xGridLabelClass}"),
                    new XAttribute("stroke-width", xGridLineThickness),
                    new XAttribute("x1", Math.Round(p1.X, 6).ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("y1", Math.Round(p1.Y, 6).ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("x2", Math.Round(p2.X, 6).ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("y2", Math.Round(p2.Y, 6).ToString(CultureInfo.InvariantCulture))
                    ));
                }
            }
            if (withYAxisGrid == true)
            {
                Double currentMajorTick = 0;
                Double nextMajorTick = currentMajorTick + majorTickValue;
                while (currentMajorTick <= 100.00)
                {
                    Point p1 = AxisCorrector(pointTransformation(new Point(0, currentMajorTick)));
                    Point p2 = AxisCorrector(pointTransformation(new Point(100, currentMajorTick)));

                    expectedRoot.Add(new XElement("line",
                  new XAttribute("stroke", yGridLineMajorColor),
                  new XAttribute("class", $"mud-enhanced-chart-y-axis-major-grid-line {yGridLineMajorClass}"),
                  new XAttribute("stroke-width", yGridLineMajorThickness),
                  new XAttribute("x1", Math.Round(p1.X, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("y1", Math.Round(p1.Y, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("x2", Math.Round(p2.X, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("y2", Math.Round(p2.Y, 6).ToString(CultureInfo.InvariantCulture))
                  ));

                    if (currentMajorTick < 100.0)
                    {
                        Double currentMinorTick = minorTickValue;
                        while (currentMinorTick < nextMajorTick)
                        {
                            Point p_minor_1 = AxisCorrector(pointTransformation(new Point(0, currentMajorTick + currentMinorTick)));
                            Point p_minor_2 = AxisCorrector(pointTransformation(new Point(100, currentMajorTick + currentMinorTick)));

                            expectedRoot.Add(new XElement("line",
                            new XAttribute("stroke", yGridLineMinorColor),
                            new XAttribute("class", $"mud-enhanced-chart-y-axis-minor-grid-line {yGridLineMinorClass}"),
                            new XAttribute("stroke-width", yGridLineMinorThickness),
                            new XAttribute("x1", Math.Round(p_minor_1.X, 6).ToString(CultureInfo.InvariantCulture)),
                            new XAttribute("y1", Math.Round(p_minor_1.Y, 6).ToString(CultureInfo.InvariantCulture)),
                            new XAttribute("x2", Math.Round(p_minor_2.X, 6).ToString(CultureInfo.InvariantCulture)),
                            new XAttribute("y2", Math.Round(p_minor_2.Y, 6).ToString(CultureInfo.InvariantCulture))
                            ));

                            currentMinorTick += minorTickValue;
                        }
                    }

                    currentMajorTick += majorTickValue;
                }
            }

            XElement root = new XElement("svg");


            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase("de-de", false, false)]
        [TestCase("de-de", false, true)]
        [TestCase("de-de", true, false)]
        [TestCase("de-de", true, true)]
        [TestCase("en-us", true, true)]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisLeftAndXAxisBottom(String culture, Boolean withXAxisGrid, Boolean withYAxisGrid)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Dictionary<Double, String> expectedYAxisLabels = new()
            {
                { 4 * 21.25, "0" },
                { 3 * 21.25, "50" },
                { 2 * 21.25, "100" },
                { 1 * 21.25, "150" },
                { 0, "200" },
            };

            CheckChartBasedOnXAxisAndYAxisAlignment(XAxisPlacement.Bottom, YAxisPlacement.Left, (p) =>
             p.ScaleY(0.85).MoveAlongYAxis(15).ScaleX(0.8).MoveAlongXAxis(20),
            new Point(40, 97.5), new Point(80, 97.5),
            expectedYAxisLabels, 15, "end", withXAxisGrid, withYAxisGrid);
        }

        [TestCase("de-de", false, false)]
        [TestCase("de-de", false, true)]
        [TestCase("de-de", true, false)]
        [TestCase("de-de", true, true)]
        [TestCase("en-us", true, true)]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisRightAndXAxisBottom(String culture, Boolean withXAxisGrid, Boolean withYAxisGrid)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Dictionary<Double, String> expectedYAxisLabels = new()
            {
                { 4 * 21.25, "0" },
                { 3 * 21.25, "50" },
                { 2 * 21.25, "100" },
                { 1 * 21.25, "150" },
                { 0, "200" },
            };

            CheckChartBasedOnXAxisAndYAxisAlignment(XAxisPlacement.Bottom, YAxisPlacement.Rigth, (p) =>
             p.ScaleY(0.85).MoveAlongYAxis(15).ScaleX(0.8),
            new Point(20, 97.5), new Point(60, 97.5),
            expectedYAxisLabels, 100 - 15, "start", withXAxisGrid, withYAxisGrid
            );
        }

        [TestCase("de-de", false, false)]
        [TestCase("de-de", false, true)]
        [TestCase("de-de", true, false)]
        [TestCase("de-de", true, true)]
        [TestCase("en-us", true, true)]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisLeftAndXAxisTop(String culture, Boolean withXAxisGrid, Boolean withYAxisGrid)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Dictionary<Double, String> expectedYAxisLabels = new()
            {
                { 100, "0" },
                { 100 - 1 * 21.25, "50" },
                { 100 - 2 * 21.25, "100" },
                { 100 - 3 * 21.25, "150" },
                { 15, "200" },
            };

            CheckChartBasedOnXAxisAndYAxisAlignment(XAxisPlacement.Top, YAxisPlacement.Left, (p) =>
             p.ScaleY(0.85).ScaleX(0.8).MoveAlongXAxis(20),
            new Point(40, 2.5), new Point(80, 2.5),
            expectedYAxisLabels, 15, "end", withXAxisGrid, withYAxisGrid);
        }

        [TestCase("de-de", false, false)]
        [TestCase("de-de", false, true)]
        [TestCase("de-de", true, false)]
        [TestCase("de-de", true, true)]
        [TestCase("en-us", true, true)]
        public void DrawSimpleDataSet_OnlyPostiveValues_YAxisRightAndXAxisTop(String culture, Boolean withXAxisGrid, Boolean withYAxisGrid)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Dictionary<Double, String> expectedYAxisLabels = new()
            {
                { 100, "0" },
                { 100 - 1 * 21.25, "50" },
                { 100 - 2 * 21.25, "100" },
                { 100 - 3 * 21.25, "150" },
                { 15, "200" },
            };

            CheckChartBasedOnXAxisAndYAxisAlignment(XAxisPlacement.Top, YAxisPlacement.Rigth, (p) =>
             p.ScaleY(0.85).ScaleX(0.8),
            new Point(20, 2.5), new Point(60, 2.5),
            expectedYAxisLabels, 100 - 15, "start", withXAxisGrid, withYAxisGrid
            );
        }

        [TestCase(183, 5, 200)]
        [TestCase(183, 6, 200)]
        [TestCase(183, 7, 200)]
        [TestCase(183, 13, 200)]
        [TestCase(183, 14, 190)]
        [TestCase(183, 15, 190)]

        [TestCase(18.3, 5, 20)]
        [TestCase(18.3, 6, 20)]
        [TestCase(18.3, 7, 20)]
        [TestCase(18.3, 13, 20)]
        [TestCase(18.3, 14, 19)]
        [TestCase(18.3, 15, 19)]

        [TestCase(0.183, 5, 0.2)]
        [TestCase(0.183, 6, 0.2)]
        [TestCase(0.183, 7, 0.2)]
        [TestCase(0.183, 13, 0.2)]
        [TestCase(0.183, 14, 0.19)]
        [TestCase(0.183, 15, 0.19)]

        [TestCase(-18.3, 5, -20)]
        [TestCase(-18.3, 6, -20)]
        [TestCase(-18.3, 7, -20)]
        [TestCase(-18.3, 13, -20)]
        [TestCase(-18.3, 14, -19)]
        [TestCase(-18.3, 15, -19)]

        [TestCase(-0.183, 5, -0.2)]
        [TestCase(-0.183, 6, -0.2)]
        [TestCase(-0.183, 7, -0.2)]
        [TestCase(-0.183, 13, -0.2)]
        [TestCase(-0.183, 14, -0.19)]
        [TestCase(-0.183, 15, -0.19)]
        public void DrawSimpleDataSet_OnlyValuesInOnRange_AutoScaleValues_BarHeigth(Double maxDataSeries, Double majorTickCount, Double expectedMax)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Boolean isnegative = maxDataSeries < 0;

            Random random = new Random();
            String color = (Colors.Red.Darken1 + "ff").ToLower();

            var firstData = new List<Double> { maxDataSeries };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, color);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.None);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, majorTickCount);
                    });
                });
            });

            Double height = (maxDataSeries / expectedMax) * 100;
            height = Math.Round((100 - height), 4);

            XElement expectedRoot = new XElement("svg", new XElement(
               new XElement("polygon",
                new XAttribute("fill", color),
                new XAttribute("data-chartid", chartId.ToString()),
                new XAttribute("class", "mud-enhanced-chart-series bar active"),
                (isnegative == false ?
                 new XAttribute("points", $"0,100 0,{height.ToString(CultureInfo.InvariantCulture)} 100,{height.ToString(CultureInfo.InvariantCulture)} 100,100") :
                 new XAttribute("points", $"0,0 0,{ (100 - height).ToString(CultureInfo.InvariantCulture)} 100,{ (100 - height).ToString(CultureInfo.InvariantCulture)} 100,0")
                 )
                )));

            var root = GetElementAsXmlDocument(comp);

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void DrawSimpleDataSet_PostiveAndNegativesValues()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();
            String firstSeriesColor = (Colors.Red.Darken1 + "ff").ToLower();
            String secondSeriesColor = (Colors.Green.Darken1 + "ff").ToLower();

            var firstData = new List<Double> { 40, -30 };
            var secondData = new List<Double> { 20, -50 };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, firstSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, secondData);
                        seriesP.Add(z => z.Color, secondSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.ShowGridLines, false);
                    setP.Add(y => y.Margin, 0.0);
                    setP.Add(y => y.Height, 10.0);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Left);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add(y => y.ShowMinorTicks, false);
                    setP.Add(y => y.LabelSize, 5.0);
                    setP.Add(y => y.Margin, 15.0);

                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 10);

                    });
                });
            });

            XElement expectedRoot = new XElement("svg",
              TransformRectToSvgElements(new[] {
                new Rectangle(new Point(20,40),new Point(20,0),new Point(40,0),new Point(40,40),firstSeriesColor),
                new Rectangle(new Point(40,40),new Point(40,20),new Point(60,20),new Point(60,40),secondSeriesColor),
                new Rectangle(new Point(60,40),new Point(60,70),new Point(80,70),new Point(80,40),firstSeriesColor),
                new Rectangle(new Point(80,40),new Point(80,90),new Point(100,90),new Point(100,40),secondSeriesColor),
              }));


            expectedRoot.Add(new XElement("text",
              new XAttribute("x", "40"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Mo"
              ),
              new XElement("text",
              new XAttribute("x", "80"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Tu"
              ));

            Dictionary<Double, String> expectedYAxisLabels = new Dictionary<Double, String>
            {
                { 90, "-50" },
                { 80, "-40" },
                { 70, "-30" },
                { 60, "-20" },
                { 50, "-10" },
                { 40, "0" },
                { 30, "10" },
                { 20, "20" },
                { 10, "30" },
                { 0.0, "40" },

            };

            foreach (var item in expectedYAxisLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedYAxisLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedYAxisLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "5"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label"),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "end"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void DrawSimpleDataSet_OnlyNegativesValues()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();
            String firstSeriesColor = (Colors.Red.Darken1 + "ff").ToLower();
            String secondSeriesColor = (Colors.Green.Darken1 + "ff").ToLower();

            var firstData = new List<Double> { -30, -60 };
            var secondData = new List<Double> { -30, -90 };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, firstSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, secondData);
                        seriesP.Add(z => z.Color, secondSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.ShowGridLines, false);
                    setP.Add(y => y.Margin, 0.0);
                    setP.Add(y => y.Height, 10.0);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Left);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add(y => y.ShowMinorTicks, false);
                    setP.Add(y => y.LabelSize, 5.0);
                    setP.Add(y => y.Margin, 15.0);

                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 10);

                    });
                });
            });

            XElement expectedRoot = new XElement("svg",
              TransformRectToSvgElements(new[] {
                new Rectangle(new Point(20,0),new Point(20,30),new Point(40,30),new Point(40,0),firstSeriesColor),
                new Rectangle(new Point(40,0),new Point(40,30),new Point(60,30),new Point(60,0),secondSeriesColor),
                new Rectangle(new Point(60,0),new Point(60,60),new Point(80,60),new Point(80,0),firstSeriesColor),
                new Rectangle(new Point(80,0),new Point(80,90),new Point(100,90),new Point(100,0),secondSeriesColor),
              }));

            expectedRoot.Add(new XElement("text",
              new XAttribute("x", "40"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Mo"
              ),
              new XElement("text",
              new XAttribute("x", "80"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Tu"
              ));

            Dictionary<Double, String> expectedYAxisLabels = new Dictionary<Double, String>
            {
                { 90, "-90" },
                { 80, "-80" },
                { 70, "-70" },
                { 60, "-60" },
                { 50, "-50" },
                { 40, "-40" },
                { 30, "-30" },
                { 20, "-20" },
                { 10, "-10" },
                { 0.0, "0" },
            };

            foreach (var item in expectedYAxisLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedYAxisLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedYAxisLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "5"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label"),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "end"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void DrawStackedAndNotStackedCharts_OnlyPositiveValues()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();
            String firstSeriesColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondSeriesColor = (Colors.DeepPurple.Darken1 + "ff").ToLower();
            String thirdSeriesColor = (Colors.Orange.Darken1 + "ff").ToLower();

            String firstSeriesInSecondDatasetSeriesColor = (Colors.Green.Darken1 + "ff").ToLower();

            var firstDataStacked = new List<Double> { 20, 30 };
            var secondDataStacked = new List<Double> { 30, 50 };
            var thirdDataStacked = new List<Double> { 20, 10 };


            var firstSeriesSecondDataset = new List<Double> { 40, 70 };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(x => x.IsStacked, true);
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first stacked series");
                        seriesP.Add(z => z.Points, firstDataStacked);
                        seriesP.Add(z => z.Color, firstSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second stacked series");
                        seriesP.Add(z => z.Points, secondDataStacked);
                        seriesP.Add(z => z.Color, secondSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my third stacked series");
                        seriesP.Add(z => z.Points, thirdDataStacked);
                        seriesP.Add(z => z.Color, thirdSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, firstSeriesSecondDataset);
                        seriesP.Add(z => z.Color, firstSeriesInSecondDatasetSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.ShowGridLines, false);
                    setP.Add(y => y.Margin, 0.0);
                    setP.Add(y => y.Height, 10.0);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Left);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add(y => y.ShowMinorTicks, false);
                    setP.Add(y => y.LabelSize, 5.0);
                    setP.Add(y => y.Margin, 15.0);

                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 10);

                    });
                });
            });

            XElement expectedRoot = new XElement("svg",
              TransformRectToSvgElements(new[] {
                new Rectangle(new Point(20,90),new Point(20,70),new Point(40,70),new Point(40,90),firstSeriesColor),
                new Rectangle(new Point(20,70),new Point(20,40),new Point(40,40),new Point(40,70),secondSeriesColor),
                new Rectangle(new Point(20,40),new Point(20,20),new Point(40,20),new Point(40,40),thirdSeriesColor),

                new Rectangle(new Point(40,90),new Point(40,50),new Point(60,50),new Point(60,90),firstSeriesInSecondDatasetSeriesColor),

                new Rectangle(new Point(60,90),new Point(60,60),new Point(80,60),new Point(80,90),firstSeriesColor),
                new Rectangle(new Point(60,60),new Point(60,10),new Point(80,10),new Point(80,60),secondSeriesColor),
                new Rectangle(new Point(60,10),new Point(60,0),new Point(80,0),new Point(80,10),thirdSeriesColor),

                new Rectangle(new Point(80,90),new Point(80,20),new Point(100,20),new Point(100,90),firstSeriesInSecondDatasetSeriesColor),
              }));

            expectedRoot.Add(new XElement("text",
              new XAttribute("x", "40"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Mo"
              ),
              new XElement("text",
              new XAttribute("x", "80"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Tu"
              ));

            Dictionary<Double, String> expectedYAxisLabels = new Dictionary<Double, String>
            {
                { 90, "0" },
                { 80, "10" },
                { 70, "20" },
                { 60, "30" },
                { 50, "40" },
                { 40, "50" },
                { 30, "60" },
                { 20, "70" },
                { 10, "80" },
                { 0.0, "90" },
            };

            foreach (var item in expectedYAxisLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedYAxisLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedYAxisLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "5"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label"),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "end"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [Test]
        public void DrawStackedAndNotStackedCharts_OnlyNegativeValues()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();
            String firstSeriesColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondSeriesColor = (Colors.DeepPurple.Darken1 + "ff").ToLower();
            String thirdSeriesColor = (Colors.Orange.Darken1 + "ff").ToLower();

            String firstSeriesInSecondDatasetSeriesColor = (Colors.Green.Darken1 + "ff").ToLower();

            var firstDataStacked = new List<Double> { -20, -30 };
            var secondDataStacked = new List<Double> { -30, -50 };
            var thirdDataStacked = new List<Double> { -20, -10 };


            var firstSeriesSecondDataset = new List<Double> { -40, -70 };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add(x => x.IsStacked, true);
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first stacked series");
                        seriesP.Add(z => z.Points, firstDataStacked);
                        seriesP.Add(z => z.Color, firstSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second stacked series");
                        seriesP.Add(z => z.Points, secondDataStacked);
                        seriesP.Add(z => z.Color, secondSeriesColor);
                    });
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my third stacked series");
                        seriesP.Add(z => z.Points, thirdDataStacked);
                        seriesP.Add(z => z.Color, thirdSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, firstSeriesSecondDataset);
                        seriesP.Add(z => z.Color, firstSeriesInSecondDatasetSeriesColor);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.ShowGridLines, false);
                    setP.Add(y => y.Margin, 0.0);
                    setP.Add(y => y.Height, 10.0);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.Left);
                    setP.Add(y => y.ShowMajorTicks, false);
                    setP.Add(y => y.ShowMinorTicks, false);
                    setP.Add(y => y.LabelSize, 5.0);
                    setP.Add(y => y.Margin, 15.0);

                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Value, 10);

                    });
                });
            });

            XElement expectedRoot = new XElement("svg",
              TransformRectToSvgElements(new[] {
                new Rectangle(new Point(20,0),new Point(20,20),new Point(40,20),new Point(40,0),firstSeriesColor),
                new Rectangle(new Point(20,20),new Point(20,50),new Point(40,50),new Point(40,20),secondSeriesColor),
                new Rectangle(new Point(20,50),new Point(20,70),new Point(40,70),new Point(40,50),thirdSeriesColor),

                new Rectangle(new Point(40,0),new Point(40,40),new Point(60,40),new Point(60,0),firstSeriesInSecondDatasetSeriesColor),

                new Rectangle(new Point(60,0),new Point(60,30),new Point(80,30),new Point(80,0),firstSeriesColor),
                new Rectangle(new Point(60,30),new Point(60,80),new Point(80,80),new Point(80,30),secondSeriesColor),
                new Rectangle(new Point(60,80),new Point(60,90),new Point(80,90),new Point(80,80),thirdSeriesColor),

                new Rectangle(new Point(80,0),new Point(80,70),new Point(100,70),new Point(100,0),firstSeriesInSecondDatasetSeriesColor),
              }));

            expectedRoot.Add(new XElement("text",
              new XAttribute("x", "40"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Mo"
              ),
              new XElement("text",
              new XAttribute("x", "80"),
              new XAttribute("y", "95"),
              new XAttribute("font-size", "10"),
              new XAttribute("class", "mud-enhanced-chart-x-axis-label"),
              new XAttribute("dominant-baseline", "middle"),
              new XAttribute("text-anchor", "middle"),
              "Tu"
              ));

            Dictionary<Double, String> expectedYAxisLabels = new Dictionary<Double, String>
            {
                { 90, "-90" },
                { 80, "-80" },
                { 70, "-70" },
                { 60, "-60" },
                { 50, "-50" },
                { 40, "-40" },
                { 30, "-30" },
                { 20, "-20" },
                { 10, "-10" },
                { 0.0, "0" },
            };

            foreach (var item in expectedYAxisLabels)
            {
                String dominantBaseline = "middle";
                if (item.Key == expectedYAxisLabels.First().Key)
                {
                    dominantBaseline = "text-after-edge";
                }
                else if (item.Key == expectedYAxisLabels.Last().Key)
                {
                    dominantBaseline = "text-before-edge";
                }

                expectedRoot.Add(new XElement("text",
                    new XAttribute("x", "5"),
                    new XAttribute("y", item.Key.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("class", "mud-enhanced-chart-y-axis-major-label"),
                    new XAttribute("font-size", 3),
                    new XAttribute("dominant-baseline", dominantBaseline),
                    new XAttribute("text-anchor", "end"),
                    item.Value
                    ));
            }

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                var element = XElement.Parse(preParsedHtml);
                RoundElementValues(item, element);
                root.Add(element);
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        [TestCase(183, -30, 5, 50, 200, -50)]
        [TestCase(18, -3, 5, 5, 20, -5)]
        [TestCase(1.8, -0.3, 5, 0.5, 2, -0.5)]
        [TestCase(0.18, -0.03, 5, 0.05, 0.2, -0.05)]
        public void DrawSimpleDataSet_PositiveAndNegativeValues_AutoScaleValues_BarHeigth(Double positiveValue, Double negativeValue, Double majorTickCount, Double majorTickValue, Double expectedMax, Double expectedMin)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Random random = new Random();
            String color = (Colors.Red.Darken1 + "ff").ToLower();
            String gridColor = (Colors.Amber.Darken1 + "ff").ToLower();
            String xAxisGridColor = (Colors.DeepOrange.Darken1 + "ff").ToLower();

            var firstData = new List<Double> { positiveValue, negativeValue };

            Guid chartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

            var comp = ctx.RenderComponent<MudEnhancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 0.0);
                p.Add(x => x.Padding, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add(x => x.Id, chartId);

                p.Add<MudEnhancedBarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<MudEnhancedBarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, color);
                    });
                });
                p.Add<MudEnhancedBarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                    setP.Add(y => y.ShowGridLines, true);
                    setP.Add(y => y.GridLineColor, xAxisGridColor);
                    setP.Add(y => y.GridLineThickness, 1.2);
                });
                p.Add<MudEnhancedNumericLinearAutoScaleAxis>(x => x.YAxes, (setP) =>
                {
                    setP.Add(y => y.Placement, YAxisPlacement.None);
                    setP.Add(y => y.ShowMajorTicks, true);
                    setP.Add<MudEnhancedTick>(y => y.MajorTick, (setT) =>
                    {
                        setT.Add(z => z.Color, gridColor);
                        setT.Add(z => z.Thickness, 1.0);
                        setT.Add(z => z.Value, majorTickCount);
                    });
                });
            });

            Double distance = expectedMax - expectedMin;

            Double maxHeigth = Math.Round(100 - (((positiveValue - expectedMin) / distance) * 100), 4);
            Double nullLevel = Math.Round(100 - (((0 - expectedMin) / distance) * 100), 4);
            Double minHeigth = Math.Round(100 - (((negativeValue - expectedMin) / distance) * 100), 4);

            XElement expectedRoot = new XElement("svg", new XElement(
               new XElement("polygon",
                new XAttribute("fill", color),
                new XAttribute("data-chartid", chartId.ToString()),
                new XAttribute("class", "mud-enhanced-chart-series bar active"),
                new XAttribute("points", $"0,{nullLevel.ToString(CultureInfo.InvariantCulture)} 0,{maxHeigth.ToString(CultureInfo.InvariantCulture)} 50,{maxHeigth.ToString(CultureInfo.InvariantCulture)} 50,{nullLevel.ToString(CultureInfo.InvariantCulture)}")
                )),
                new XElement("polygon",
                new XAttribute("fill", color),
                new XAttribute("data-chartid", chartId.ToString()),
                new XAttribute("class", "mud-enhanced-chart-series bar active"),
                new XAttribute("points", $"50,{nullLevel.ToString(CultureInfo.InvariantCulture)} 50,{minHeigth.ToString(CultureInfo.InvariantCulture)} 100,{minHeigth.ToString(CultureInfo.InvariantCulture)} 100,{nullLevel.ToString(CultureInfo.InvariantCulture)}")
                ),
                new XElement("line",
                new XAttribute("stroke", xAxisGridColor),
                new XAttribute("class", $"mud-enhanced-chart-x-axis-grid-line"),
                new XAttribute("stroke-width", "1.2"),
                new XAttribute("x1", "0"),
                new XAttribute("y1", "100"),
                new XAttribute("x2", "0"),
                new XAttribute("y2", "0")
              ),
                new XElement("line",
                new XAttribute("stroke", xAxisGridColor),
                new XAttribute("class", $"mud-enhanced-chart-x-axis-grid-line"),
                new XAttribute("stroke-width", "1.2"),
                new XAttribute("x1", "50"),
                new XAttribute("y1", "100"),
                new XAttribute("x2", "50"),
                new XAttribute("y2", "0")
              ),
                new XElement("line",
                new XAttribute("stroke", xAxisGridColor),
                new XAttribute("class", $"mud-enhanced-chart-x-axis-grid-line"),
                new XAttribute("stroke-width", "1.2"),
                new XAttribute("x1", "100"),
                new XAttribute("y1", "100"),
                new XAttribute("x2", "100"),
                new XAttribute("y2", "0")
              ));

            Double tick = expectedMin;
            Double tickAmount = (distance / majorTickValue);
            Double deltaPerTick = 100 / tickAmount;

            Double yTick = 100.0;
            for (int i = 0; i <= tickAmount; i++)
            {
                expectedRoot.Add(new XElement("line",
                  new XAttribute("stroke", gridColor),
                  new XAttribute("class", $"mud-enhanced-chart-y-axis-major-grid-line"),
                  new XAttribute("stroke-width", "1"),
                  new XAttribute("x1", Math.Round(0.0, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("y1", Math.Round(yTick, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("x2", Math.Round(100.0, 6).ToString(CultureInfo.InvariantCulture)),
                  new XAttribute("y2", Math.Round(yTick, 6).ToString(CultureInfo.InvariantCulture))
                ));

                yTick -= deltaPerTick;
            }

            var root = GetElementAsXmlDocument(comp, true);

            root.Should().BeEquivalentTo(expectedRoot);
        }

        private IEnumerable<Rectangle> TransformToSvgCoordinates(IEnumerable<Rectangle> input)
            => input.Select((Func<Rectangle, Rectangle>)(x => new Rectangle(
                x.P1.TransformPointToSvgCoordinateSystem(),
                x.P2.TransformPointToSvgCoordinateSystem(),
                x.P3.TransformPointToSvgCoordinateSystem(),
                x.P4.TransformPointToSvgCoordinateSystem(),
                (string)x.FillColor
                )));

        private IEnumerable<XElement> TransformRectToSvgElements(IEnumerable<Rectangle> input)
            => input.Select((Func<Rectangle, XElement>)(e => new XElement("polygon",
                new XAttribute("fill", (object)e.FillColor),
                new XAttribute("data-chartid", _defaultChartId.ToString()),
                new XAttribute("class", "mud-enhanced-chart-series bar active"),
                new XAttribute("points", _staticRegex.Replace($"{e.P1.X.ToString(CultureInfo.InvariantCulture)},{e.P1.Y.ToString(CultureInfo.InvariantCulture)} {e.P2.X.ToString(CultureInfo.InvariantCulture)},{e.P2.Y.ToString(CultureInfo.InvariantCulture)} {e.P3.X.ToString(CultureInfo.InvariantCulture)},{e.P3.Y.ToString(CultureInfo.InvariantCulture)} {e.P4.X.ToString(CultureInfo.InvariantCulture)},{e.P4.Y.ToString(CultureInfo.InvariantCulture)}", "0"))
                )));

        private static Regex _staticRegex = new Regex(@"(\-0)");

    }

    record Point(Double X, Double Y);
    record Rectangle(Point P1, Point P2, Point P3, Point P4, String FillColor);

    static class PointTransforamtion
    {
        public static Point ScaleY(this Point p, Double value) => new Point(p.X, p.Y * value);
        public static Point ScaleX(this Point p, Double value) => new Point(p.X * value, p.Y);
        public static Point MoveAlongYAxis(this Point p, Double value) => new Point(p.X, p.Y + value);
        public static Point MoveAlongXAxis(this Point p, Double value) => new Point(p.X + value, p.Y);

        public static Point MirrowY(this Point p) => new Point(p.X, -p.Y);


        public static Point TransformPointToSvgCoordinateSystem(this Point point) => new Point(Math.Round(point.X, 10), Math.Round((point.Y - 100) * (-1), 10));

    }
}

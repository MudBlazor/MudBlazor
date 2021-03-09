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
using MudBlazor.Components.EnchancedChart;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.EnchancedChart
{
    [TestFixture]
    public class MudEnchancedBarChartTests
    {
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

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<BarDataSet> sets = comp.Instance;
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

        }

        [Test]
        public void UpdateOfOwnValuesRecevied()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
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

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var xAxesComponent = comp.FindComponent<BarChartXAxis>();
            called = 0;

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowGridLines, false));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowGridLines, true));

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, XAxisPlacement.Top));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.Placement, XAxisPlacement.Bottom));

            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "my new class"));
            xAxesComponent.SetParametersAndRender(p => p.Add(x => x.LabelCssClass, "another new class"));

            called.Should().Be(6);

        }

        [Test]
        public void UpdatesOfTickValuesReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var yAxesComponent = comp.FindComponent<NumericLinearAxis>();
            called = 0;

            Int32 expectedCalledCounter = 0;
            called.Should().Be(expectedCalledCounter);

            var tickComponents = yAxesComponent.FindComponents<Tick>();

            foreach (var tickComponent in tickComponents)
            {
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Mode, TickMode.Absolut));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Mode, TickMode.Relative));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Value, 10.5));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Value, 20.3));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Thickness, 2.1));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Thickness, 0.7));

                tickComponent.SetParametersAndRender(p => p.Add(x => x.Color, "#121234"));
                tickComponent.SetParametersAndRender(p => p.Add(x => x.Color, "#A2F3D4"));

                expectedCalledCounter += 8;
                called.Should().Be(expectedCalledCounter);
            }
        }

        [Test]
        public void UpdatesOfYAxisValuesReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            var yAxesComponent = comp.FindComponent<NumericLinearAxis>();
            called = 0;

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMajorTicks, false));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMajorTicks, true));

            called.Should().Be(2);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMinorTicks, true));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ShowMinorTicks, false));

            called.Should().Be(4);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ScalingType, ScalingType.Manuel));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.ScalingType, ScalingType.Auto));

            called.Should().Be(6);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Min, 10.5));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Min, 20.3));

            called.Should().Be(8);

            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Max, 10.5));
            yAxesComponent.SetParametersAndRender(p => p.Add(x => x.Max, 20.3));

            called.Should().Be(10);
        }

        [Test]
        public void UpdatesOfDataSetsValuesAreReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                   {
                       seriesP.Add(z => z.Name, "my first series");
                   });
                });
            });

            called = 0;

            var set = comp.FindComponent<BarDataSet>();

            var series = set.FindComponent<BarChartSeries>();

            series.SetParametersAndRender(p => p.Add(x => x.Name, "my first series with a new name"));
            series.SetParametersAndRender(p => p.Add(x => x.Name, "my second series"));

            called.Should().Be(2);

            set.SetParametersAndRender(p => p.Add(x => x.IsStacked, true));
            set.SetParametersAndRender(p => p.Add(x => x.IsStacked, false));

            called.Should().Be(4);

            set.SetParametersAndRender(p => p.Add(x => x.Name, "Something"));
            set.SetParametersAndRender(p => p.Add(x => x.Name, "Something new"));

            called.Should().Be(6);

            series.Instance.Dispose();
            called.Should().Be(7);

            set.Instance.Dispose();
            called.Should().Be(8);
        }

        [Test]
        public void BarChartSeries_DefaultValues()
        {
            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                    });
                });
            });

            var set = comp.FindComponent<BarDataSet>();
            var series = set.FindComponent<BarChartSeries>();

            series.Instance.Color.Should().NotBeNull();
            series.Instance.Points.Should().NotBeNull();
            series.Instance.Name.Should().NotBeNull();
        }

        [Test]
        public void UpdatesOfClearReceived()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            called = 0;

            ((ICollection<BarDataSet>)comp.Instance).Clear();
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
            IRenderedComponent<MudEnchancedBarChart> comp;
            GenerateSampleBarChart((p) => {
                p.Add<BarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.Placement, XAxisPlacement.None);
                });
            }, out untransformedExpectedRects, out comp);

            XElement expectedRoot = new XElement("svg",
                TransformRectToSvgElements(TransformToSvgCoordinates(untransformedExpectedRects))
                );
            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                root.Add(XElement.Parse(item.OuterHtml));
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
            IRenderedComponent<MudEnchancedBarChart> comp;
            GenerateSampleBarChart((p) => {
                p.Add<BarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.Placement, XAxisPlacement.Bottom);
                    setP.Add(y => y.Height, 15.0);
                    setP.Add(y => y.Margin, 5.0);
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
                new XAttribute("class", classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Mo"
                ),
                new XElement("text",
                new XAttribute("x", secondLabelPoint.X),
                new XAttribute("y", secondLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Tu"
                )
                );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                root.Add(XElement.Parse(item.OuterHtml));
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
            IRenderedComponent<MudEnchancedBarChart> comp;
            GenerateSampleBarChart((p) => {
                p.Add<BarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                    setP.Add(y => y.LabelCssClass, classLabel);
                    setP.Add(y => y.Placement, XAxisPlacement.Top);
                    setP.Add(y => y.Height, 15.0);
                    setP.Add(y => y.Margin, 5.0);
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
                new XAttribute("class", classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Mo"
                ),
                new XElement("text",
                new XAttribute("x", secondLabelPoint.X),
                new XAttribute("y", secondLabelPoint.Y),
                new XAttribute("font-size", 15.0),
                new XAttribute("class", classLabel),
                new XAttribute("dominant-baseline", "middle"),
                new XAttribute("text-anchor", "middle"),
                "Tu"
                )
                );

            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                root.Add(XElement.Parse(item.OuterHtml));
            }

            root.Should().BeEquivalentTo(expectedRoot);
        }

        private void GenerateSampleBarChart(
            Action<ComponentParameterCollectionBuilder<MudEnchancedBarChart>> additionalConfiguration,
            out List<Rectangle> untransformedExpectedRects, 
            out IRenderedComponent<MudEnchancedBarChart> comp)
        {
            String firtSeriesColor = (Colors.Brown.Default + "FF").ToLower();
            String secondSeriesColor = (Colors.BlueGrey.Default + "FF").ToLower();
            String thirdSeriesColor = (Colors.Red.Default + "FF").ToLower();
            String fourthSeriesColor = (Colors.Orange.Default + "FF").ToLower();

            var firstData = new List<Double> { 125.0, 150.0 };
            var secondData = new List<Double> { 100.0, 200.0 };
            var thirdData = new List<Double> { 0.0, 150.0 };
            var fourthData = new List<Double> { 150.0 };

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
            comp = ctx.RenderComponent<MudEnchancedBarChart>(p =>
            {
                p.Add(x => x.Margin, 1.0);
                p.Add(x => x.Padding, 10.0);
                p.Add<BarChartXAxis>(x => x.XAxis, (setP) =>
                {
                    setP.Add(y => y.Labels, new List<String> { "Mo", "Tu" });
                });
                p.Add<BarDataSet>(x => x.DataSets, (setP) =>
                {
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my first series");
                        seriesP.Add(z => z.Points, firstData);
                        seriesP.Add(z => z.Color, firtSeriesColor);

                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my second series");
                        seriesP.Add(z => z.Points, secondData);
                        seriesP.Add(z => z.Color, secondSeriesColor);

                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my third series");
                        seriesP.Add(z => z.Points, thirdData);
                        seriesP.Add(z => z.Color, thirdSeriesColor);

                    });
                    setP.Add<BarChartSeries>(y => y.ChildContent, (seriesP) =>
                    {
                        seriesP.Add(z => z.Name, "my fourth series");
                        seriesP.Add(z => z.Points, fourthData);
                        seriesP.Add(z => z.Color, fourthSeriesColor);
                    });
                });
                additionalConfiguration(p);
            });
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
                new XAttribute("points", _staticRegex.Replace($"{e.P1.X.ToString(CultureInfo.InvariantCulture)},{e.P1.Y.ToString(CultureInfo.InvariantCulture)} {e.P2.X.ToString(CultureInfo.InvariantCulture)},{e.P2.Y.ToString(CultureInfo.InvariantCulture)} {e.P3.X.ToString(CultureInfo.InvariantCulture)},{e.P3.Y.ToString(CultureInfo.InvariantCulture)} {e.P4.X.ToString(CultureInfo.InvariantCulture)},{e.P4.Y.ToString(CultureInfo.InvariantCulture)}", "0"))
                )));

        private static Regex _staticRegex = new Regex(@"(\-0)");

    }

    record Point(Double X, Double Y);
    record Rectangle(Point P1, Point P2, Point P3, Point P4, String FillColor);

    static class PointTransforamtion
    {
        public static Point ScaleY(this Point p, Double value) => new Point(p.X, p.Y * value);
        public static Point MoveAlongYAxis(this Point p, Double value) => new Point(p.X, p.Y + value);
        public static Point TransformPointToSvgCoordinateSystem(this Point point) => new Point(Math.Round(point.X, 10), Math.Round((point.Y - 100) * (-1), 10));

    }
}

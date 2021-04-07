// Not Used

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.EnhanceChart;
using MudBlazor.UnitTests.TestComponents.EnhancedChart;
using NUnit.Framework;
using static MudBlazor.UnitTests.Components.EnhancedChart.MudEnhancedChartTesterHelper;

namespace MudBlazor.UnitTests.Components.EnhancedChart
{
    [TestFixture]
    public class MudEnhancedBarChart_AnimationFocuses_Tests
    {
        private Guid _defaultChartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b932");

        private String _firstSeriesColor = "#123416cc";
        private String _secondSeriesColor = "#123417cc";
        private String _thirdSeriesColor = "#123418cc";

        private String _defaultAnimationDuration = "500ms";

        private Bunit.TestContext ctx;

        private Mock<IJSRuntime> _mockedRuntime;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();

            _mockedRuntime = new Mock<IJSRuntime>(MockBehavior.Strict);
            _mockedRuntime.Setup(x => x.InvokeAsync<Object>("mudEnhancedChartHelper.triggerAnimation", new object[] { _defaultChartId })).ReturnsAsync(new Object()).Verifiable();
            ctx.Services.Add(new ServiceDescriptor(typeof(IJSRuntime), _mockedRuntime.Object));
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        private String ToInvariant(Double input) => input == 0 ? "0" : input.ToString(CultureInfo.InvariantCulture);

        private Double TransformY(Double y) => (y - 100) * (-1);

        private String GetRectanglePoints(Double x, Double y, Double width, Double height)
        {
            Point p1 = new Point(x, TransformY(y));
            Point p2 = new Point(x, TransformY(y + height));
            Point p3 = new Point(x + width, TransformY(y + height));
            Point p4 = new Point(x + width, TransformY(y));

            return $"{ToInvariant(p1.X)},{ToInvariant(p1.Y)} {ToInvariant(p2.X)},{ToInvariant(p2.Y)} {ToInvariant(p3.X)},{ToInvariant(p3.Y)} {ToInvariant(p4.X)},{ToInvariant(p4.Y)}";
        }

        [Test]
        public void InitialValues()
        {
            var comp = ctx.RenderComponent<MudEnhancedBarChartAnimationFocusesTest>();

            var root = GetElementAsXmlDocument(comp.FindComponent<MudEnhancedBarChart>());

            var firstBarAnimation = new XElement("animate",
                new XAttribute("attributename", "points"),
                new XAttribute("dur", _defaultAnimationDuration),
                new XAttribute("from", GetRectanglePoints(17.5, 10, 10, 0)),
                new XAttribute("to", GetRectanglePoints(17.5, 10, 10, 30)));

            XElement firstBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(17.5, 10, 10, 30)),
                 firstBarAnimation
                );

            var secondBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(27.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(27.5, 10, 10, 90)));

            XElement secondBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(27.5, 10, 10, 90)),
                 secondBarAnimation
                );

            var thirdBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(37.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(37.5, 10, 10, 60)));

            XElement thirdBar = new XElement("polygon",
                 new XAttribute("fill", _thirdSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(37.5, 10, 10, 60)),
                 thirdBarAnimation
                );

            var fourthBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(62.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(62.5, 10, 10, 90)));

            XElement fourthBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(62.5, 10, 10, 90)),
                 fourthBarAnimation
                );

            var fithBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(72.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(72.5, 10, 10, 60)));

            XElement fithBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(72.5, 10, 10, 60)),
                 fithBarAnimation
                );

            var sixthBarAnimation = new XElement("animate",
                  new XAttribute("attributename", "points"),
                  new XAttribute("dur", _defaultAnimationDuration),
                  new XAttribute("from", GetRectanglePoints(82.5, 10, 10, 0)),
                  new XAttribute("to", GetRectanglePoints(82.5, 10, 10, 30)));

            XElement sixthBar = new XElement("polygon",
                 new XAttribute("fill", _thirdSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(82.5, 10, 10, 30)),
                 sixthBarAnimation
                );

            XElement expectedRoot = new XElement("svg", firstBar, secondBar, thirdBar, fourthBar, fithBar, sixthBar);

            root.Should().BeEquivalentTo(expectedRoot);

            _mockedRuntime.Verify();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AddSeries(Boolean viaLegend)
        {
            var comp = ctx.RenderComponent<MudEnhancedBarChartAnimationFocusesTest>(p =>
            {
                if (viaLegend == false)
                {
                    p.Add(x => x.ShowThirdSeries, false);
                }
                else
                {
                    p.Add(x => x.ThirdSeriesIsEnabled, false);
                }
            });

            if (viaLegend == false)
            {
                comp.Instance.EnableThirdSeries();
            }
            else
            {
                comp.Instance.EnableThirdSeriesViaLgend();
            }


            var root = GetElementAsXmlDocument(comp.FindComponent<MudEnhancedBarChart>());

            var firstBarAnimation = new XElement("animate",
                new XAttribute("attributename", "points"),
                new XAttribute("dur", _defaultAnimationDuration),
                new XAttribute("from", GetRectanglePoints(17.5, 10, 15, 30)),
                new XAttribute("to", GetRectanglePoints(17.5, 10, 10, 30)));

            XElement firstBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(17.5, 10, 10, 30)),
                 firstBarAnimation
                );

            var secondBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(17.5 + 15, 10, 15, 90)),
              new XAttribute("to", GetRectanglePoints(27.5, 10, 10, 90)));

            XElement secondBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(27.5, 10, 10, 90)),
                 secondBarAnimation
                );

            var thirdBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(37.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(37.5, 10, 10, 60)));

            XElement thirdBar = new XElement("polygon",
                 new XAttribute("fill", _thirdSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(37.5, 10, 10, 60)),
                 thirdBarAnimation
                );

            var fourthBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(62.5, 10, 15, 90)),
              new XAttribute("to", GetRectanglePoints(62.5, 10, 10, 90)));

            XElement fourthBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(62.5, 10, 10, 90)),
                 fourthBarAnimation
                );

            var fithBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(62.5 + 15, 10, 15, 60)),
              new XAttribute("to", GetRectanglePoints(72.5, 10, 10, 60)));

            XElement fithBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(72.5, 10, 10, 60)),
                 fithBarAnimation
                );

            var sixthBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(82.5, 10, 10, 0)),
              new XAttribute("to", GetRectanglePoints(82.5, 10, 10, 30)));

            XElement sixthBar = new XElement("polygon",
                 new XAttribute("fill", _thirdSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(82.5, 10, 10, 30)),
                 sixthBarAnimation
                );

            XElement expectedRoot = new XElement("svg", firstBar, secondBar, thirdBar, fourthBar, fithBar, sixthBar);

            root.Should().BeEquivalentTo(expectedRoot);

            _mockedRuntime.Verify();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RemoveSeries(Boolean viaLegend)
        {
            var comp = ctx.RenderComponent<MudEnhancedBarChartAnimationFocusesTest>();

            if (viaLegend == true)
            {
                comp.Instance.RemoveThirdSeriesByLegend();
            }
            else
            {
                comp.Instance.RemoveThirdSeries();
            }

            var root = GetElementAsXmlDocument(comp.FindComponent<MudEnhancedBarChart>());

            var firstBarAnimation = new XElement("animate",
                new XAttribute("attributename", "points"),
                new XAttribute("dur", _defaultAnimationDuration),
                new XAttribute("from", GetRectanglePoints(17.5, 10, 10, 30)),
                new XAttribute("to", GetRectanglePoints(17.5, 10, 15, 30)));

            XElement firstBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(17.5, 10, 15, 30)),
                 firstBarAnimation
                );

            var secondBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(27.5, 10, 10, 90)),
              new XAttribute("to", GetRectanglePoints(17.5 + 15, 10, 15, 90)));

            XElement secondBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(17.5 + 15, 10, 15, 90)),
                 secondBarAnimation
                );


            var fourthBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(62.5, 10, 10, 90)),
              new XAttribute("to", GetRectanglePoints(62.5, 10, 15, 90)));

            XElement fourthBar = new XElement("polygon",
                 new XAttribute("fill", _firstSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(62.5, 10, 15, 90)),
                 fourthBarAnimation
                );

            var fithBarAnimation = new XElement("animate",
              new XAttribute("attributename", "points"),
              new XAttribute("dur", _defaultAnimationDuration),
              new XAttribute("from", GetRectanglePoints(72.5, 10, 10, 60)),
              new XAttribute("to", GetRectanglePoints(62.5 + 15, 10, 15, 60)));

            XElement fithBar = new XElement("polygon",
                 new XAttribute("fill", _secondSeriesColor),
                 new XAttribute("data-chartid", _defaultChartId.ToString()),
                 new XAttribute("class", "mud-enhanced-chart-series bar active"),
                 new XAttribute("points", GetRectanglePoints(62.5 + 15, 10, 15, 60)),
                 fithBarAnimation
                );


            XElement expectedRoot = new XElement("svg", firstBar, secondBar, fourthBar, fithBar);

            root.Should().BeEquivalentTo(expectedRoot);

            _mockedRuntime.Verify();
        }
        [Test]
        public void ToggleActivityNotTriggeredAnimation()
        {
            var comp = ctx.RenderComponent<MudEnhancedBarChartAnimationFocusesTest>();

            for (int i = 0; i < 6; i++)
            {
                Int32 seriesIndex = i % 3;
                var series = comp.FindComponents<MudEnhancedBarChartSeries>();
                comp.InvokeAsync(() =>
               {
                   series.ElementAt(seriesIndex).Instance.SentRequestToBecomeActiveAlone();
               });

                var root = GetElementAsXmlDocument(comp.FindComponent<MudEnhancedBarChart>());

                var firstBarAnimation = new XElement("animate",
                                new XAttribute("attributename", "points"),
                                new XAttribute("dur", _defaultAnimationDuration),
                                new XAttribute("from", GetRectanglePoints(17.5, 10, 10, 0)),
                                new XAttribute("to", GetRectanglePoints(17.5, 10, 10, 30)));

                XElement firstBar = new XElement("polygon",
                     new XAttribute("fill", _firstSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 0 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(17.5, 10, 10, 30)),
                     firstBarAnimation
                    );

                var secondBarAnimation = new XElement("animate",
                  new XAttribute("attributename", "points"),
                  new XAttribute("dur", _defaultAnimationDuration),
                  new XAttribute("from", GetRectanglePoints(27.5, 10, 10, 0)),
                  new XAttribute("to", GetRectanglePoints(27.5, 10, 10, 90)));

                XElement secondBar = new XElement("polygon",
                     new XAttribute("fill", _secondSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 1 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(27.5, 10, 10, 90)),
                     secondBarAnimation
                    );

                var thirdBarAnimation = new XElement("animate",
                  new XAttribute("attributename", "points"),
                  new XAttribute("dur", _defaultAnimationDuration),
                  new XAttribute("from", GetRectanglePoints(37.5, 10, 10, 0)),
                  new XAttribute("to", GetRectanglePoints(37.5, 10, 10, 60)));

                XElement thirdBar = new XElement("polygon",
                     new XAttribute("fill", _thirdSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 2 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(37.5, 10, 10, 60)),
                     thirdBarAnimation
                    );

                var fourthBarAnimation = new XElement("animate",
                  new XAttribute("attributename", "points"),
                  new XAttribute("dur", _defaultAnimationDuration),
                  new XAttribute("from", GetRectanglePoints(62.5, 10, 10, 0)),
                  new XAttribute("to", GetRectanglePoints(62.5, 10, 10, 90)));

                XElement fourthBar = new XElement("polygon",
                     new XAttribute("fill", _firstSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 0 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(62.5, 10, 10, 90)),
                     fourthBarAnimation
                    );

                var fithBarAnimation = new XElement("animate",
                  new XAttribute("attributename", "points"),
                  new XAttribute("dur", _defaultAnimationDuration),
                  new XAttribute("from", GetRectanglePoints(72.5, 10, 10, 0)),
                  new XAttribute("to", GetRectanglePoints(72.5, 10, 10, 60)));

                XElement fithBar = new XElement("polygon",
                     new XAttribute("fill", _secondSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 1 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(72.5, 10, 10, 60)),
                     fithBarAnimation
                    );

                var sixthBarAnimation = new XElement("animate",
                      new XAttribute("attributename", "points"),
                      new XAttribute("dur", _defaultAnimationDuration),
                      new XAttribute("from", GetRectanglePoints(82.5, 10, 10, 0)),
                      new XAttribute("to", GetRectanglePoints(82.5, 10, 10, 30)));

                XElement sixthBar = new XElement("polygon",
                     new XAttribute("fill", _thirdSeriesColor),
                     new XAttribute("data-chartid", _defaultChartId.ToString()),
                     new XAttribute("class", $"mud-enhanced-chart-series bar {(seriesIndex == 2 ? "active" : "inactive")}"),
                     new XAttribute("points", GetRectanglePoints(82.5, 10, 10, 30)),
                     sixthBarAnimation
                    );

                XElement expectedRoot = new XElement("svg", firstBar, secondBar, thirdBar, fourthBar, fithBar, sixthBar);

                root.Should().BeEquivalentTo(expectedRoot);

                _mockedRuntime.Verify(
                    x => x.InvokeAsync<Object>("mudEnhancedChartHelper.triggerAnimation", new object[] { _defaultChartId }), Times.Exactly(1));
            }
        }
    }
}

// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using Bunit.Rendering;
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
    public class MudEnhancedDonutChartTests
    {
        private Guid _defaultChartId = Guid.Parse("ed8a9a45-f109-41b9-9ff6-074ad168b934");

        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();

            var mockedRuntime = new Mock<IJSRuntime>(MockBehavior.Strict);
            mockedRuntime.Setup(x => x.InvokeAsync<Object>("mudEnhancedChartHelper.triggerAnimation", new object[] { _defaultChartId })).ReturnsAsync(new Object()).Verifiable();
            ctx.Services.Add(new ServiceDescriptor(typeof(IJSRuntime), mockedRuntime.Object));
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void DefaultValues()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedDonutChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<MudEnhancedDonutChartDataPoint> points = comp.Instance;
            points.Should().BeEmpty();

            called.Should().Be(1);

            comp.Instance.Padding.Should().Be(2.0);
            comp.Instance.Thickness.Should().Be(20.0);
            comp.Instance.StartAngle.Should().Be(-90.0);
            comp.Instance.AnimationIsEnabled.Should().BeFalse();
            comp.Instance.Id.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void UpdatingValuesResultsInRedraw()
        {
            Int32 called = 0;

            var comp = ctx.RenderComponent<MudEnhancedDonutChart>(p =>
            {
                p.Add(x => x.BeforeCreatingInstructionCallBack, (chart) =>
                {
                    called++;
                });
            });

            ICollection<MudEnhancedDonutChartDataPoint> points = comp.Instance;
            points.Should().BeEmpty();

            called.Should().Be(1);

            called = 0;
            //setting default values changed trigger redraw
            comp.SetParametersAndRender(x => x.Add(y => y.StartAngle, -90.0));
            comp.SetParametersAndRender(x => x.Add(y => y.StartAngle, 0.0));
            called.Should().Be(1);

            called = 0;
            //setting default values changed trigger redraw
            comp.SetParametersAndRender(x => x.Add(y => y.Padding, 2.0));
            comp.SetParametersAndRender(x => x.Add(y => y.Padding, 10.0));
            called.Should().Be(1);

            called = 0;
            //setting default values changed trigger redraw
            comp.SetParametersAndRender(x => x.Add(y => y.Thickness, 20.0));
            comp.SetParametersAndRender(x => x.Add(y => y.Thickness, 10.0));
            called.Should().Be(1);

            called = 0;
            comp.SetParametersAndRender(p =>
            {
                p.Add<MudEnhancedDonutChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 0.0);
                });
            });

            called.Should().Be(1);
        }

        [Test]
        public void MultipleValue()
        {
            String firstPointColor = (Colors.BlueGrey.Darken1 + "ff").ToLower();
            String secondPointColor = (Colors.Amber.Darken1 + "ff").ToLower();
            String thirdPointColor = (Colors.Brown.Darken1 + "ff").ToLower();
            String fourthPointColor = (Colors.Green.Darken1 + "ff").ToLower();

            var comp = ctx.RenderComponent<MudEnhancedDonutChart>(p =>
            {
                p.Add(x => x.Id, _defaultChartId);
                p.Add(x => x.Padding, 20.0);
                p.Add(x => x.StartAngle, 0.0);
                p.Add(x => x.AnimationIsEnabled, false);
                p.Add<MudEnhancedDonutChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 250.0);
                    setP.Add(x => x.FillColor, firstPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-1");
                });
                p.Add<MudEnhancedDonutChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 500.0);
                    setP.Add(x => x.FillColor, secondPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-2");
                });
                p.Add<MudEnhancedDonutChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 125);
                    setP.Add(x => x.FillColor, thirdPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-3");
                });
                p.Add<MudEnhancedDonutChartDataPoint>(x => x.ChildContent, (setP) =>
                {
                    setP.Add(x => x.Value, 125);
                    setP.Add(x => x.FillColor, fourthPointColor);
                    setP.Add(x => x.AddtionalClass, "my-additional-class-4");
                });
            });

            XElement expectedRoot = new XElement("svg",
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series donut active my-additional-class-1"),
                new XAttribute("fill", (object)firstPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 60 50 L 80 50 A 30 30,0,0,1, 50 80 L 50 60  A 10 10,0,0,0, 60 50")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series donut active my-additional-class-2"),
                new XAttribute("fill", (object)secondPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 60 L 50 80 A 30 30,0,0,1, 50 20 L 50 40  A 10 10,0,0,0, 50 60")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series donut active my-additional-class-3"),
                new XAttribute("fill", (object)thirdPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 50 40 L 50 20 A 30 30,0,0,1, 71.213203 28.786797 L 57.071068 42.928932  A 10 10,0,0,0, 50 40")
                ),
                new XElement("path",
                new XAttribute("data-chartid", _defaultChartId),
                new XAttribute("class", "mud-enhanced-chart-series donut active my-additional-class-4"),
                new XAttribute("fill", (object)fourthPointColor),
                new XAttribute("stroke", "none"),
                new XAttribute("d", "M 57.071068 42.928932 L 71.213203 28.786797 A 30 30,0,0,1, 80 50 L 60 50  A 10 10,0,0,0, 57.071068 42.928932")
                ));

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
    }
}

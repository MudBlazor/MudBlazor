using System.Reflection;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class ChartToolTipTests : BunitTest
    {
        [SetUp]
        public void Init()
        {

        }

        [SetCulture("ru")]
        [Test]
        public void BarChartEmptyData()
        {
            var comp = Context.RenderComponent<ChartTooltip>(parameters => parameters
                    .Add(p => p.Title, "Some Title")
                    .Add(p => p.Subtitle, "Some Subtitle")
                    .Add(p => p.X, 10.05)
                    .Add(p => p.Y, 20.02)
                    .Add(p => p.Color, "red")
                );

            comp.Markup.Should().Contain("<g class=\"svg-tooltip\" opacity=\"1\" style=\"pointer-events: none;\">");
            comp.Markup.Should().Contain("<polygon points=\"2.05,12.02 18.05,12.02 10.05,18.52\" fill=\"red\" stroke=\"white\" stroke-width=\"2\"></polygon>");
            comp.Markup.Should().Contain("<rect x=\"-9.95\" y=\"-25.98\" width=\"40\" height=\"40\" rx=\"4\" ry=\"4\" fill=\"red\" stroke=\"white\" stroke-width=\"1\"></rect>");
            comp.Markup.Should().Contain("<polygon points=\"0.05,10.02 20.05,10.02 10.05,18.52\" fill=\"red\" stroke=\"white\" stroke-width=\"0\"></polygon>");
            comp.Markup.Should().Contain("<text x=\"10.05\" y=\"-9.98\"");
            comp.Markup.Should().Contain("<text x=\"10.05\" y=\"-9.98\" text-anchor=\"middle\" font-size=\"12px\" blazor:elementReference=\"\"><tspan x=\"10.05\" dy=\"0\">Some Title</tspan><tspan x=\"10.05\" dy=\"16\">Some Subtitle</tspan></text>");
        }
    }
}

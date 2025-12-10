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
            var comp = Context.Render<ChartTooltip>(parameters => parameters
                    .Add(p => p.Title, "Some Title")
                    .Add(p => p.Subtitle, "Some Subtitle")
                    .Add(p => p.X, 10.05)
                    .Add(p => p.Y, 20.02)
                    .Add(p => p.Color, "red")
                );

            comp.Markup.Should().Contain("<g class=\"svg-tooltip\" style=\"pointer-events: none;\">");
            comp.Markup.Should().Contain("<polygon points=\"2.05,12.02 18.05,12.02 10.05,18.02\" fill=\"red\" stroke=\"white\" stroke-width=\"2\"></polygon>");
            comp.Markup.Should().Contain("<rect x=\"-2.95\" y=\"-8.48\" width=\"26\" height=\"15\" rx=\"4\" ry=\"4\" fill=\"red\" stroke=\"white\" stroke-width=\"1\"></rect>");
            comp.Markup.Should().Contain("<polygon points=\"0.05,10.02 20.05,10.02 10.05,18.02\" fill=\"red\"></polygon>");
            comp.Markup.Should().Contain("<text x=\"10.05\" y=\"2.02\" font-size=\"12px\" fill=\"white\" text-anchor=\"middle\" stroke=\"black\" stroke-width=\"0.8\" paint-order=\"stroke\" filter=\"url(#text-shadow)\" blazor:elementReference=\"\">");
            comp.Markup.Should().Contain("<tspan x=\"10.05\" dy=\"-.3em\">Some Title</tspan><tspan x=\"10.05\" dy=\"0\">Some Subtitle</tspan></text>");
        }
    }
}

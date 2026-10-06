using AwesomeAssertions;
using Bunit;
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

        [Test]
        public void NonFiniteCoordinatesSettleInsteadOfLoopingTheRenderer()
        {
            // Recalculating the box ends in StateHasChanged, so the change check has to latch or every render queues another one.
            // NaN != NaN is always true, so a non-finite coordinate used to recurse until the process died with a stack overflow.
            var comp = Context.Render<ChartTooltip>(parameters => parameters
                    .Add(p => p.Title, "Some Title")
                    .Add(p => p.X, double.NaN)
                    .Add(p => p.Y, double.NaN)
                );

            // One initial render plus the one the recalculation asks for, then it settles.
            comp.RenderCount.Should().BeLessThanOrEqualTo(3);
        }

        /// <summary>
        /// Verifies that a tooltip stays hidden until its text has been measured, instead of showing the text at the origin without its background (#13422).
        /// </summary>
        [Test]
        public async Task TooltipStaysHiddenUntilItsTextIsMeasured()
        {
            var measurement = Context.JSInterop.Setup<ChartTooltip.BBox>("mudGetSvgBBox", _ => true);

            var comp = Context.Render<ChartTooltip>(parameters => parameters
                    .Add(p => p.Title, "Some Title")
                    .Add(p => p.X, 200)
                    .Add(p => p.Y, 100)
                );

            // The text is still rendered because the browser has to measure it.
            comp.Find("g.svg-tooltip").GetAttribute("visibility").Should().Be("hidden");
            comp.Find("g.svg-tooltip text").TextContent.Should().Contain("Some Title");

            // Another render before the measurement returns keeps it hidden too.
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Color, "red"));
            comp.Find("g.svg-tooltip").GetAttribute("visibility").Should().Be("hidden");

            measurement.SetResult(new ChartTooltip.BBox(Width: 100, Height: 14));

            comp.WaitForAssertion(() => comp.Find("g.svg-tooltip").HasAttribute("visibility").Should().BeFalse());
            comp.Find("g.svg-tooltip rect").GetAttribute("width").Should().Be("110");
            comp.Find("g.svg-tooltip text").GetAttribute("x").Should().Be("200");
        }

        /// <summary>
        /// Verifies that a measured tooltip stays visible while it is measured again for a new text, so labels that update don't blink (#13422).
        /// </summary>
        [Test]
        public async Task TooltipStaysVisibleWhileItIsMeasuredAgain()
        {
            Context.JSInterop.Setup<ChartTooltip.BBox>("mudGetSvgBBox", _ => true).SetResult(new ChartTooltip.BBox(Width: 100, Height: 14));

            var comp = Context.Render<ChartTooltip>(parameters => parameters
                    .Add(p => p.Title, "Some Title")
                    .Add(p => p.X, 200)
                    .Add(p => p.Y, 100)
                );

            comp.WaitForAssertion(() => comp.Find("g.svg-tooltip").HasAttribute("visibility").Should().BeFalse());

            // The latest setup takes precedence, so the next measurement stays pending until it gets a result.
            var remeasurement = Context.JSInterop.Setup<ChartTooltip.BBox>("mudGetSvgBBox", _ => true);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                    .Add(p => p.Title, "A longer title")
                    .Add(p => p.X, 50)
                );

            comp.Find("g.svg-tooltip").HasAttribute("visibility").Should().BeFalse();
            remeasurement.Invocations.Should().ContainSingle();

            // Another render before the measurement returns keeps it visible too.
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Color, "red"));
            comp.Find("g.svg-tooltip").HasAttribute("visibility").Should().BeFalse();

            remeasurement.SetResult(new ChartTooltip.BBox(Width: 60, Height: 14));

            comp.WaitForAssertion(() => comp.Find("g.svg-tooltip rect").GetAttribute("width").Should().Be("70"));
            comp.Find("g.svg-tooltip text").GetAttribute("x").Should().Be("50");
            comp.Find("g.svg-tooltip").HasAttribute("visibility").Should().BeFalse();
        }
    }
}
